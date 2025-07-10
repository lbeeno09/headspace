using Microsoft.UI.Xaml.Controls;

namespace headspace.Views.Common
{
    public sealed partial class RenameDialog : ContentDialog
    {
        public string NewName => NameTextBox.Text;

        public RenameDialog(string currentName)
        {
            InitializeComponent();

            NameTextBox.Text = currentName;
        }
    }
}
