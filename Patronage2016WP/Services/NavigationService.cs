using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Patronage2016WP.Services
{
    public class NavigationService
    {
        #region Private Fields
        private static Frame rootFrame; 
        #endregion

        #region Public Methods
        public void RegisterRootFrame(Frame frame)
        {
            rootFrame = frame;
        }

        public void Navigate(Type type)
        {
            rootFrame.Navigate(type);
        }

        public void Navigate(Type type, object parameter)
        {
            rootFrame.Navigate(type, parameter);
        }

        public void GoBack()
        {
            ((Frame)Window.Current.Content).GoBack();
        } 
        #endregion
    }
}
