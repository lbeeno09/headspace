using headspace.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace headspace
{
    public sealed partial class MainWindow : Window
    {
        private readonly HomePage homePage = new();
        private readonly NotesPage notesPage = new();
        private readonly DocumentsPage documentsPage = new();
        private readonly ScreenplayPage screenplayPage = new();
        private readonly DrawingsPage drawingsPage = new();
        private readonly MoodboardPage moodboardPage = new();
        private readonly StoryboardPage storyboardPage = new();
        //private readonly MusicsPage musicsPage;

        private object currentPage = null;

        public MainWindow()
        {
            this.InitializeComponent();

            RootNavigationView.SelectedItem = RootNavigationView.MenuItems[0];
            ContentFrame.Content = homePage;
            currentPage = homePage;
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            string selectedTag = (args.SelectedItem as NavigationViewItem)?.Tag?.ToString();

            object targetPage = selectedTag switch
            {
                "HomePage" => homePage,
                "NotesPage" => notesPage,
                "DocumentsPage" => documentsPage,
                "ScreenplayPage" => screenplayPage,
                "DrawingsPage" => drawingsPage,
                "MoodboardPage" => moodboardPage,
                "StoryboardPage" => storyboardPage,
                // "MusicsPage" => musicPage,
                _ => null
            };

            if(targetPage != null && targetPage != currentPage)
            {
                currentPage = targetPage;
                ContentFrame.Content = targetPage;
            }
        }

        private async void SaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add("Head Space", new List<string>() { ".hsp" });
            picker.SuggestedFileName = "Untitled";

            // Associate picker with app window
            IntPtr hwnd = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSaveFileAsync();
            if(file != null)
            {
                // TODO: Save project here
                using var stream = await file.OpenStreamForWriteAsync();
                using var writer = new StreamWriter(stream);
                writer.Write("Project data goes here.");
            }
        }
    }
}
