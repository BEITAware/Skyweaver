using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Ferrita.Controls.ChatSessionControl.Views;
using Ferrita.Models.Search;
using Ferrita.Services.Search;
using Ferrita.Services.FerritaTools;

namespace Ferrita.Tools
{
    /// <summary>
    /// WebSearch 工具，支持通过首选项中配置的 API (Brave Search、Tavily 或 Vertex AI Search) 进行网络检索
    /// </summary>
    public sealed class WebSearchTool :
        IFerritaTool,
        IFerritaToolInvocationPresentationProvider,
        IFerritaToolPromptDescriptionProvider
    {
        public const string ToolName = "WebSearch";

        private static readonly HttpClient s_httpClient = new();

        private static readonly FerritaToolDefinition s_definition = new(
            ToolName,
            "使用设置中配置的 Brave Search、Tavily 或 Google Vertex AI Search 搜索网页内容，返回包含页面网址、标题和摘要的结构化结果。",
            "WebSearch",
            new[]
            {
                new FerritaToolParameterDefinition(
                    "Queries",
                    "搜索关键词。可以传入单个字符串，或者使用 XML 标签格式传入多个关键词，例如 <Query>A</Query><Query>B</Query>。",
                    FerritaToolParameterType.String,
                    isRequired: true)
            },
            defaultAgentPermission: FerritaToolDefaultAgentPermission.Allow,
            defaultToolKitKeys: new[] { "Web" });

        public FerritaToolDefinition Definition => s_definition;

        public string GetPromptDescription(FerritaToolPromptDescriptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return "根据用户激活的搜索 API 首选项在 Web 上进行搜索。支持在 Queries 参数中指定单个查询或使用 XML 格式（例如 <Query>A</Query><Query>B</Query>）指定多个查询。返回整合的结果列表。每个结果都包含标题、网页 URL 以及文本摘要。输出内容将始终包含来源网址。";
        }

        public FrameworkElement? CreateInvocationPresentation(FerritaToolInvocationPresentationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return ToolInvocationCardFactory.CreateWebSearch(
                context,
                new[]
                {
                    new ToolInvocationCardFieldDefinition("Queries", "Queries", "正在等待搜索查询...")
                });
        }

        public async Task<FerritaToolResult> ExecuteAsync(
            FerritaToolContext context,
            FerritaToolArguments arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var queriesText = arguments.GetString("Queries") ?? string.Empty;
            if (string.IsNullOrWhiteSpace(queriesText))
            {
                return FerritaToolResult.Failure("Queries 参数不能为空。");
            }

            var queries = ParseQueries(queriesText);
            if (queries.Count == 0)
            {
                return FerritaToolResult.Failure("无法从 Queries 参数中提取出有效的搜索查询词。");
            }

            try
            {
                var configRepo = new SearchConfigurationRepository();
                var config = configRepo.Load();

                var results = new List<SearchResultItem>();
                string? globalAnswer = null;

                // 遍历每个查询词进行搜索并整合结果
                foreach (var q in queries)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var searchRes = await PerformSearchAsync(config, q, cancellationToken).ConfigureAwait(false);
                    if (searchRes.Items != null)
                    {
                        results.AddRange(searchRes.Items);
                    }
                    if (!string.IsNullOrWhiteSpace(searchRes.Answer))
                    {
                        globalAnswer = searchRes.Answer; // 保留结果中的大模型直接回答
                    }
                }

                // 根据 URL 进行去重
                var uniqueResults = new List<SearchResultItem>();
                var seenUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var item in results)
                {
                    if (string.IsNullOrWhiteSpace(item.Url)) continue;
                    if (seenUrls.Add(item.Url))
                    {
                        uniqueResults.Add(item);
                    }
                }

                var content = FormatResults(uniqueResults, globalAnswer);
                var data = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                {
                    ["selectedApi"] = config.SelectedApi.ToString(),
                    ["queries"] = string.Join(", ", queries),
                    ["resultCount"] = uniqueResults.Count
                };

                return FerritaToolResult.Success(content, data);
            }
            catch (Exception ex)
            {
                return FerritaToolResult.Failure($"网页搜索失败: {ex.Message}");
            }
        }

        private static List<string> ParseQueries(string queriesText)
        {
            var queries = new List<string>();
            var normalized = queriesText.Trim();
            if (normalized.Length == 0) return queries;

            try
            {
                // 尝试用外层根节点包起来以支持 XML 解析
                var wrapped = $"<root>{normalized}</root>";
                var doc = XDocument.Parse(wrapped);
                var elements = doc.Root?.Elements("Query").ToList();
                if (elements != null && elements.Count > 0)
                {
                    foreach (var elem in elements)
                    {
                        if (!string.IsNullOrWhiteSpace(elem.Value))
                        {
                            queries.Add(elem.Value.Trim());
                        }
                    }
                }
            }
            catch
            {
                // 解析失败说明只是普通的单个查询关键词，直接采纳
            }

            if (queries.Count == 0)
            {
                queries.Add(normalized);
            }

            return queries;
        }

        private static async Task<SearchResponse> PerformSearchAsync(
            SearchConfiguration config,
            string query,
            CancellationToken cancellationToken)
        {
            switch (config.SelectedApi)
            {
                case SearchApiType.BraveSearch:
                    return await SearchBraveAsync(config, query, cancellationToken).ConfigureAwait(false);
                case SearchApiType.Tavily:
                    return await SearchTavilyAsync(config, query, cancellationToken).ConfigureAwait(false);
                case SearchApiType.VertexAiSearch:
                    return await SearchVertexAsync(config, query, cancellationToken).ConfigureAwait(false);
                default:
                    throw new InvalidOperationException($"不支持或未知的搜索 API 类型: {config.SelectedApi}");
            }
        }

        private static async Task<SearchResponse> SearchBraveAsync(
            SearchConfiguration config,
            string query,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(config.BraveSearchApiKey))
            {
                throw new InvalidOperationException("未配置 Brave Search API 密钥。");
            }

            var url = $"https://api.search.brave.com/res/v1/web/search" +
                      $"?q={Uri.EscapeDataString(query)}" +
                      $"&country={Uri.EscapeDataString(config.BraveSearchCountry)}" +
                      $"&search_lang={Uri.EscapeDataString(config.BraveSearchLanguage)}" +
                      $"&safesearch={Uri.EscapeDataString(config.BraveSearchSafeSearch)}" +
                      $"&count={config.BraveSearchCount}";

            if (!string.Equals(config.BraveSearchFreshness, "none", StringComparison.OrdinalIgnoreCase))
            {
                url += $"&freshness={Uri.EscapeDataString(config.BraveSearchFreshness)}";
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("X-Subscription-Token", config.BraveSearchApiKey);

            using var response = await s_httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Brave Search 请求失败，HTTP 状态码为 {(int)response.StatusCode}: {responseBody}");
            }

            var items = new List<SearchResultItem>();
            using var doc = JsonDocument.Parse(responseBody);
            if (doc.RootElement.TryGetProperty("web", out var webNode) &&
                webNode.TryGetProperty("results", out var resultsNode) &&
                resultsNode.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in resultsNode.EnumerateArray())
                {
                    var title = item.TryGetProperty("title", out var t) ? t.GetString() : string.Empty;
                    var itemUrl = item.TryGetProperty("url", out var u) ? u.GetString() : string.Empty;
                    var description = item.TryGetProperty("description", out var d) ? d.GetString() : string.Empty;

                    if (!string.IsNullOrWhiteSpace(itemUrl))
                    {
                        items.Add(new SearchResultItem(title ?? string.Empty, itemUrl, description ?? string.Empty));
                    }
                }
            }

            return new SearchResponse(items, null);
        }

        private static async Task<SearchResponse> SearchTavilyAsync(
            SearchConfiguration config,
            string query,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(config.TavilyApiKey))
            {
                throw new InvalidOperationException("未配置 Tavily API 密钥。");
            }

            var payload = new
            {
                api_key = config.TavilyApiKey,
                query = query,
                search_depth = config.TavilySearchDepth,
                include_answer = config.TavilyIncludeAnswer,
                topic = config.TavilyTopic,
                max_results = config.TavilyMaxResults,
                include_images = config.TavilyIncludeImages,
                include_raw_content = config.TavilyIncludeRawContent
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            using var response = await s_httpClient.PostAsync("https://api.tavily.com/search", jsonContent, cancellationToken).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Tavily Search 请求失败，HTTP 状态码为 {(int)response.StatusCode}: {responseBody}");
            }

            var items = new List<SearchResultItem>();
            string? answer = null;

            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;
            if (root.TryGetProperty("results", out var resultsNode) && resultsNode.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in resultsNode.EnumerateArray())
                {
                    var title = item.TryGetProperty("title", out var t) ? t.GetString() : string.Empty;
                    var itemUrl = item.TryGetProperty("url", out var u) ? u.GetString() : string.Empty;
                    var content = item.TryGetProperty("content", out var c) ? c.GetString() : string.Empty;

                    if (!string.IsNullOrWhiteSpace(itemUrl))
                    {
                        items.Add(new SearchResultItem(title ?? string.Empty, itemUrl, content ?? string.Empty));
                    }
                }
            }

            if (root.TryGetProperty("answer", out var ansNode) && ansNode.ValueKind == JsonValueKind.String)
            {
                answer = ansNode.GetString();
            }

            return new SearchResponse(items, answer);
        }

        private static async Task<SearchResponse> SearchVertexAsync(
            SearchConfiguration config,
            string query,
            CancellationToken cancellationToken)
        {
            var projectId = config.VertexAiProjectId;
            var location = config.VertexAiLocation;
            var dataStoreId = config.VertexAiDataStoreId;

            if (string.IsNullOrWhiteSpace(projectId) || string.IsNullOrWhiteSpace(dataStoreId))
            {
                throw new InvalidOperationException("未配置 Vertex AI Search ProjectId 或 DataStoreId。");
            }

            var locationEndpoint = string.Equals(location, "global", StringComparison.OrdinalIgnoreCase)
                ? "discoveryengine.googleapis.com"
                : $"{location.ToLowerInvariant()}-discoveryengine.googleapis.com";

            var url = $"https://{locationEndpoint}/v1beta/projects/{projectId}/locations/{location}/dataStores/{dataStoreId}/servingConfigs/default_search:search";

            // 处理认证逻辑
            string? token = null;
            if (string.IsNullOrWhiteSpace(config.VertexAiApiKey))
            {
                if (string.IsNullOrWhiteSpace(config.VertexAiCredentialsJson))
                {
                    throw new InvalidOperationException("Vertex AI Search API Key 或 CredentialsJson 必须配置其中之一。");
                }
                token = await GetGoogleAccessTokenAsync(config.VertexAiCredentialsJson, s_httpClient, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                url += $"?key={Uri.EscapeDataString(config.VertexAiApiKey)}";
            }

            var payload = new
            {
                query = query,
                pageSize = config.VertexAiMaxResults
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = jsonContent;
            if (token != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            using var response = await s_httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Vertex AI Search 请求失败，HTTP 状态码为 {(int)response.StatusCode}: {responseBody}");
            }

            var items = new List<SearchResultItem>();
            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;
            if (root.TryGetProperty("results", out var resultsNode) && resultsNode.ValueKind == JsonValueKind.Array)
            {
                foreach (var result in resultsNode.EnumerateArray())
                {
                    if (result.TryGetProperty("document", out var docNode))
                    {
                        string? title = null;
                        string? link = null;
                        string? snippet = null;

                        // 尝试从 derivedStructData 获取数据
                        if (docNode.TryGetProperty("derivedStructData", out var derivedNode))
                        {
                            title = derivedNode.TryGetProperty("title", out var t) ? t.GetString() : null;
                            link = derivedNode.TryGetProperty("link", out var l) ? l.GetString() : null;
                            
                            if (derivedNode.TryGetProperty("snippets", out var snippetsNode) && snippetsNode.ValueKind == JsonValueKind.Array)
                            {
                                var firstSnippet = snippetsNode.EnumerateArray().FirstOrDefault();
                                if (firstSnippet.ValueKind == JsonValueKind.Object)
                                {
                                    snippet = firstSnippet.TryGetProperty("snippet", out var s) ? s.GetString() : null;
                                }
                            }
                        }

                        // 回退从 structData 获取数据
                        if (string.IsNullOrWhiteSpace(link) && docNode.TryGetProperty("structData", out var structNode))
                        {
                            title ??= structNode.TryGetProperty("title", out var t) ? t.GetString() : null;
                            link ??= structNode.TryGetProperty("link", out var l) ? l.GetString() : null;
                            link ??= structNode.TryGetProperty("url", out var u) ? u.GetString() : null;
                            snippet ??= structNode.TryGetProperty("snippet", out var s) ? s.GetString() : null;
                            snippet ??= structNode.TryGetProperty("content", out var c) ? c.GetString() : null;
                        }

                        if (!string.IsNullOrWhiteSpace(link))
                        {
                            items.Add(new SearchResultItem(title ?? string.Empty, link, snippet ?? string.Empty));
                        }
                    }
                }
            }

            return new SearchResponse(items, null);
        }

        private static async Task<string> GetGoogleAccessTokenAsync(
            string credentialsJson,
            HttpClient httpClient,
            CancellationToken cancellationToken)
        {
            using var doc = JsonDocument.Parse(credentialsJson);
            var root = doc.RootElement;
            var clientEmail = root.GetProperty("client_email").GetString() ?? throw new InvalidOperationException("Google凭据中缺少 client_email。");
            var privateKey = root.GetProperty("private_key").GetString() ?? throw new InvalidOperationException("Google凭据中缺少 private_key。");

            var header = Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"alg\":\"RS256\",\"typ\":\"JWT\"}"))
                .Replace("=", "").Replace("+", "-").Replace("/", "_");

            var iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var exp = iat + 3600;
            var payloadJson = $"{{\"iss\":\"{clientEmail}\",\"scope\":\"https://www.googleapis.com/auth/cloud-platform\",\"aud\":\"https://oauth2.googleapis.com/token\",\"exp\":{exp},\"iat\":{iat}}}";
            var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(payloadJson))
                .Replace("=", "").Replace("+", "-").Replace("/", "_");

            var rawAssertion = $"{header}.{payload}";
            byte[] signature;
            using (var rsa = RSA.Create())
            {
                rsa.ImportFromPem(privateKey.ToCharArray());
                signature = rsa.SignData(Encoding.UTF8.GetBytes(rawAssertion), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }

            var assertion = Convert.ToBase64String(signature).Replace("=", "").Replace("+", "-").Replace("/", "_");
            var jwt = $"{rawAssertion}.{assertion}";

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
                new KeyValuePair<string, string>("assertion", jwt)
            });

            using var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token", content, cancellationToken).ConfigureAwait(false);
            var responseStr = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"生成 Google Access Token 失败: {response.StatusCode} - {responseStr}");
            }

            using var responseDoc = JsonDocument.Parse(responseStr);
            return responseDoc.RootElement.GetProperty("access_token").GetString() ?? throw new InvalidOperationException("未在 Google 授权响应中找到 access_token。");
        }

        private static string FormatResults(List<SearchResultItem> results, string? answer)
        {
            var builder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(answer))
            {
                builder.AppendLine("=== 摘要 / 直接回答 ===");
                builder.AppendLine(answer);
                builder.AppendLine();
            }

            builder.AppendLine("=== 网页搜索结果 ===");
            if (results.Count == 0)
            {
                builder.AppendLine("未找到相关结果。");
            }
            else
            {
                for (var i = 0; i < results.Count; i++)
                {
                    var item = results[i];
                    builder.AppendLine($"[{i + 1}] 标题: {item.Title}");
                    builder.AppendLine($"    网址: {item.Url}");
                    if (!string.IsNullOrWhiteSpace(item.Snippet))
                    {
                        builder.AppendLine($"    摘要: {item.Snippet}");
                    }
                    builder.AppendLine();
                }
            }

            return builder.ToString().TrimEnd();
        }

        private sealed record SearchResponse(List<SearchResultItem>? Items, string? Answer);
        private sealed record SearchResultItem(string Title, string Url, string Snippet);
    }
}
