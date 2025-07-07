using headspace.Models;
using headspace.Services.Interfaces;
using headspace.ViewModels.Common;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using System.Linq;

namespace headspace.ViewModels
{
    public class MusicViewModel : ViewModelBase<MusicModel>
    {
        private readonly IDialogService _dialogService;
        public XamlRoot? ViewXamlRoot { get; set; }

        public MusicViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        protected override void Add()
        {
            string exampleAbc = @"X:1
T:Example Scale
M:4/4
K:C
C D E F | G A B c";

            var newMusic = new MusicModel { Title = $"New Music {Items.Count + 1}", Content = exampleAbc };

            Items.Add(newMusic);
            SelectedItem = newMusic;
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
