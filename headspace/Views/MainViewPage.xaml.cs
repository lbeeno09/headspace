using headspace.Models.Common;
using headspace.Views.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace headspace.Views
{
    public sealed partial class MainViewPage : Page
    {
        private ObservableCollection<SidebarItem> navigationItems;
        private SidebarItem preferencesItem;
        private const string FileExtension = ".hsp";

        public MainViewPage()
        {
            this.InitializeComponent();
            this.Loaded += MainViewPage_Loaded;

            ContentFrame.CacheSize = 15;

            navigationItems = new ObservableCollection<SidebarItem>
            {
                new SidebarItem { Name = "Home", Icon = "\xE80F", PageType = typeof(Views.HomePage) },
                new SidebarItem { Name = "Note", Icon = "\xE70B", PageType = typeof(Views.NotesPage) },
                new SidebarItem { Name = "Document", Icon = "\xE8A5", PageType = typeof(Views.DocumentsPage) },
                new SidebarItem { Name = "Screenplay", Icon = "\xEFA9", PageType = typeof(Views.ScreenplayPage) },
                new SidebarItem { Name = "Drawing", Icon = "\xE8B9", PageType = typeof(Views.DrawingsPage) },
                new SidebarItem { Name = "Moodboard", Icon = "\xE8B8", PageType = typeof(Views.MoodboardPage) },
                new SidebarItem { Name = "Storyboard", Icon = "\xE8A9", PageType = typeof(Views.StoryboardPage) },
                //new SidebarItem {Name = "Music", Icon = "&#xE8A9;", PageType = typeof(Views.MusicPage) },
            };

            preferencesItem = new SidebarItem { Name = "Preferences", Icon = "\xE713", PageType = typeof(Views.PreferencesPage) };

            SidebarNavigationItemsControl.ItemsSource = navigationItems;
            PreferencesButtonContainer.Content = preferencesItem;

            ContentFrame.Navigate(typeof(Views.HomePage));

            CollapseSidebar();

            LoadProjectData(Constants.DefaultProjectFileName);
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

        private void Sidebar_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ExpandSidebar();
        }


        private void Sidebar_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            CollapseSidebar();
        }

        private void ExpandSidebar()
        {
            SidebarGrid.Width = 200;

            UpdateSidebarItemTextVisibility(Visibility.Visible);
        }

        private void CollapseSidebar()
        {
            SidebarGrid.Width = 50;

            UpdateSidebarItemTextVisibility(Visibility.Collapsed);
        }

        private void UpdateSidebarItemTextVisibility(Visibility visibility)
        {
            foreach(var item in SidebarNavigationItemsControl.Items)
            {
                var container = SidebarNavigationItemsControl.ContainerFromItem(item) as ContentPresenter;
                if(container != null)
                {
                    var textBlock = FindVisualChild<TextBlock>(container);
                    if(textBlock != null)
                    {
                        textBlock.Visibility = visibility;
                    }
                }
            }

            var preferencesTextBlock = FindVisualChild<TextBlock>(PreferencesButtonContainer);
            if(preferencesTextBlock != null)
            {
                preferencesTextBlock.Visibility = visibility;
            }
        }

        // Helper method 
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

        private void SidebarNavigationButton_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button clickedButton && clickedButton.DataContext is SidebarItem item)
            {
                if(item.PageType != null)
                {
                    ContentFrame.Navigate(item.PageType);
                    System.Diagnostics.Debug.WriteLine($"Navigated to: {item.Name}");
                }
            }
        }

        private async void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker();

            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle((App.Current as App).m_window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, windowHandle);

            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(FileExtension);

            StorageFile file = await openPicker.PickSingleFileAsync();
            if(file != null)
            {
                await LoadProjectData(file.Name, file);
            }
        }

        private async Task LoadProjectData(string fileName, StorageFile file = null)
        {
            try
            {
                if(file == null)
                {
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                    file = await localFolder.GetFileAsync(fileName);
                }

                string json = await FileIO.ReadTextAsync(file);
                var loadedProject = JsonSerializer.Deserialize<Project>(json);

                var appInstance = App.Current as App;
                if(appInstance != null && loadedProject != null)
                {
                    appInstance.CurrentProject.ProjectName = loadedProject.ProjectName;

                    // TODO: abstract all this into one op
                    appInstance.CurrentProject.Notes.Clear();
                    foreach(var note in loadedProject.Notes)
                    {
                        appInstance.CurrentProject.Notes.Add(note);
                    }
                    appInstance.CurrentProject.Documents.Clear();
                    foreach(var document in loadedProject.Documents)
                    {
                        appInstance.CurrentProject.Documents.Add(document);
                    }
                    appInstance.CurrentProject.Screenplays.Clear();
                    foreach(var screenplay in loadedProject.Screenplays)
                    {
                        appInstance.CurrentProject.Screenplays.Add(screenplay);
                    }
                    appInstance.CurrentProject.Drawings.Clear();
                    foreach(var drawing in loadedProject.Drawings)
                    {
                        appInstance.CurrentProject.Drawings.Add(drawing);
                    }
                    appInstance.CurrentProject.Moodboards.Clear();
                    foreach(var moodboard in loadedProject.Moodboards)
                    {
                        appInstance.CurrentProject.Moodboards.Add(moodboard);
                    }
                    appInstance.CurrentProject.Storyboards.Clear();
                    foreach(var storyboard in loadedProject.Storyboards)
                    {
                        appInstance.CurrentProject.Storyboards.Add(storyboard);
                    }

                    ContentFrame.Navigate(typeof(Views.HomePage));

                    System.Diagnostics.Debug.WriteLine($"Project '{appInstance.CurrentProject.ProjectName}' loaded from: {file.Path}");
                }
            }
            catch(FileNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine($"No project file '{fileName}' found. Starting with default/new project.");

                var appInstance = App.Current as App;
                appInstance.CurrentProject = new Project();
                ContentFrame.Navigate(typeof(Views.HomePage));
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading project: {ex.Message}");
            }
        }

        private async void SaveProject_Click(object sender, RoutedEventArgs e)
        {
            await SaveProjectAsync((App.Current as App).CurrentProject.ProjectName);
        }

        private async void SaveProjectAs_Click(object sender, RoutedEventArgs e)
        {
            var savePicker = new FileSavePicker();

            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle((App.Current as App).m_window);
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, windowHandle);

            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("Headspace", new List<string> { FileExtension });
            savePicker.SuggestedFileName = (App.Current as App).CurrentProject.ProjectName;

            StorageFile file = await savePicker.PickSaveFileAsync();
            if(file != null)
            {
                var appInstance = App.Current as App;
                appInstance.CurrentProject.ProjectName = Path.GetFileNameWithoutExtension(file.Name);
                await SaveProjectAsync(file.Name, file);
            }
        }

        private async Task SaveProjectAsync(string fileName, StorageFile file = null)
        {
            try
            {
                if(ContentFrame.Content is ISavablePage currentPage)
                {
                    currentPage.SavePageContentToModel();
                }

                if(file == null)
                {
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                    file = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                }

                var appInstance = App.Current as App;
                string json = JsonSerializer.Serialize(appInstance.CurrentProject, new JsonSerializerOptions { WriteIndented = true });

                await FileIO.WriteTextAsync(file, json);

                System.Diagnostics.Debug.WriteLine($"Project '{appInstance.CurrentProject.ProjectName}' saved to: {file.Path}");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving project: {ex.Message}");
            }
        }
    }

    public static class Constants
    {
        public const string DefaultProjectFileName = "NewProject.hsp";
    }
}
