using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MultiCaRWin10.Context;
using MultiCaRWin10.Model;
using MultiCaRWin10.ViewModel;

namespace MultiCaRWin10.Views
{
    /// <summary>
    /// Page principale
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        /// ViewModel
        /// </summary>
        public MainPageViewModel ViewModel { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            ViewModel = new MainPageViewModel();
            MainPivot.Title = ContextStatic.NomAppli;
        }
        
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Background = App.ListTimer.GetBackGroundColor();
            SelectGridBas();

        }



        #region outils

        /// <summary>
        /// sélectionne la grid du bas en fonction du pivot Item
        /// </summary>
        private void SelectGridBas()
        {
            MiscCaRGrid.Visibility = MainPivot.SelectedIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
            MiscChronoGrid.Visibility = MainPivot.SelectedIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

        #region évènements Boutons et Pivot généraux
        //évènement pour changer l'affichage commun en fonction du pivot
        private void MainPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectGridBas();
        }
        
        //boutons de note pour l'appli
        private async void RateButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?PFN=" + Package.Current.Id.FamilyName));
        }

        //boutons mail pour bugs et suggestion
        private async void BugsButton_Click(object sender, RoutedEventArgs e)
        {
            var mailto = new Uri("mailto:?to=" + ContextStatic.Support + "&subject=Bugs ou suggestions pour " + ContextStatic.NomAppli);
            await Launcher.LaunchUriAsync(mailto);
        }

        //bouton A propos de...
        private void AppdButton_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(AboutPage),null);
        }
        #endregion
        
        #region boutons compte à rebours
        private void AddCaRButton_Click(object sender, RoutedEventArgs e)
        {
            MainPageViewModel.DelegateAjoutTimer ajoutTimer = ViewModel.SaveChrono;
            var list = new List<object> { ajoutTimer };
            ((Frame)Window.Current.Content).Navigate(typeof(GererTimer), list);
        }
        
        private void OpenParam_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(ParamToastPage));
        }

        private async void SortCaR_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.SortCaR();
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;
            await ViewModel.StopCaR((CaR)button.Tag,false);
            MainPageViewModel.DelegateAjoutTimer ajoutTimer = ViewModel.SaveChrono;
            var list = new List<object> { ajoutTimer, button.Tag as CaR };
            ((Frame)Window.Current.Content).Navigate(typeof(GererTimer), list);
        }

        private async void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.StopCaR(((Button)sender).Tag as CaR,false);
        }

        private async void ValidButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.ValidCaR(((Button)sender).Tag as CaR);
        }

        private async void LectureButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.StartCaR(((Button)sender).Tag as CaR);
        }

        private async void SupButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.SupCaR(((Button)sender).Tag as CaR);
        }

        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.ResetCaR(((Button)sender).Tag as CaR);
        }
        #endregion

        #region boutons chronos

        private async void ResetChronoButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.ResetChrono();
        }

        private async void AddChronoButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.AddChrono();
        }

        private async void ValidChronoButtonv_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.ValidChrono(((Button)sender).Tag as Chrono);
        }

        private async void PauseChronoButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.PauseChrono(((Button)sender).Tag as Chrono);
        }

        private async void StartChronoButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.StartChrono(((Button)sender).Tag as Chrono);
        }

        private async void SupChronoButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.SupChrono(((Button)sender).Tag as Chrono);
        }

        private async void MemChronoButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.SaveChrono(((Button)sender).Tag as Chrono);
        }
        #endregion
        
    }
}
