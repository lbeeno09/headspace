using headspace.Services.Interfaces;
using Microsoft.UI.Xaml.Controls;
using System;

namespace headspace.Services.Implementations
{
    public class NavigationService : INavigationService
    {
        private Frame _frame;

        public void Initialize(Frame frame)
        {
            _frame = frame;
        }

        public void NavigateTo(string pageKey)
        {
            var pageType = Type.GetType($"headspace.Views.{pageKey}");
            if(pageType != null)
            {
                _frame.Navigate(pageType);
            }
        }
    }
}
