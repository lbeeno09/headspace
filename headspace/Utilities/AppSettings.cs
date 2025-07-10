using CommunityToolkit.Mvvm.ComponentModel;

namespace headspace.Utilities
{
    public partial class AppSettings : ObservableObject
    {
        [ObservableProperty]
        private AppTheme _theme = AppTheme.Default;

        [ObservableProperty]
        private string _defaultAuthorName = "User";

    }
}
