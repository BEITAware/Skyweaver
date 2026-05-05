using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Xaml;
using System.Xaml.Permissions;

namespace Skyweaver.Services.LiveXaml
{
    public enum LiveXamlDiagnosticSeverity
    {
        Info = 0,
        Warning = 1,
        Error = 2
    }

    public sealed record LiveXamlDiagnostic(
        LiveXamlDiagnosticSeverity Severity,
        string Message,
        string? FilePath = null,
        int? LineNumber = null,
        int? ColumnNumber = null)
    {
        public string ToDisplayString()
        {
            var location = FilePath == null
                ? string.Empty
                : LineNumber is > 0 && ColumnNumber is > 0
                    ? $"{FilePath}({LineNumber},{ColumnNumber})"
                    : FilePath;
            return location.Length == 0
                ? $"[{Severity}] {Message}"
                : $"[{Severity}] {location}: {Message}";
        }
    }

    public sealed class LiveXamlLoadResult
    {
        public bool IsSuccess { get; init; }

        public string Summary { get; init; } = string.Empty;

        public string XamlFilePath { get; init; } = string.Empty;

        public string? CodeBehindFilePath { get; init; }

        public string? RootClassName { get; init; }

        public string? RootElementTypeName { get; init; }

        public FrameworkElement? View { get; init; }

        public IReadOnlyList<LiveXamlDiagnostic> Diagnostics { get; init; } = Array.Empty<LiveXamlDiagnostic>();

        public string BuildDiagnosticsText()
        {
            return Diagnostics.Count == 0
                ? string.Empty
                : string.Join(Environment.NewLine, Diagnostics.Select(item => item.ToDisplayString()));
        }
    }

    public static class LiveXamlRuntime
    {
        private const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
        private const string MarkupCompatibilityNamespace = "http://schemas.openxmlformats.org/markup-compatibility/2006";
        private const string GeneratedInitializationFlagFieldName = "__skyweaverLiveXamlIsInitialized";
        private const string GeneratedCodeAttributeFullName = "System.CodeDom.Compiler.GeneratedCodeAttribute";

        public static LiveXamlLoadResult Validate(string absoluteXamlFilePath, string? absoluteCodeBehindFilePath = null)
        {
            return Execute(absoluteXamlFilePath, absoluteCodeBehindFilePath, instantiatePreview: false);
        }

        public static LiveXamlLoadResult LoadPreview(string absoluteXamlFilePath, string? absoluteCodeBehindFilePath = null)
        {
            return Execute(absoluteXamlFilePath, absoluteCodeBehindFilePath, instantiatePreview: true);
        }

        public static void InitializeGeneratedComponent(object rootObject, string absoluteXamlFilePath)
        {
            ArgumentNullException.ThrowIfNull(rootObject);

            var normalizedXamlFilePath = LiveXamlFileSupport.NormalizeAbsoluteXamlPath(absoluteXamlFilePath);
            if (!File.Exists(normalizedXamlFilePath))
            {
                throw new FileNotFoundException("The requested XAML file does not exist.", normalizedXamlFilePath);
            }

            var sourceDocument = ReadSourceDocument(normalizedXamlFilePath);
            var resolvedRootType = ResolveElementType(sourceDocument.Document.Root, rootObject.GetType().Assembly)
                ?? ResolveElementType(sourceDocument.Document.Root, localAssembly: null);
            if (resolvedRootType != null && !resolvedRootType.IsAssignableFrom(rootObject.GetType()))
            {
                throw new InvalidOperationException(
                    $"The generated root object type '{rootObject.GetType().FullName}' does not match the XAML root '{resolvedRootType.FullName}'.");
            }

            var preparedLoad = PrepareLoadDocument(sourceDocument.Document, rootObject.GetType().Assembly);
            ApplyXamlToExistingRoot(
                rootObject,
                preparedLoad.Document,
                rootObject.GetType().Assembly,
                preparedLoad.EventBindings);
        }

        private static LiveXamlLoadResult Execute(
            string absoluteXamlFilePath,
            string? absoluteCodeBehindFilePath,
            bool instantiatePreview)
        {
            var diagnostics = new List<LiveXamlDiagnostic>();
            LiveXamlPreviewPlan? plan = null;

            try
            {
                plan = BuildPreviewPlan(absoluteXamlFilePath, absoluteCodeBehindFilePath, diagnostics);
                if (diagnostics.Any(item => item.Severity == LiveXamlDiagnosticSeverity.Error))
                {
                    return BuildFailureResult(plan, diagnostics);
                }

                FrameworkElement? view = null;
                if (instantiatePreview)
                {
                    view = InstantiatePreview(plan);
                }

                return new LiveXamlLoadResult
                {
                    IsSuccess = true,
                    Summary = instantiatePreview
                        ? $"LiveXAML rendered successfully from {plan.XamlFilePath}."
                        : $"LiveXAML validation passed for {plan.XamlFilePath}.",
                    XamlFilePath = plan.XamlFilePath,
                    CodeBehindFilePath = plan.CodeBehindFilePath,
                    RootClassName = plan.RootClassName,
                    RootElementTypeName = plan.RootElementTypeName,
                    View = view,
                    Diagnostics = diagnostics.ToArray()
                };
            }
            catch (Exception ex)
            {
                diagnostics.Add(new LiveXamlDiagnostic(
                    LiveXamlDiagnosticSeverity.Error,
                    ex.Message,
                    TryNormalizePath(absoluteXamlFilePath)));
                return BuildFailureResult(plan, diagnostics);
            }
        }

