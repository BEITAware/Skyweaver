using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Ferrita.Models.ChatSession;
using Ferrita.Services.ChatSession;

namespace Ferrita.Windows
{
    public sealed class CreateChatSessionUniversalDialog : UniversalNewDialog
    {
        private readonly TextBox _sessionNameTextBox;
        private readonly ComboBox _flowComboBox;
        private readonly UniversalNewDialogOption _sessionNameOption;
        private readonly UniversalNewDialogOption _sessionFlowOption;

        public CreateChatSessionUniversalDialog(
            IReadOnlyList<ChatSessionFlowBindingOption>? sessionFlowOptions = null,
            string? initialSessionName = null)
        {
            MainTitle = "创建会话";
            MainDescription = "创建一个新的Ferrita会话。创建会话后，Ferrita可帮助你执行代理任务或陪伴你聊天。";
            SetMainIcon("/Ferrita;component/Resources/FerritaLogo.png");

            _sessionNameTextBox = CreateSessionNameTextBox(initialSessionName);
            _flowComboBox = CreateSessionFlowComboBox(sessionFlowOptions ?? Array.Empty<ChatSessionFlowBindingOption>());

            _sessionNameOption = AddSettingOption(
                "会话名称",
                "当前会话的名称",
                "/Ferrita;component/Resources/EditDocument.png",
                _sessionNameTextBox,
                _ => Dispatcher.BeginInvoke(() =>
                {
                    _sessionNameTextBox.Focus();
                    _sessionNameTextBox.SelectAll();
                }, DispatcherPriority.Input));

            _sessionFlowOption = AddSettingOption(
                "会话流",
                "当前会话所使用的会话流",
                "/Ferrita;component/Resources/NewNodeGraph.png",
                _flowComboBox,
                _ => Dispatcher.BeginInvoke(() => _flowComboBox.Focus(), DispatcherPriority.Input));

            AddTriggerOption(
                "创建会话",
                "创建相关会话并关闭对话框",
                "/Ferrita;component/Resources/CheckMark.png",
                _ =>
                {
                    TryCreateSession();
                    return true;
                });

            _sessionNameTextBox.TextChanged += (_, _) => RefreshSessionNameContent();
            _flowComboBox.SelectionChanged += (_, _) => RefreshSessionFlowContent();

            RefreshSessionNameContent();
            RefreshSessionFlowContent();
        }

        public string SessionName => _sessionNameTextBox.Text.Trim();

        public ChatSessionFlowBinding? SelectedFlowBinding =>
            (_flowComboBox.SelectedItem as ChatSessionFlowBindingOption)?.ToBinding();

        private static TextBox CreateSessionNameTextBox(string? initialSessionName)
        {
            return new TextBox
            {
                MinWidth = 260,
                Text = initialSessionName?.Trim() ?? string.Empty,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        private static ComboBox CreateSessionFlowComboBox(IReadOnlyList<ChatSessionFlowBindingOption> sessionFlowOptions)
        {
            var comboBox = new ComboBox
            {
                MinWidth = 260,
                MinHeight = 32,
                DisplayMemberPath = nameof(ChatSessionFlowBindingOption.DisplayName),
                ItemsSource = sessionFlowOptions,
                VerticalAlignment = VerticalAlignment.Center
            };

            if (sessionFlowOptions.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }

            return comboBox;
        }

        private void RefreshSessionNameContent()
        {
            if (_sessionNameOption == null)
            {
                return;
            }

            _sessionNameOption.ContentText = string.IsNullOrWhiteSpace(SessionName)
                ? string.Empty
                : SessionName;
        }

        private void RefreshSessionFlowContent()
        {
            if (_sessionFlowOption == null)
            {
                return;
            }

            _sessionFlowOption.ContentText = _flowComboBox.SelectedItem is ChatSessionFlowBindingOption option
                ? option.DisplayName
                : string.Empty;
        }

        private void TryCreateSession()
        {
            if (string.IsNullOrWhiteSpace(SessionName))
            {
                ShowMissingField("会话名称", _sessionNameOption);
                return;
            }

            if (SelectedFlowBinding == null)
            {
                ShowMissingField("会话流", _sessionFlowOption);
                return;
            }

            ClearMainHint();
            HighlightOption(null);
            CloseWithResult(true);
        }

        private void ShowMissingField(string fieldName, UniversalNewDialogOption option)
        {
            ShowMainHint($"缺失{fieldName}，无法创建会话。");
            HighlightOption(option);
        }
    }
}
