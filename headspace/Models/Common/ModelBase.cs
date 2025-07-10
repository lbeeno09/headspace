using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace headspace.Models.Common
{
    public abstract partial class ModelBase : ObservableObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        private string? _title;
        public string? Title
        {
            get => _title;
            set => SetPropertyAndMarkDirty(ref _title, value);
        }

        [JsonIgnore]
        public bool IsDirty { get; set; }

        [JsonIgnore]
        // Which save folder to use
        public abstract string FilePathPrefix { get; }

        protected ModelBase()
        {
            IsDirty = true;
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
