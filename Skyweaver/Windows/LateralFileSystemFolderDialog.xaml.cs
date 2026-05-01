using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace Skyweaver.Windows
{
    public partial class LateralFileSystemFolderDialog : Window
    {
        private readonly bool _requiresSourcePath;

        public LateralFileSystemFolderDialog(
            string title,
            string promptText,
            string confirmButtonText,
            string? inheritedFromName = null,
            string initialName = "",
            string initialSourcePath = "",
            bool requiresSourcePath = true)
        {
            InitializeComponent();

            _requiresSourcePath = requiresSourcePath;
            Title = title;
            PromptTextBlock.Text = promptText;
            ConfirmButton.Content = confirmButtonText;
            NameTextBox.Text = initialName;
            SourcePathTextBox.Text = initialSourcePath;

            if (!string.IsNullOrWhiteSpace(inheritedFromName))
            {
                ParentHintTextBlock.Text = $"继承来源：{inheritedFromName}";
                ParentHintTextBlock.Visibility = Visibility.Visible;
            }

            if (!_requiresSourcePath)
            {
                SourcePathLabel.Visibility = Visibility.Collapsed;
                SourcePathPanel.Visibility = Visibility.Collapsed;
                Height = 220;
            }

            Loaded += (_, _) =>
            {
                NameTextBox.Focus();
                NameTextBox.SelectAll();
            };
        }

        public string FolderDisplayName => NameTextBox.Text.Trim();

        public string SourceFolderPath => SourcePathTextBox.Text.Trim();

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "选择被投影的源文件夹",
                SelectedPath = Directory.Exists(SourceFolderPath)
                    ? SourceFolderPath
                    : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ShowNewFolderButton = false
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SourcePathTextBox.Text = dialog.SelectedPath;
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FolderDisplayName))
            {
                System.Windows.MessageBox.Show(this, "请输入侧向文件夹的显示名称。", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!_requiresSourcePath)
            {
                DialogResult = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(SourceFolderPath))
            {
                System.Windows.MessageBox.Show(this, "请选择被投影的源文件夹。", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Directory.Exists(SourceFolderPath))
            {
                System.Windows.MessageBox.Show(this, "所选的源文件夹不存在。", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
        }
    }
}
