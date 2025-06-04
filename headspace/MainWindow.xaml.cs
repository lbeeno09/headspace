using headspace.Views;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
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
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(CustomTitleBar);

            RootNavigationView.SelectedItem = RootNavigationView.MenuItems[0];
            ContentFrame.Content = homePage;
            currentPage = homePage;
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            if(this.AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.Minimize();
            }
        }

        private void MaximizeRestore_Click(object sender, RoutedEventArgs e)
        {
            if (this.AppWindow.Presenter is OverlappedPresenter presenter)
            {
                if(presenter.IsMaximizable)
                {
                    presenter.Maximize();
                } else
                {
                    presenter.Restore();
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if(args.IsSettingsSelected)
            {
                //currentPage = settingsPage;
                //ContentFrame.Content = settingsPage;

                //return;
            }

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

        private void NavigationView_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if(RootNavigationView.IsPaneOpen)
            {
                RootNavigationView.IsPaneOpen = false;
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

        private async void PreferencesMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
