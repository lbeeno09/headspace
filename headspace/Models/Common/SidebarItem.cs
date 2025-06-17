using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;

namespace headspace.Models.Common
{
    public class SidebarItem : ObservableObject
    {
        public string Content { get; set; }
        public SymbolIcon Icon { get; set; }
        public string Tag { get; set; }
        public ICommand Command { get; set; }
    }
}
