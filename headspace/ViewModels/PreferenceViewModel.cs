using CommunityToolkit.Mvvm.ComponentModel;
using headspace.Services.Interfaces;
using headspace.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace headspace.ViewModels
{
    public class PreferenceViewModel : ObservableObject
    {
        private readonly ISettingsService _settingsService;

        public List<AppTheme> ThemeOptions { get; } = Enum.GetValues(typeof(AppTheme)).Cast<AppTheme>().ToList();

        public AppTheme SelectedTheme
        {
            get => _settingsService.CurrentSettings.Theme;
            set
            {
                if(_settingsService.CurrentSettings.Theme != value)
                {
                    _settingsService.CurrentSettings.Theme = value;
                    OnPropertyChanged();
                    _settingsService.ApplyTheme();
                }
            }
        }

        public PreferenceViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }
    }
}