        private static LiveXamlLoadResult BuildFailureResult(
            LiveXamlPreviewPlan? plan,
            IReadOnlyList<LiveXamlDiagnostic> diagnostics)
        {
            return new LiveXamlLoadResult
            {
                IsSuccess = false,
                Summary = "LiveXAML could not be loaded.",
                XamlFilePath = plan?.XamlFilePath ?? string.Empty,
                CodeBehindFilePath = plan?.CodeBehindFilePath,
                RootClassName = plan?.RootClassName,
                RootElementTypeName = plan?.RootElementTypeName,
                Diagnostics = diagnostics.ToArray()
            };
        }

        private static LiveXamlPreviewPlan BuildPreviewPlan(
            string absoluteXamlFilePath,
            string? codeBehindFilePath,
            ICollection<LiveXamlDiagnostic> diagnostics)
        {
            var normalizedXamlFilePath = LiveXamlFileSupport.NormalizeAbsoluteXamlPath(absoluteXamlFilePath);
            if (!File.Exists(normalizedXamlFilePath))
            {
                diagnostics.Add(new LiveXamlDiagnostic(
                    LiveXamlDiagnosticSeverity.Error,
                    "The requested XAML file does not exist.",
                    normalizedXamlFilePath));
                throw new FileNotFoundException("The requested XAML file does not exist.", normalizedXamlFilePath);
            }

            var normalizedCodeBehindFilePath = string.IsNullOrWhiteSpace(codeBehindFilePath)
                ? LiveXamlFileSupport.ResolveSiblingCodeBehindPath(normalizedXamlFilePath)
                : Path.GetFullPath(codeBehindFilePath.Trim());
            var sourceDocument = ReadSourceDocument(normalizedXamlFilePath);
            var rootClassName = GetRootClassName(sourceDocument.Document.Root);
            var namedElements = CollectNamedElementNames(sourceDocument.Document, normalizedXamlFilePath, diagnostics);
            var rootElementType = ResolveElementType(sourceDocument.Document.Root, localAssembly: null);
            var rootElementTypeName = rootElementType?.FullName ?? sourceDocument.Document.Root?.Name.LocalName;

            var codeBehindText = TryReadCodeBehindText(normalizedCodeBehindFilePath);
            var effectiveCodeBehindText = NormalizeCodeBehindText(
                codeBehindText,
                rootClassName,
                rootElementType,
                diagnostics,
                normalizedCodeBehindFilePath ?? $"{normalizedXamlFilePath}.cs");

            Assembly? compiledAssembly = null;
            Type? compiledRootType = null;
            if (!string.IsNullOrWhiteSpace(rootClassName))
            {
                var compilationSource = BuildCompilationSource(
                    rootClassName!,
                    rootElementType,
                    effectiveCodeBehindText,
                    namedElements,
                    normalizedXamlFilePath,
                    normalizedCodeBehindFilePath);
                compiledAssembly = CompileAssembly(compilationSource, diagnostics);
                compiledRootType = compiledAssembly?.GetType(rootClassName!, throwOnError: false, ignoreCase: false);
                if (compiledAssembly != null && compiledRootType == null)
                {
                    diagnostics.Add(new LiveXamlDiagnostic(
                        LiveXamlDiagnosticSeverity.Error,
                        $"The compiled assembly did not contain the expected root type '{rootClassName}'.",
                        normalizedCodeBehindFilePath ?? normalizedXamlFilePath));
                }
            }
            else if (!string.IsNullOrWhiteSpace(codeBehindText))
            {
                diagnostics.Add(new LiveXamlDiagnostic(
                    LiveXamlDiagnosticSeverity.Error,
                    "A non-empty .xaml.cs file requires an x:Class on the XAML root element.",
                    normalizedCodeBehindFilePath));
            }

            var localAssembly = compiledAssembly;
            var resolvedRootElementType = ResolveElementType(sourceDocument.Document.Root, localAssembly) ?? rootElementType;
            rootElementTypeName = resolvedRootElementType?.FullName ?? rootElementTypeName;

            if (compiledRootType != null)
            {
                if (!typeof(FrameworkElement).IsAssignableFrom(compiledRootType))
                {
                    diagnostics.Add(new LiveXamlDiagnostic(
                        LiveXamlDiagnosticSeverity.Error,
                        "The x:Class root type must inherit from FrameworkElement.",
                        normalizedCodeBehindFilePath ?? normalizedXamlFilePath));
                }

                if (typeof(Window).IsAssignableFrom(compiledRootType))
                {
                    diagnostics.Add(new LiveXamlDiagnostic(
                        LiveXamlDiagnosticSeverity.Error,
                        "Window roots are not embeddable in the chat preview. Use UserControl or another embeddable FrameworkElement root.",
                        normalizedXamlFilePath));
                }

                if (resolvedRootElementType != null && !resolvedRootElementType.IsAssignableFrom(compiledRootType))
                {
                    diagnostics.Add(new LiveXamlDiagnostic(
                        LiveXamlDiagnosticSeverity.Error,
                        $"The x:Class root '{compiledRootType.FullName}' must inherit from the declared XAML root '{resolvedRootElementType.FullName}'.",
                        normalizedXamlFilePath));
                }
            }
            else if (resolvedRootElementType != null)
            {
                if (!typeof(FrameworkElement).IsAssignableFrom(resolvedRootElementType))
                {
                    diagnostics.Add(new LiveXamlDiagnostic(
                        LiveXamlDiagnosticSeverity.Error,
                        "The XAML root must be a FrameworkElement so it can be embedded in the chat preview.",
                        normalizedXamlFilePath));
                }

                if (typeof(Window).IsAssignableFrom(resolvedRootElementType))
                {
                    diagnostics.Add(new LiveXamlDiagnostic(
                        LiveXamlDiagnosticSeverity.Error,
                        "Window roots are not embeddable in the chat preview. Use UserControl or another embeddable FrameworkElement root.",
                        normalizedXamlFilePath));
                }
            }

            var preparedLoad = PrepareLoadDocument(sourceDocument.Document, localAssembly);
            ValidateLoadSchema(
                preparedLoad.Document,
                normalizedXamlFilePath,
                localAssembly,
                diagnostics);

            return new LiveXamlPreviewPlan(
                normalizedXamlFilePath,
                normalizedCodeBehindFilePath,
                rootClassName,
                rootElementTypeName,
                compiledAssembly,
                compiledRootType,
                resolvedRootElementType,
                preparedLoad);
        }

