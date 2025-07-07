using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using headspace.Models;
using headspace.Models.Common;
using headspace.Services.Interfaces;
using headspace.ViewModels.Common;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using System.Linq;
using Windows.UI;

namespace headspace.ViewModels
{
    public partial class StoryboardViewModel : ViewModelBase<StoryboardModel>
    {
        private readonly IDialogService _dialogService;
        public XamlRoot? ViewXamlRoot { get; set; }

        [ObservableProperty]
        private Color _primaryColor = Colors.Black;

        [ObservableProperty]
        private Color _secondaryColor = Colors.White;

        [ObservableProperty]
        private float _strokeThickness = 2.0f;

        [ObservableProperty]
        private bool _isEraserMode = false;

        [ObservableProperty]
        private PanelModel? _activePanel;


        public StoryboardViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        [RelayCommand]
        private void AddPanel()
        {
            if(SelectedItem == null)
            {
                return;
            }

            var newPanel = new PanelModel { Title = $"Panel {SelectedItem.Panels.Count + 1}" };

            SelectedItem.Panels.Add(newPanel);
            ActivePanel = newPanel;
        }

        [RelayCommand]
        private void DeletePanel()
        {
            if(SelectedItem == null || ActivePanel == null || SelectedItem.Panels.Count <= 1)
            {
                return;
            }

            int index = SelectedItem.Panels.IndexOf(ActivePanel);
            SelectedItem.Panels.Remove(ActivePanel);
            ActivePanel = SelectedItem.Panels.ElementAtOrDefault(index - 1) ?? SelectedItem.Panels.FirstOrDefault();
        }

        protected override void Add()
        {
            var newStoryboard = new StoryboardModel { Title = $"New Scene {Items.Count + 1}" };
            var initialPanel = new PanelModel { Title = "Base Panel" };
            newStoryboard.Panels.Add(initialPanel);

            Items.Add(newStoryboard);
            SelectedItem = newStoryboard;
            ActivePanel = initialPanel;
        }

        protected override async void Rename()
        {
            if(SelectedItem == null || ViewXamlRoot == null)
            {
                return;
            }

            var newName = await _dialogService.ShowRenameDialogAsync(SelectedItem.Title, ViewXamlRoot);
            if(!string.IsNullOrWhiteSpace(newName))
            {
                SelectedItem.Title = newName;
            }
        }

        protected override void Delete()
        {
            if(SelectedItem == null)
            {
                return;
            }

            Items.Remove(SelectedItem);
            SelectedItem = Items.FirstOrDefault();
        }

        protected override void Save()
        {
            if(SelectedItem == null)
            {
                return;
            }

            Debug.WriteLine($"SAVING ITEM: {SelectedItem.Title}");
        }

        protected override void SaveAll()
        {
            Debug.WriteLine("SAVING ALL ITEMS...");
            if(Items.Count == 0)
            {
                Debug.WriteLine("No items to save.");
                return;
            }

            foreach(var note in Items)
            {
                Debug.WriteLine($" -> Saving: {note.Title}");
            }
            Debug.WriteLine("...DONE");
        }
    }
}
