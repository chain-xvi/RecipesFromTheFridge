﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using RecipesFromTheFridge.Services;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace RecipesFromTheFridge.Views
{
    public class ShellNavigationItem : INotifyPropertyChanged
    {
        public string Label { get; set; }

        public Symbol Symbol { get; set; }

        public Type PageType { get; set; }

        private Visibility _selectedVis = Visibility.Collapsed;

        public Visibility SelectedVis
        {
            get { return _selectedVis; }

            set { Set(ref _selectedVis, value); }
        }

        public char SymbolAsChar
        {
            get { return (char)Symbol; }
        }

        private IconElement _iconElement = null;

        public IconElement Icon
        {
            get
            {
                var foregroundBinding = new Binding
                {
                    Source = this,
                    Path = new PropertyPath("SelectedForeground"),
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                };

                if (_iconElement != null)
                {
                    BindingOperations.SetBinding(_iconElement, IconElement.ForegroundProperty, foregroundBinding);

                    return _iconElement;
                }

                var fontIcon = new FontIcon { FontSize = 16, Glyph = SymbolAsChar.ToString() };

                BindingOperations.SetBinding(fontIcon, FontIcon.ForegroundProperty, foregroundBinding);

                return fontIcon;
            }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                Set(ref _isSelected, value);

                SelectedVis = value ? Visibility.Visible : Visibility.Collapsed;

                SelectedForeground = IsSelected
                    ? Application.Current.Resources["SystemControlForegroundAccentBrush"] as SolidColorBrush
                    : GetStandardTextColorBrush();
            }
        }

        private SolidColorBrush _selectedForeground = null;

        public SolidColorBrush SelectedForeground
        {
            get { return _selectedForeground ?? (_selectedForeground = GetStandardTextColorBrush()); }
            set { Set(ref _selectedForeground, value); }
        }

        private ShellNavigationItem(string label, Symbol symbol, Type pageType)
            : this(label, pageType)
        {
            Symbol = symbol;
        }

        private ShellNavigationItem(string label, IconElement icon, Type pageType)
            : this(label, pageType)
        {
            _iconElement = icon;
        }

        private ShellNavigationItem(string label, Type pageType)
        {
            Label = label;
            PageType = pageType;

            ThemeSelectorService.OnThemeChanged += (s, e) =>
            {
                if (!IsSelected)
                {
                    SelectedForeground = GetStandardTextColorBrush();
                }
            };
        }

        public static ShellNavigationItem FromType<T>(string label, Symbol symbol)
            where T : Page
        {
            return new ShellNavigationItem(label, symbol, typeof(T));
        }

        public static ShellNavigationItem FromType<T>(string label, IconElement icon)
            where T : Page
        {
            return new ShellNavigationItem(label, icon, typeof(T));
        }

        private SolidColorBrush GetStandardTextColorBrush()
        {
            return ThemeSelectorService.GetSystemControlForegroundForTheme();
        }

        public override string ToString()
        {
            return Label;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
