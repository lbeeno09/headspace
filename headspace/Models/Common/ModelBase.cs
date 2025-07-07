using CommunityToolkit.Mvvm.ComponentModel;
using System.Runtime.CompilerServices;

namespace headspace.Models.Common
{
    public abstract partial class ModelBase : ObservableObject
    {
        [ObservableProperty]
        private string? _title;

        private bool _isDirty = false;
        public bool IsDirty
        {
            get => _isDirty;
            set => SetProperty(ref _isDirty, value);
        }

        protected bool SetPropertyAndMarkDirty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
        {
            var result = SetProperty(ref field, newValue, propertyName);
            if(result)
            {
                IsDirty = true;
            }

            return result;
        }
    }
}
