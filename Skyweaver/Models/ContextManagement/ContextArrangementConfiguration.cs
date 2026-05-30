using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Models.ContextManagement
{
    public sealed class ContextArrangementConfiguration : ObservableObject
    {
        private bool _optimizeToolCallPrompt;
        private bool _toolCallIdTable;

        public bool OptimizeToolCallPrompt
        {
            get => _optimizeToolCallPrompt;
            set => SetProperty(ref _optimizeToolCallPrompt, value);
        }

        public bool ToolCallIdTable
        {
            get => _toolCallIdTable;
            set => SetProperty(ref _toolCallIdTable, value);
        }
    }
}