        private static FrameworkElement InstantiatePreview(LiveXamlPreviewPlan plan)
        {
            if (plan.CompiledRootType != null)
            {
                object? rootObject;
                try
                {
                    rootObject = Activator.CreateInstance(plan.CompiledRootType);
                }
                catch (MissingMethodException)
                {
                    throw new InvalidOperationException(
                        $"The root x:Class '{plan.CompiledRootType.FullName}' needs a public parameterless constructor.");
                }

                if (rootObject == null)
                {
                    throw new InvalidOperationException(
                        $"The root x:Class '{plan.CompiledRootType.FullName}' could not be instantiated.");
                }

                EnsureGeneratedComponentInitialized(rootObject);
                return rootObject as FrameworkElement
                    ?? throw new InvalidOperationException("The instantiated x:Class root is not a FrameworkElement.");
            }

            var previewRoot = CreateRootObject(plan.PreparedLoad.Document, plan.CompiledAssembly);
            return previewRoot as FrameworkElement
                ?? throw new InvalidOperationException("The XAML root is not a FrameworkElement.");
        }

        private static void EnsureGeneratedComponentInitialized(object rootObject)
        {
            var rootType = rootObject.GetType();
            var initializationField = rootType.GetField(
                GeneratedInitializationFlagFieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            var isInitialized = initializationField?.GetValue(rootObject) is bool flag && flag;
            if (isInitialized)
            {
                return;
            }

            var initializeComponentMethod = rootType.GetMethod(
                "InitializeComponent",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                binder: null,
                types: Type.EmptyTypes,
                modifiers: null);
            initializeComponentMethod?.Invoke(rootObject, null);
        }

        private static LiveXamlSourceDocument ReadSourceDocument(string xamlFilePath)
        {
            try
            {
                using var stream = File.OpenRead(xamlFilePath);
                var document = XDocument.Load(stream, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
                return new LiveXamlSourceDocument(xamlFilePath, document);
            }
            catch (XmlException ex)
            {
                throw new InvalidOperationException(
                    $"The XAML is not well-formed XML: {ex.Message}");
            }
        }

        private static string? GetRootClassName(XElement? rootElement)
        {
            return rootElement?
                .Attributes()
                .FirstOrDefault(attribute =>
                    string.Equals(attribute.Name.NamespaceName, XamlNamespace, StringComparison.Ordinal) &&
                    string.Equals(attribute.Name.LocalName, "Class", StringComparison.Ordinal))
                ?.Value
                ?.Trim();
        }

        private static IReadOnlyList<string> CollectNamedElementNames(
            XDocument document,
            string xamlFilePath,
            ICollection<LiveXamlDiagnostic> diagnostics)
        {
            var results = new List<string>();
            var usedNames = new HashSet<string>(StringComparer.Ordinal);

            foreach (var element in document.Root?.DescendantsAndSelf() ?? Array.Empty<XElement>())
            {
                var nameAttribute = element.Attributes().FirstOrDefault(attribute =>
                    string.Equals(attribute.Name.NamespaceName, XamlNamespace, StringComparison.Ordinal) &&
                    string.Equals(attribute.Name.LocalName, "Name", StringComparison.Ordinal))
                    ?? element.Attributes().FirstOrDefault(attribute =>
                        string.IsNullOrEmpty(attribute.Name.NamespaceName) &&
                        string.Equals(attribute.Name.LocalName, "Name", StringComparison.Ordinal));

                var rawName = nameAttribute?.Value?.Trim();
                if (string.IsNullOrWhiteSpace(rawName))
                {
                    continue;
                }

                if (!SyntaxFacts.IsValidIdentifier(rawName))
                {
                    diagnostics.Add(new LiveXamlDiagnostic(
                        LiveXamlDiagnosticSeverity.Error,
                        $"The named element '{rawName}' is not a valid C# identifier, so it cannot behave like a normal WPF named field.",
                        xamlFilePath,
                        GetLineNumber(nameAttribute),
                        GetColumnNumber(nameAttribute)));
                    continue;
                }

                if (!usedNames.Add(rawName))
                {
                    diagnostics.Add(new LiveXamlDiagnostic(
                        LiveXamlDiagnosticSeverity.Error,
                        $"The name '{rawName}' is declared more than once in the same LiveXAML document.",
                        xamlFilePath,
                        GetLineNumber(nameAttribute),
                        GetColumnNumber(nameAttribute)));
                    continue;
                }

                results.Add(rawName);
            }

            return results;
        }

        private static string NormalizeCodeBehindText(
            string? codeBehindText,
            string? rootClassName,
            Type? rootElementType,
            ICollection<LiveXamlDiagnostic> diagnostics,
            string codeBehindFilePath)
        {
            if (string.IsNullOrWhiteSpace(rootClassName))
            {
                return string.IsNullOrWhiteSpace(codeBehindText)
                    ? string.Empty
                    : codeBehindText!;
            }

            if (!string.IsNullOrWhiteSpace(codeBehindText))
            {
                return codeBehindText!;
            }

            if (rootElementType == null)
            {
                diagnostics.Add(new LiveXamlDiagnostic(
                    LiveXamlDiagnosticSeverity.Error,
                    "The XAML root type could not be resolved, so the host could not synthesize a default x:Class stub.",
                    codeBehindFilePath));
                return string.Empty;
            }

            SplitQualifiedTypeName(rootClassName, out var namespaceName, out var typeName);
            var builder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                builder.AppendLine($"namespace {namespaceName}");
                builder.AppendLine("{");
            }

            builder.AppendLine($"    public partial class {typeName} : global::{rootElementType.FullName}");
            builder.AppendLine("    {");
            builder.AppendLine($"        public {typeName}()");
            builder.AppendLine("        {");
            builder.AppendLine("            InitializeComponent();");
            builder.AppendLine("        }");
            builder.AppendLine("    }");

            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                builder.AppendLine("}");
            }

            return builder.ToString();
        }

