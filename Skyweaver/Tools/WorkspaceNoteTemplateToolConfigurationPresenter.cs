using System.Windows;
using System.Xml.Linq;
using Skyweaver.Services.SkyweaverTools;

namespace Skyweaver.Tools
{
    public sealed class WorkspaceNoteTemplateToolConfigurationPresenter : SkyweaverToolConfigurationPresenter
    {
        private readonly WorkspaceNoteTemplateToolConfigurationViewModel _viewModel;
        private readonly WorkspaceNoteTemplateToolConfigurationView _view;

        public WorkspaceNoteTemplateToolConfigurationPresenter(SkyweaverToolConfigurationEditorContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var settings = WorkspaceNoteTemplateToolSettings.FromConfiguration(context.InitialConfiguration);
            _viewModel = new WorkspaceNoteTemplateToolConfigurationViewModel(settings, RaiseConfigurationChanged);
            _view = new WorkspaceNoteTemplateToolConfigurationView
            {
                DataContext = _viewModel
            };
        }

        public override FrameworkElement View => _view;

        public override bool TryCaptureConfiguration(out XElement? configuration, out string? errorMessage)
        {
            try
            {
                configuration = _viewModel.ToSettings().ToXElement();
                errorMessage = null;
                return true;
            }
            catch (Exception ex)
            {
                configuration = null;
                errorMessage = $"WorkspaceNoteTemplate 配置无效：{ex.Message}";
                return false;
            }
        }
    }
}
