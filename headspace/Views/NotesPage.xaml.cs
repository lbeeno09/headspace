using headspace.ViewModels;
using headspace.Views.Common;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;

namespace headspace.Views
{
    public sealed partial class NotesPage : Page, ISavablePage
    {
        private NoteViewModel ViewModel => DataContext as NoteViewModel;

        public NotesPage()
        {
            this.InitializeComponent();

            this.DataContext = new NoteViewModel();
            this.Loaded += (s, e) =>
            {
                if(ViewModel != null)
                {
                    ViewModel.PageXamlRoot = this.XamlRoot;
                }
            };
        }

        public void SavePageContentToModel()
        {
            if(ViewModel.SelectedNote != null)
            {
                ViewModel.SelectedNote.LastModified = DateTime.Now;

                System.Diagnostics.Debug.WriteLine($"Notes page content updated in model for {ViewModel.SelectedNote.Title}");
            }
        }
    }

    public class NullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
