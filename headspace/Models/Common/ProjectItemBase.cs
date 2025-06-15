using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace headspace.Models.Common
{
    public abstract partial class ProjectItemBase : ObservableObject
    {
        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private DateTime lastModified;

        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}
