using headspace.Models;
using headspace.Services.Interfaces;
using headspace.ViewModels.Common;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using System.Linq;

namespace headspace.ViewModels
{
    public class NoteViewModel : ViewModelBase<NoteModel>
    {
        private readonly IProjectService _projectService;
        private readonly IDialogService _dialogService;

        public XamlRoot? ViewXamlRoot { get; set; }

        public NoteViewModel(IProjectService projectService, IDialogService dialogService)
        {
            _projectService = projectService;
            _dialogService = dialogService;

            Items = _projectService.CurrentProject.Notes;
        }

        protected override void Add()
        {
            var newNote = new NoteModel { Title = $"New Note {Items.Count + 1}", Content = "" };

            Items.Add(newNote);
            SelectedItem = newNote;
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
