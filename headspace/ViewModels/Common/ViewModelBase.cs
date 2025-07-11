using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using headspace.Models.Common;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace headspace.ViewModels.Common
{
    public abstract partial class ViewModelBase<T> : ObservableObject where T : ModelBase
    {
        [ObservableProperty]
        private ObservableCollection<T> _items = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ExportCommand))]
        [NotifyPropertyChangedFor(nameof(IsItemSelected))]
        private T? _selectedItem;

        public bool IsItemSelected => SelectedItem != null;

        [RelayCommand]
        protected abstract void Add();
        [RelayCommand]
        protected abstract void Rename();
        [RelayCommand]
        protected abstract void Delete();
        [RelayCommand]
        protected abstract Task Save();

        [RelayCommand]
        protected abstract Task SaveAll();

        [RelayCommand]
        protected abstract Task Export();
    }
}
