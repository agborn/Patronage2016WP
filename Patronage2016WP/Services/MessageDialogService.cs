using System;

namespace Patronage2016WP.Services
{
    public class MessageDialogService
    {
        public static async void ShowMessageDialog(string message)
        {
            Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(message);
            await dialog.ShowAsync();
        }
    }
}
