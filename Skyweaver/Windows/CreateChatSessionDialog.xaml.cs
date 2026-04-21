using System.Windows;
using Skyweaver.Models.ChatSession;
using Skyweaver.Services.ChatSession;

namespace Skyweaver.Windows
{
    public partial class CreateChatSessionDialog : Window
    {
        private readonly IReadOnlyList<ChatSessionFlowBindingOption> _sessionFlowOptions;
        private readonly string _initialSessionName;

        public string SessionName => SessionNameTextBox.Text.Trim();

        public ChatSessionFlowBinding? SelectedFlowBinding =>
            (SessionFlowComboBox.SelectedItem as ChatSessionFlowBindingOption)?.ToBinding();

        public CreateChatSessionDialog(
            IReadOnlyList<ChatSessionFlowBindingOption>? sessionFlowOptions = null,
            string? initialSessionName = null)
        {
            _sessionFlowOptions = sessionFlowOptions?.ToArray() ?? Array.Empty<ChatSessionFlowBindingOption>();
            _initialSessionName = string.IsNullOrWhiteSpace(initialSessionName) ? "新建会话" : initialSessionName.Trim();
            InitializeComponent();
            SessionNameTextBox.Text = _initialSessionName;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SessionFlowComboBox.ItemsSource = _sessionFlowOptions;
            SessionFlowComboBox.SelectedIndex = _sessionFlowOptions.Count > 0 ? 0 : -1;
            SessionNameTextBox.Focus();
            SessionNameTextBox.SelectAll();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SessionName))
            {
                MessageBox.Show(this, "请输入会话名称。", "创建会话", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedFlowBinding == null)
            {
                MessageBox.Show(this, "请先为新会话选择一个会话流。", "创建会话", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
        }
    }
}
