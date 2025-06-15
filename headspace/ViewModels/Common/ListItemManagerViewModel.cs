using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using headspace.Models.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace headspace.ViewModels.Common
{
    public partial class ListItemManagerViewModel<TItem> : ObservableObject where TItem : ProjectItemBase, new()
    {
        public ObservableCollection<TItem> Items { get; }

        private TItem selectedItem;
        public TItem SelectedItem
        {
            get => selectedItem;
            set
            {
                if(SetProperty(ref selectedItem, value))
                {
                    ((RelayCommand)DeleteCommand).NotifyCanExecuteChanged();
                    ((AsyncRelayCommand)RenameCommand).NotifyCanExecuteChanged();

                    OnItemSelected?.Invoke(this, selectedItem);

                }
            }
        }

        // Command for list manipulation
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RenameCommand { get; }

        // notify parent ViewModel when selected
        public event EventHandler<TItem> OnItemSelected;

        [ObservableProperty]
        private XamlRoot xamlRoot;

        public ListItemManagerViewModel(ObservableCollection<TItem> items)
        {
            Items = items;

            AddCommand = new RelayCommand(AddItem);
            DeleteCommand = new RelayCommand(DeleteItem);
            RenameCommand = new AsyncRelayCommand(RenameItemAsync);
        }

        private void AddItem()
        {
            string baseTitle = "Untitled";
            string newTitle = baseTitle;

            int i = 0;
            while(Items.Select(n => n.Title).Contains(newTitle))
            {
                newTitle = $"{baseTitle}{++i}";
            }

            var newItem = new TItem { Title = newTitle, LastModified = DateTime.Now };

            Items.Add(newItem);
            SelectedItem = newItem;
        }


        private void DeleteItem()
        {
            if(SelectedItem != null)
            {
                Items.Remove(SelectedItem);
                SelectedItem = Items.FirstOrDefault();
            }
        }

        // TODO: Set to MVVM structure
        private async Task RenameItemAsync()
        {
            if(SelectedItem != null && XamlRoot != null)
            {
                TextBox inputTextBox = new TextBox { Text = SelectedItem.Title, PlaceholderText = "Enter new name" };

                ContentDialog renameDialog = new ContentDialog
                {
                    Title = "Rename Item",
                    Content = inputTextBox,
                    PrimaryButtonText = "Rename",
                    CloseButtonText = "Cancel",
                    XamlRoot = XamlRoot
                };

                ContentDialogResult result = await renameDialog.ShowAsync();

                if(result == ContentDialogResult.Primary && inputTextBox != null)
                {
                    string newName = inputTextBox.Text.Trim();
                    if(!string.IsNullOrEmpty(newName))
                    {
                        SelectedItem.Title = newName;

                        System.Diagnostics.Debug.WriteLine($"Item renamed to: {newName}");
                    }
                }
            }
            else if(XamlRoot == null)
            {
                System.Diagnostics.Debug.WriteLine($"Cannot show dialog: XamlRoot is null. Ensure its set from Page");
            }
        }
    }
}
