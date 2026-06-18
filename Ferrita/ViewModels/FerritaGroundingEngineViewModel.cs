using System;
using System.Collections.ObjectModel;
using Ferrita.Infrastructure.Mvvm;

namespace Ferrita.ViewModels
{
    /// <summary>
    /// 未完成的 Grounding 进度项
    /// </summary>
    public class PendingGroundingItem : ObservableObject
    {
        private string _taskName = string.Empty;
        private string _statusText = string.Empty;
        private double _progress;
        private string _etaText = string.Empty;

        /// <summary>
        /// 任务/消息提示名称
        /// </summary>
        public string TaskName
        {
            get => _taskName;
            set => SetProperty(ref _taskName, value);
        }

        /// <summary>
        /// 当前状态描述
        /// </summary>
        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        /// <summary>
        /// 进度值 (0.0 - 1.0)
        /// </summary>
        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        /// <summary>
        /// 预计剩余时间描述
        /// </summary>
        public string EtaText
        {
            get => _etaText;
            set => SetProperty(ref _etaText, value);
        }
    }

    /// <summary>
    /// Grounding Engine 窗口的 ViewModel
    /// </summary>
    public class FerritaGroundingEngineViewModel : ObservableObject
    {
        private ObservableCollection<GroundingPairViewModel> _groundingPairs = new();
        private ObservableCollection<PendingGroundingItem> _pendingGroundings = new();
        private GroundingPairViewModel? _selectedPair;

        /// <summary>
        /// 消息事实配对集合（已完成的 Grounding）
        /// </summary>
        public ObservableCollection<GroundingPairViewModel> GroundingPairs
        {
            get => _groundingPairs;
            set => SetProperty(ref _groundingPairs, value);
        }

        /// <summary>
        /// 未完成的 Grounding 进度队列
        /// </summary>
        public ObservableCollection<PendingGroundingItem> PendingGroundings
        {
            get => _pendingGroundings;
            set => SetProperty(ref _pendingGroundings, value);
        }

        /// <summary>
        /// 当前选中项
        /// </summary>
        public GroundingPairViewModel? SelectedPair
        {
            get => _selectedPair;
            set => SetProperty(ref _selectedPair, value);
        }

        public FerritaGroundingEngineViewModel()
        {
            // 加载初始模拟数据以供界面预览
            LoadDefaultData();
            LoadPendingData();
        }

        private void LoadPendingData()
        {
            PendingGroundings.Add(new PendingGroundingItem
            {
                TaskName = "智能体回复: 分布式传输协议选型建议",
                StatusText = "正在从 Google Search 提取网页核心观点...",
                Progress = 0.45,
                EtaText = "剩余约 4.5s"
            });

            PendingGroundings.Add(new PendingGroundingItem
            {
                TaskName = "系统集成: 对接 Tunnel-Next 适配器问题核实",
                StatusText = "正在匹配本地知识库 (s:\\ProjectFerrita\\docs)...",
                Progress = 0.80,
                EtaText = "剩余约 1.2s"
            });

            PendingGroundings.Add(new PendingGroundingItem
            {
                TaskName = "代码规范: C# 12 集合表达式性能参数比对",
                StatusText = "正在调用逻辑引擎计算事实判定可信度...",
                Progress = 0.15,
                EtaText = "剩余约 8.0s"
            });

            PendingGroundings.Add(new PendingGroundingItem
            {
                TaskName = "环境校验: Windows 平台文件锁排查进度",
                StatusText = "任务队列排队中 (Waiting in queue)...",
                Progress = 0.00,
                EtaText = "等待中"
            });
        }

        private void LoadDefaultData()
        {
            // 绿色 - 证实
            GroundingPairs.Add(new GroundingPairViewModel
            {
                ChatMessage = "Ferrita 是一款专门为分布式协作 and 智能辅助设计的现代开发平台，支持实时事实比对引擎。",
                FactMessage = "证实：Ferrita 产品规格书（Sec 1.2）中指明其核心子系统为分布式语义比对器与实时辅助窗口。",
                Severity = FactSeverity.Verified,
                Confidence = 0.98,
                Source = "Product-Spec-v2",
                Timestamp = "21:28:01"
            });

            // 红色 - 冲突
            GroundingPairs.Add(new GroundingPairViewModel
            {
                ChatMessage = "数据引擎的默认运行端口被设置为 8080，由于本地开发占用，我们需要手动将它重定向到 9090。",
                FactMessage = "冲突：端口配置规范（config.json#L45）中默认端口实为 5005，而非 8080，请核实原始配置。",
                Severity = FactSeverity.Contradicted,
                Confidence = 1.00,
                Source = "System-Config-Doc",
                Timestamp = "21:28:15"
            });

            // 黄色 - 未证实/可疑
            GroundingPairs.Add(new GroundingPairViewModel
            {
                ChatMessage = "下一代 Tunnel 的最高吞吐量据称可以达到 50k ops/sec，这有助于支撑超大规模智能体的并发通信。",
                FactMessage = "未证实：实验室报告暂未对吞吐上限做最终评估，当前的基准测试记录为 32k ops/sec。",
                Severity = FactSeverity.Unverified,
                Confidence = 0.74,
                Source = "Lab-Benchmark-Draft",
                Timestamp = "21:28:22"
            });
        }
    }
}