        private static LiveXamlCompilationSource BuildCompilationSource(
            string rootClassName,
            Type? rootElementType,
            string userCodeBehindText,
            IReadOnlyList<string> namedElements,
            string xamlFilePath,
            string? codeBehindFilePath)
        {
            var generatedPartialSource = BuildGeneratedPartialSource(rootClassName, namedElements, xamlFilePath);
            if (string.IsNullOrWhiteSpace(userCodeBehindText) && rootElementType == null)
            {
                throw new InvalidOperationException("The XAML root type could not be resolved for code-behind compilation.");
            }

            return new LiveXamlCompilationSource(
                string.IsNullOrWhiteSpace(userCodeBehindText)
                    ? null
                    : new LiveXamlSourceText(codeBehindFilePath ?? "LiveXaml.xaml.cs", userCodeBehindText),
                new LiveXamlSourceText("LiveXaml.Generated.g.cs", generatedPartialSource));
        }

        private static string BuildGeneratedPartialSource(
            string rootClassName,
            IReadOnlyList<string> namedElements,
            string xamlFilePath)
        {
            SplitQualifiedTypeName(rootClassName, out var namespaceName, out var typeName);
            var builder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                builder.AppendLine($"namespace {namespaceName}");
                builder.AppendLine("{");
            }

            builder.AppendLine($"    public partial class {typeName}");
            builder.AppendLine("    {");
            builder.AppendLine($"        private bool {GeneratedInitializationFlagFieldName};");
            builder.AppendLine();
            foreach (var namedElement in namedElements)
            {
                builder.AppendLine("        [global::System.CodeDom.Compiler.GeneratedCode(\"Skyweaver.LiveXaml\", \"1.0\")]");
                builder.AppendLine($"        public dynamic {namedElement};");
            }

            if (namedElements.Count > 0)
            {
                builder.AppendLine();
            }

