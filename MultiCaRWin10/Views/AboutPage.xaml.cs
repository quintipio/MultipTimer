using Windows.UI.Xaml.Navigation;
using MultiCaRWin10.Context;

namespace MultiCaRWin10.Views
{
    /// <summary>
    /// Page à propos de
    /// </summary>
    public sealed partial class AboutPage
    {

        /// <summary>
        /// Constructeur
        /// </summary>
        public AboutPage()
        {
            InitializeComponent();

        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Background = App.ListTimer.GetBackGroundColor();
            AppNomAppTextBlock.Text = ContextStatic.NomAppli;
            VersionTextBlock.Text = ContextStatic.Version;
            DeveloppeurTextBlock.Text = ContextStatic.Developpeur;
        }

        private void BackBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
