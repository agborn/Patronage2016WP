using Patronage2016WP.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Patronage2016WP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImageDetailsView : Page
    {
        public ImageDetailsView()
        {
            this.InitializeComponent();
            this.DataContext = new ImageDetailsViewModel();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var vm = this.DataContext as ImageDetailsViewModel;
            if (vm != null)
            {
                vm.LoadPage.Execute(e.Parameter);
            }
        }
    }
}
