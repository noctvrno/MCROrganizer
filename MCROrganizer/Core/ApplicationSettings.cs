using MCROrganizer.Core.Utils;
using System;
using System.ComponentModel;

namespace MCROrganizer.Core
{
    public enum ApplicationMode
    {
        Classic,
        Modern
    }

    public class ApplicationSettings
    {
        private static ApplicationMode _mode = ApplicationMode.Classic;
        public static ApplicationMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                NotifyPropertyChanged(nameof(Mode));
            }
        }

        public static event PropertyChangedEventHandler StaticPropertyChanged;

        private static void NotifyPropertyChanged(String propertyName)
        {
            StaticPropertyChanged?.Invoke(null, new(propertyName));
        }
    }
}
