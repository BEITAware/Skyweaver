using System.Collections.Generic;
using System.Windows;
using Ferrita.PageControls.Tiles.ViewModels;

namespace Ferrita.Windows
{
    public partial class StickyNoteRepliesWindow : Window
    {
        public StickyNoteRepliesWindow(IEnumerable<StickyNoteReplyViewModel> replies, string title)
        {
            InitializeComponent();
            TxtTitle.Text = title;
            LstReplies.ItemsSource = replies;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
