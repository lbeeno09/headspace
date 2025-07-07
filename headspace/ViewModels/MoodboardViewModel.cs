using CommunityToolkit.Mvvm.ComponentModel;
using headspace.Models;
using headspace.Services.Interfaces;
using headspace.ViewModels.Common;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using System.Linq;
using Windows.UI;

namespace headspace.ViewModels
{
    public partial class MoodboardViewModel : ViewModelBase<MoodboardModel>
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


        public MoodboardViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        protected override void Add()
        {
            var newMoodboard = new MoodboardModel { Title = $"New Moodboard {Items.Count + 1}" };

            Items.Add(newMoodboard);
            SelectedItem = newMoodboard;
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
