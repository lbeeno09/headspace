using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace headspace.ViewModels.Common
{
    [ObservableObject]
    public abstract partial class ViewModelBase<T> where T : class
    {
        [ObservableProperty]
        private ObservableCollection<T> _items = new();
        [ObservableProperty]
        private T? _selectedItem;



        [RelayCommand]
        protected abstract void Add();
        [RelayCommand]
        protected abstract void Rename();
        [RelayCommand]
        protected abstract void Delete();
        [RelayCommand]
        protected abstract void Save();
        [RelayCommand]
        protected abstract void SaveAll();
    }
}