            builder.AppendLine("        public void InitializeComponent()");
            builder.AppendLine("        {");
            builder.AppendLine($"            if ({GeneratedInitializationFlagFieldName})");
            builder.AppendLine("            {");
            builder.AppendLine("                return;");
            builder.AppendLine("            }");
            builder.AppendLine();
            builder.AppendLine($"            {GeneratedInitializationFlagFieldName} = true;");
            builder.AppendLine($"            global::Skyweaver.Services.LiveXaml.LiveXamlRuntime.InitializeGeneratedComponent(this, {ToCSharpStringLiteral(xamlFilePath)});");
            builder.AppendLine("        }");
            builder.AppendLine("    }");

            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                builder.AppendLine("}");
            }

            return builder.ToString();
        }

        private static Assembly? CompileAssembly(
            LiveXamlCompilationSource compilationSource,
            ICollection<LiveXamlDiagnostic> diagnostics)
        {
            var syntaxTrees = new List<SyntaxTree>();
            if (compilationSource.UserCode != null)
            {
                syntaxTrees.Add(CSharpSyntaxTree.ParseText(
                    compilationSource.UserCode.Text,
                    new CSharpParseOptions(LanguageVersion.Latest),
                    path: compilationSource.UserCode.Path));
            }

            syntaxTrees.Add(CSharpSyntaxTree.ParseText(
                compilationSource.GeneratedCode.Text,
                new CSharpParseOptions(LanguageVersion.Latest),
                path: compilationSource.GeneratedCode.Path));

            var compilation = CSharpCompilation.Create(
                assemblyName: $"Skyweaver.LiveXaml.{Guid.NewGuid():N}",
                syntaxTrees: syntaxTrees,
                references: BuildMetadataReferences(),
                options: new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Debug,
                    nullableContextOptions: NullableContextOptions.Enable));

            using var assemblyStream = new MemoryStream();
            EmitResult emitResult = compilation.Emit(assemblyStream);
            foreach (var diagnostic in emitResult.Diagnostics
                         .Where(item => item.Severity is DiagnosticSeverity.Error or DiagnosticSeverity.Warning)
                         .OrderBy(item => item.Location.GetLineSpan().Path, StringComparer.OrdinalIgnoreCase)
                         .ThenBy(item => item.Location.GetLineSpan().StartLinePosition.Line))
            {
                var lineSpan = diagnostic.Location.GetLineSpan();
                diagnostics.Add(new LiveXamlDiagnostic(
                    diagnostic.Severity == DiagnosticSeverity.Warning
                        ? LiveXamlDiagnosticSeverity.Warning
                        : LiveXamlDiagnosticSeverity.Error,
                    diagnostic.GetMessage(),
                    string.IsNullOrWhiteSpace(lineSpan.Path) ? null : lineSpan.Path,
                    diagnostic.Location == Location.None ? null : lineSpan.StartLinePosition.Line + 1,
                    diagnostic.Location == Location.None ? null : lineSpan.StartLinePosition.Character + 1));
            }

            if (!emitResult.Success)
            {
                return null;
            }

            assemblyStream.Position = 0;
            return AssemblyLoadContext.Default.LoadFromStream(assemblyStream);
        }

        private static IReadOnlyList<MetadataReference> BuildMetadataReferences()
        {
            var referencePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var trustedPlatformAssemblies = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
            if (!string.IsNullOrWhiteSpace(trustedPlatformAssemblies))
            {
                foreach (var referencePath in trustedPlatformAssemblies.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
                {
                    referencePaths.Add(referencePath);
                }
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (assembly.IsDynamic || string.IsNullOrWhiteSpace(assembly.Location))
                    {
                        continue;
                    }

                    referencePaths.Add(assembly.Location);
                }
                catch
                {
                    // Some framework-backed assemblies may not expose a stable Location.
                }
            }

            return referencePaths
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .Select(path => MetadataReference.CreateFromFile(path))
                .ToArray();
        }

        private static LiveXamlPreparedLoadDocument PrepareLoadDocument(XDocument originalDocument, Assembly? localAssembly)
        {
            var clone = XDocument.Parse(
                originalDocument.ToString(SaveOptions.DisableFormatting),
                LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
            var rootElement = clone.Root ?? throw new InvalidOperationException("The XAML document has no root element.");

            rootElement.Attributes()
                .Where(attribute =>
                    string.Equals(attribute.Name.NamespaceName, XamlNamespace, StringComparison.Ordinal) &&
                    string.Equals(attribute.Name.LocalName, "Class", StringComparison.Ordinal))
                .Remove();

            if (localAssembly != null)
            {
                ApplyLocalAssemblyToClrNamespaceAliases(rootElement, localAssembly);
            }

            var eventBindings = ExtractAndStripEventBindings(rootElement, localAssembly);
            return new LiveXamlPreparedLoadDocument(clone, eventBindings);
        }

        private static void ApplyLocalAssemblyToClrNamespaceAliases(XElement rootElement, Assembly localAssembly)
        {
            var assemblyName = localAssembly.GetName().Name;
            if (string.IsNullOrWhiteSpace(assemblyName))
            {
                return;
            }

            foreach (var attribute in rootElement.Attributes().Where(attribute => attribute.IsNamespaceDeclaration))
            {
                var namespaceValue = attribute.Value?.Trim() ?? string.Empty;
                if (!namespaceValue.StartsWith("clr-namespace:", StringComparison.Ordinal) ||
                    namespaceValue.Contains(";assembly=", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                attribute.Value = $"{namespaceValue};assembly={assemblyName}";
            }
        }

        private static IReadOnlyList<LiveXamlEventBinding> ExtractAndStripEventBindings(XElement rootElement, Assembly? localAssembly)
        {
            var generatedNameIndex = 0;
            var eventBindings = new List<LiveXamlEventBinding>();

            foreach (var element in rootElement.DescendantsAndSelf())
            {
                var elementType = ResolveElementType(element, localAssembly);
                if (elementType == null)
                {
                    continue;
                }

                foreach (var attribute in element.Attributes().ToArray())
                {
                    if (attribute.IsNamespaceDeclaration ||
                        string.Equals(attribute.Name.NamespaceName, XamlNamespace, StringComparison.Ordinal) ||
                        string.Equals(attribute.Name.NamespaceName, MarkupCompatibilityNamespace, StringComparison.Ordinal) ||
                        attribute.Name.LocalName.Contains('.', StringComparison.Ordinal))
                    {
                        continue;
                    }

                    var eventInfo = elementType.GetEvent(attribute.Name.LocalName, BindingFlags.Instance | BindingFlags.Public);
                    if (eventInfo == null)
                    {
                        continue;
                    }

                    var targetName = GetDeclaredName(element);
                    if (string.IsNullOrWhiteSpace(targetName))
                    {
                        generatedNameIndex++;
                        targetName = $"__skyweaverAutoEventTarget{generatedNameIndex}";
                        SetXamlName(element, targetName);
                    }

                    eventBindings.Add(new LiveXamlEventBinding(
                        targetName!,
                        eventInfo.Name,
                        attribute.Value?.Trim() ?? string.Empty));
                    attribute.Remove();
                }
            }

            return eventBindings;
        }

        private static void ValidateLoadSchema(
            XDocument preparedLoadDocument,
            string xamlFilePath,
            Assembly? localAssembly,
            ICollection<LiveXamlDiagnostic> diagnostics)
        {
            try
            {
                using var stringReader = new StringReader(preparedLoadDocument.ToString(SaveOptions.DisableFormatting));
                using var xmlReader = XmlReader.Create(stringReader, new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    IgnoreComments = false,
                    IgnoreWhitespace = false
                });
                using var xamlReader = CreateXamlReader(xmlReader, localAssembly, provideLineInfo: true);
                while (xamlReader.Read())
                {
                }
            }
            catch (XamlException ex)
            {
                diagnostics.Add(new LiveXamlDiagnostic(
                    LiveXamlDiagnosticSeverity.Error,
                    ex.Message,
                    xamlFilePath,
                    ex.LineNumber > 0 ? ex.LineNumber : null,
                    ex.LinePosition > 0 ? ex.LinePosition : null));
            }
            catch (XmlException ex)
            {
                diagnostics.Add(new LiveXamlDiagnostic(
                    LiveXamlDiagnosticSeverity.Error,
                    ex.Message,
                    xamlFilePath,
                    ex.LineNumber > 0 ? ex.LineNumber : null,
                    ex.LinePosition > 0 ? ex.LinePosition : null));
            }
        }

        private static object CreateRootObject(XDocument preparedLoadDocument, Assembly? localAssembly)
        {
            using var stringReader = new StringReader(preparedLoadDocument.ToString(SaveOptions.DisableFormatting));
            using var xmlReader = XmlReader.Create(stringReader, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                IgnoreComments = false,
                IgnoreWhitespace = false
            });
            using var xamlReader = CreateXamlReader(xmlReader, localAssembly, provideLineInfo: false);
            var writerSettings = localAssembly == null
                ? new XamlObjectWriterSettings()
                : new XamlObjectWriterSettings
                {
                    AccessLevel = XamlAccessLevel.AssemblyAccessTo(localAssembly)
                };
            using var xamlWriter = new XamlObjectWriter(xamlReader.SchemaContext, writerSettings);
            while (xamlReader.Read())
            {
                xamlWriter.WriteNode(xamlReader);
            }

            return xamlWriter.Result
                ?? throw new InvalidOperationException("The LiveXAML document did not produce a root object.");
        }

        private static void ApplyXamlToExistingRoot(
            object rootObject,
            XDocument preparedLoadDocument,
            Assembly? localAssembly,
            IReadOnlyList<LiveXamlEventBinding> eventBindings)
        {
            using var stringReader = new StringReader(preparedLoadDocument.ToString(SaveOptions.DisableFormatting));
            using var xmlReader = XmlReader.Create(stringReader, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                IgnoreComments = false,
                IgnoreWhitespace = false
            });
            using var xamlReader = CreateXamlReader(xmlReader, localAssembly, provideLineInfo: false);
            var writerSettings = new XamlObjectWriterSettings
            {
                RootObjectInstance = rootObject,
                AccessLevel = XamlAccessLevel.PrivateAccessTo(rootObject.GetType())
            };
            using var xamlWriter = new XamlObjectWriter(xamlReader.SchemaContext, writerSettings);
            while (xamlReader.Read())
            {
                xamlWriter.WriteNode(xamlReader);
            }

            AssignNamedElementFields(rootObject);
            AttachEventHandlers(rootObject, eventBindings);
        }

        private static XamlXmlReader CreateXamlReader(XmlReader xmlReader, Assembly? localAssembly, bool provideLineInfo)
        {
            return new XamlXmlReader(
                xmlReader,
                new XamlSchemaContext(),
                new XamlXmlReaderSettings
                {
                    LocalAssembly = localAssembly,
                    ProvideLineInfo = provideLineInfo
                });
        }

        private static void AssignNamedElementFields(object rootObject)
        {
            if (rootObject is not FrameworkElement frameworkElement)
            {
                return;
            }

            foreach (var field in rootObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!field.GetCustomAttributes()
                        .Any(attribute => string.Equals(attribute.GetType().FullName, GeneratedCodeAttributeFullName, StringComparison.Ordinal)))
                {
                    continue;
                }

                var value = ResolveNamedObject(frameworkElement, field.Name);
                if (value != null)
                {
                    field.SetValue(rootObject, value);
                }
            }
        }

        private static void AttachEventHandlers(object rootObject, IReadOnlyList<LiveXamlEventBinding> eventBindings)
        {
            if (rootObject is not FrameworkElement frameworkElement)
            {
                return;
            }

            foreach (var eventBinding in eventBindings)
            {
                var targetObject = ResolveNamedObject(frameworkElement, eventBinding.TargetName)
                    ?? (string.Equals(frameworkElement.Name, eventBinding.TargetName, StringComparison.Ordinal) ? frameworkElement : null);
                if (targetObject == null)
                {
                    continue;
                }

                var eventInfo = targetObject.GetType().GetEvent(eventBinding.EventName, BindingFlags.Instance | BindingFlags.Public);
                if (eventInfo == null)
                {
                    throw new InvalidOperationException(
                        $"The event '{eventBinding.EventName}' could not be found on '{targetObject.GetType().FullName}'.");
                }

                var eventHandlerDelegate = TryCreateEventHandlerDelegate(rootObject, eventInfo.EventHandlerType, eventBinding.HandlerName);
                if (eventHandlerDelegate != null)
                {
                    eventInfo.AddEventHandler(targetObject, eventHandlerDelegate);
                }
            }
        }

        private static Delegate? TryCreateEventHandlerDelegate(object rootObject, Type? eventHandlerType, string handlerName)
        {
            if (eventHandlerType == null)
            {
                return null;
            }

            var methods = rootObject.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(method => string.Equals(method.Name, handlerName, StringComparison.Ordinal))
                .ToArray();

            foreach (var method in methods)
            {
                var createdDelegate = Delegate.CreateDelegate(
                    eventHandlerType,
                    rootObject,
                    method,
                    throwOnBindFailure: false);
                if (createdDelegate != null)
                {
                    return createdDelegate;
                }
            }

            throw new InvalidOperationException(
                $"The event handler method '{handlerName}' could not be bound to the requested event delegate type '{eventHandlerType.FullName}'.");
        }

        private static object? ResolveNamedObject(FrameworkElement root, string name)
        {
            if (string.Equals(root.Name, name, StringComparison.Ordinal))
            {
                return root;
            }

            return root.FindName(name);
        }

        private static Type? ResolveElementType(XElement? element, Assembly? localAssembly)
        {
            if (element == null)
            {
                return null;
            }

            var xamlNamespace = element.Name.NamespaceName;
            var localName = element.Name.LocalName;
            if (xamlNamespace.StartsWith("clr-namespace:", StringComparison.Ordinal))
            {
                ParseClrNamespace(xamlNamespace, out var clrNamespace, out var assemblyName);
                var candidateAssemblies = string.IsNullOrWhiteSpace(assemblyName)
                    ? EnumerateLocalCandidateAssemblies(localAssembly)
                    : EnumerateAllCandidateAssemblies()
                        .Where(assembly => string.Equals(assembly.GetName().Name, assemblyName, StringComparison.OrdinalIgnoreCase));
                foreach (var assembly in candidateAssemblies)
                {
                    var type = assembly.GetType($"{clrNamespace}.{localName}", throwOnError: false, ignoreCase: false);
                    if (type != null)
                    {
                        return type;
                    }
                }

                return null;
            }

            foreach (var assembly in EnumerateAllCandidateAssemblies(localAssembly))
            {
                foreach (var xmlnsDefinition in assembly.GetCustomAttributes<XmlnsDefinitionAttribute>())
                {
                    if (!string.Equals(xmlnsDefinition.XmlNamespace, xamlNamespace, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    var type = assembly.GetType($"{xmlnsDefinition.ClrNamespace}.{localName}", throwOnError: false, ignoreCase: false);
                    if (type != null)
                    {
                        return type;
                    }
                }
            }

            return null;
        }

        private static IEnumerable<Assembly> EnumerateAllCandidateAssemblies(Assembly? extraAssembly = null)
        {
            if (extraAssembly != null)
            {
                yield return extraAssembly;
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()
                         .Where(assembly => !assembly.IsDynamic)
                         .Distinct())
            {
                yield return assembly;
            }
        }

        private static IEnumerable<Assembly> EnumerateLocalCandidateAssemblies(Assembly? localAssembly)
        {
            if (localAssembly != null)
            {
                yield return localAssembly;
            }

            foreach (var assembly in EnumerateAllCandidateAssemblies())
            {
                yield return assembly;
            }
        }

        private static void ParseClrNamespace(string xamlNamespace, out string clrNamespace, out string? assemblyName)
        {
            var rawBody = xamlNamespace["clr-namespace:".Length..];
            var segments = rawBody.Split(';', StringSplitOptions.RemoveEmptyEntries);
            clrNamespace = segments[0].Trim();
            assemblyName = null;

            foreach (var segment in segments.Skip(1))
            {
                var trimmedSegment = segment.Trim();
                if (trimmedSegment.StartsWith("assembly=", StringComparison.OrdinalIgnoreCase))
                {
                    assemblyName = trimmedSegment["assembly=".Length..].Trim();
                    break;
                }
            }
        }

        private static string? TryReadCodeBehindText(string? codeBehindFilePath)
        {
            if (string.IsNullOrWhiteSpace(codeBehindFilePath) || !File.Exists(codeBehindFilePath))
            {
                return null;
            }

            return File.ReadAllText(codeBehindFilePath);
        }

        private static string? GetDeclaredName(XElement element)
        {
            return element.Attributes().FirstOrDefault(attribute =>
                       string.Equals(attribute.Name.NamespaceName, XamlNamespace, StringComparison.Ordinal) &&
                       string.Equals(attribute.Name.LocalName, "Name", StringComparison.Ordinal))
                       ?.Value?.Trim()
                   ?? element.Attributes().FirstOrDefault(attribute =>
                       string.IsNullOrEmpty(attribute.Name.NamespaceName) &&
                       string.Equals(attribute.Name.LocalName, "Name", StringComparison.Ordinal))
                       ?.Value?.Trim();
        }

        private static void SetXamlName(XElement element, string value)
        {
            element.SetAttributeValue(XName.Get("Name", XamlNamespace), value);
        }

        private static void SplitQualifiedTypeName(string qualifiedTypeName, out string? namespaceName, out string typeName)
        {
            var lastDotIndex = qualifiedTypeName.LastIndexOf('.');
            if (lastDotIndex < 0)
            {
                namespaceName = null;
                typeName = qualifiedTypeName.Trim();
                return;
            }

            namespaceName = qualifiedTypeName[..lastDotIndex].Trim();
            typeName = qualifiedTypeName[(lastDotIndex + 1)..].Trim();
        }

        private static string ToCSharpStringLiteral(string value)
        {
            return SymbolDisplay.FormatLiteral(value ?? string.Empty, quote: true);
        }

        private static int? GetLineNumber(XObject? value)
        {
            return value is IXmlLineInfo lineInfo && lineInfo.HasLineInfo() && lineInfo.LineNumber > 0
                ? lineInfo.LineNumber
                : null;
        }

        private static int? GetColumnNumber(XObject? value)
        {
            return value is IXmlLineInfo lineInfo && lineInfo.HasLineInfo() && lineInfo.LinePosition > 0
                ? lineInfo.LinePosition
                : null;
        }

        private static string? TryNormalizePath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            try
            {
                return Path.GetFullPath(path);
            }
            catch
            {
                return path;
            }
        }

        private sealed record LiveXamlSourceDocument(
            string XamlFilePath,
            XDocument Document);

        private sealed record LiveXamlSourceText(
            string Path,
            string Text);

        private sealed record LiveXamlCompilationSource(
            LiveXamlSourceText? UserCode,
            LiveXamlSourceText GeneratedCode);

        private sealed record LiveXamlPreparedLoadDocument(
            XDocument Document,
            IReadOnlyList<LiveXamlEventBinding> EventBindings);

        private sealed record LiveXamlEventBinding(
            string TargetName,
            string EventName,
            string HandlerName);

        private sealed record LiveXamlPreviewPlan(
            string XamlFilePath,
            string? CodeBehindFilePath,
            string? RootClassName,
            string? RootElementTypeName,
            Assembly? CompiledAssembly,
            Type? CompiledRootType,
            Type? RootFrameworkElementType,
            LiveXamlPreparedLoadDocument PreparedLoad);
    }
}
