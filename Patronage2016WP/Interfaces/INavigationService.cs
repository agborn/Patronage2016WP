using System;
using Windows.UI.Xaml.Controls;

namespace Patronage2016WP.Interfaces
{
    public interface INavigationService
    {
        void RegisterRootFrame(Frame frame);
        void Navigate(Type type);
        void Navigate(Type type, object parameter);
        void GoBack();
    }
}
