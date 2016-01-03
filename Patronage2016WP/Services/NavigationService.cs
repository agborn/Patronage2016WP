using Patronage2016WP.Interfaces;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Patronage2016WP.Services
{
    public class NavigationService : INavigationService
    {
        #region Private Fields
        private static Frame _rootFrame; 
        #endregion

        #region Public Methods
        public void RegisterRootFrame(Frame frame)
        {
            _rootFrame = frame;
        }

        public void Navigate(Type type)
        {
            _rootFrame.Navigate(type);
        }

        public void Navigate(Type type, object parameter)
        {
            _rootFrame.Navigate(type, parameter);
        }

        public void GoBack()
        {
            ((Frame)Window.Current.Content).GoBack();
        } 
        #endregion
    }
}
