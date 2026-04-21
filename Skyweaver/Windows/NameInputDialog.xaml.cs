using System.Windows;

namespace Skyweaver.Windows
{
    public partial class NameInputDialog : Window
    {
        private readonly string _validationMessage;

        public NameInputDialog(
            string title,
            string promptText,
            string initialValue,
            string confirmButtonText,
            string validationMessage)
        {
            _validationMessage = validationMessage;

            InitializeComponent();
            Title = title;
            PromptTextBlock.Text = promptText;
            ValueTextBox.Text = initialValue ?? string.Empty;
            ConfirmButton.Content = confirmButtonText;

            Loaded += OnLoaded;
        }

        public string InputValue => ValueTextBox.Text.Trim();

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ValueTextBox.Focus();
            ValueTextBox.SelectAll();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputValue))
            {
                MessageBox.Show(this, _validationMessage, Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
        }
    }
}
