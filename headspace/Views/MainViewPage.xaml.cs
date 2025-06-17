using headspace.Models.Common;
using headspace.Services.Implementations;
using headspace.Views.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Storage;

namespace headspace.Views
{
    public sealed partial class MainViewPage : Page
    {
        private const string _FileExtension = ".hsp";
        private ProjectDataService _projectDataService;

        public MainViewPage()
        {
            this.InitializeComponent();
            this.Loaded += MainViewPage_Loaded;

            ContentFrame.CacheSize = 10;

            _projectDataService = new ProjectDataService((App.Current as App).m_window);

            NavView.IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
            NavView.PaneDisplayMode = NavigationViewPaneDisplayMode.LeftCompact;
            ContentFrame.Navigate(typeof(Views.HomePage));

            //LoadProjectOnStartup();
        }

        private void MainViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            var appInstance = App.Current as App;
            if(appInstance != null && appInstance.m_window != null)
            {
                var currentWindow = appInstance.m_window;

                currentWindow.ExtendsContentIntoTitleBar = true;
                currentWindow.SetTitleBar(AppTitleBar);
            }
        }

        // --- Navigation View ---
        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            var navItems = new List<SidebarItem>
            {
                new SidebarItem { Content = "Home", Icon = new SymbolIcon(Symbol.Home),  Tag = "home" },
                new SidebarItem { Content = "Note", Icon = new SymbolIcon(Symbol.Memo),  Tag = "note" },
                new SidebarItem { Content = "Document", Icon = new SymbolIcon(Symbol.Document),  Tag = "document" },
                new SidebarItem { Content = "Screenplay", Icon = new SymbolIcon(Symbol.Message),  Tag = "screenplay" },
                new SidebarItem { Content = "Drawing", Icon = new SymbolIcon(Symbol.Highlight),  Tag = "drawing" },
                new SidebarItem { Content = "Moodboard", Icon = new SymbolIcon(Symbol.People),  Tag = "moodboard" },
                new SidebarItem { Content = "Storyboard", Icon = new SymbolIcon(Symbol.ViewAll),  Tag = "storyboard" },
            };
            foreach(var item in navItems)
            {
                var navItem = new NavigationViewItem
                {
                    Content = item.Content,
                    Icon = item.Icon,
                    Tag = item.Tag
                };

                NavView.MenuItems.Add(navItem);
            }
            NavView.MenuItems.Add(new NavigationViewItemSeparator());

            // Initial Item
            NavView.SelectedItem = navItems[0];
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if(args.IsSettingsInvoked)
            {
                ContentFrame.Navigate(typeof(PreferencesPage));
            }
            else
            {
                var item = sender.MenuItems.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem);
                NavView_Navigate(item as NavigationViewItem);
            }
        }

        private void NavView_Navigate(NavigationViewItem item)
        {
            switch(item.Tag)
            {
                case "home":
                    ContentFrame.Navigate(typeof(HomePage));
                    break;
                case "note":
                    ContentFrame.Navigate(typeof(NotesPage));
                    break;
                case "document":
                    ContentFrame.Navigate(typeof(DocumentsPage));
                    break;
                case "screenplay":
                    ContentFrame.Navigate(typeof(ScreenplayPage));
                    break;
                case "drawing":
                    ContentFrame.Navigate(typeof(DrawingsPage));
                    break;
                case "moodboard":
                    ContentFrame.Navigate(typeof(MoodboardPage));
                    break;
                case "storyboard":
                    ContentFrame.Navigate(typeof(StoryboardPage));
                    break;
            }
        }

        private async void LoadProjectOnStartup()
        {
            Project loadedProject = await _projectDataService.LoadProjectAsync(Constants.DefaultProjectFileName);
            UpdateGlobalProject(loadedProject);
            ContentFrame.Navigate(typeof(Views.HomePage));
        }

        private async void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            StorageFile file = await _projectDataService.PickOpenFileAsync(new List<string> { _FileExtension });
            if(file != null)
            {
                Project loadedProject = await _projectDataService.LoadProjectAsync(file.Name, file);
                UpdateGlobalProject(loadedProject);
                ContentFrame.Navigate(typeof(Views.HomePage));
            }
        }

        private async void SaveProjectAs_Click(object sender, RoutedEventArgs e)
        {
            var appInstance = App.Current as App;
            StorageFile file = await _projectDataService.PickSaveFileAsync(appInstance.CurrentProject.ProjectName, new List<string> { _FileExtension });

            if(file != null)
            {
                _projectDataService.SetProjectName(appInstance.CurrentProject, Path.GetFileNameWithoutExtension(file.Name));

                await _projectDataService.SaveProjectAsync(appInstance.CurrentProject, file.Name, file);
            }
        }

        private async void SaveProject_Click(object sender, RoutedEventArgs e)
        {
            if(ContentFrame.Content is ISavablePage currentPage)
            {
                currentPage.SavePageContentToModel();
            }

            await _projectDataService.SaveProjectAsync((App.Current as App).CurrentProject, Constants.DefaultProjectFileName);
        }

        ////////// Helper Methods //////////
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for(int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if(child is T typedChild)
                {
                    return typedChild;
                }

                var foundChild = FindVisualChild<T>(child);
                if(foundChild != null)
                {
                    return foundChild;
                }
            }

            return null;
        }

        private void UpdateGlobalProject(Project newProject)
        {
            var appInstance = App.Current as App;
            if(appInstance != null && newProject != null)
            {
                appInstance.CurrentProject.ProjectName = newProject.ProjectName;

                // TODO: abstract all this into one op
                appInstance.CurrentProject.Notes.Clear();
                foreach(var note in newProject.Notes)
                {
                    appInstance.CurrentProject.Notes.Add(note);
                }
                appInstance.CurrentProject.Documents.Clear();
                foreach(var document in newProject.Documents)
                {
                    appInstance.CurrentProject.Documents.Add(document);
                }
                appInstance.CurrentProject.Screenplays.Clear();
                foreach(var screenplay in newProject.Screenplays)
                {
                    appInstance.CurrentProject.Screenplays.Add(screenplay);
                }
                appInstance.CurrentProject.Drawings.Clear();
                foreach(var drawing in newProject.Drawings)
                {
                    appInstance.CurrentProject.Drawings.Add(drawing);
                }
                appInstance.CurrentProject.Moodboards.Clear();
                foreach(var moodboard in newProject.Moodboards)
                {
                    appInstance.CurrentProject.Moodboards.Add(moodboard);
                }
                appInstance.CurrentProject.Storyboards.Clear();
                foreach(var storyboard in newProject.Storyboards)
                {
                    appInstance.CurrentProject.Storyboards.Add(storyboard);
                }
                appInstance.CurrentProject.Musics.Clear();
                foreach(var music in newProject.Musics)
                {
                    appInstance.CurrentProject.Musics.Add(music);
                }
            }
        }

    }

    public static class Constants
    {
        public const string DefaultProjectFileName = "NewProject.hsp";
    }
}
