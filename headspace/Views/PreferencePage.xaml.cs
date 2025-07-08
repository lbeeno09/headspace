using headspace.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace headspace.Views
{
    public sealed partial class PreferencePage : Page
    {
        public PreferenceViewModel ViewModel { get; }

        public PreferencePage()
        {
            this.InitializeComponent();

            ViewModel = ((App)Application.Current).Services.GetRequiredService<PreferenceViewModel>();
        }
    }
}
