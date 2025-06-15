using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows.Input;

namespace headspace.Models.Common
{
    public class SidebarItem : ObservableObject
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public Type PageType { get; set; }
        public ICommand Command { get; set; }
    }
}
