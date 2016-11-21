using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MultiCaRWin10.Context;
using MultiCaRWin10.Model;
using MultiCaRWin10.Utils;
using MultiCaRWin10.ViewModel;

namespace MultiCaRWin10.Views
{

    /// <summary>
    /// Page d'ajout ou de modification des Timers
    /// </summary>
    public sealed partial class GererTimer
    {
        private int _idTimerModif;
        private MainPageViewModel.DelegateAjoutTimer _ajoutTimer;

        /// <summary>
        /// Constructeur
        /// </summary>
        public GererTimer()
        {
            InitializeComponent();

        }
        
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Background = App.ListTimer.GetBackGroundColor();

            //affichage du nom de l'appli
            NomAppliTextBlock.Text = ContextStatic.NomAppli;

            //récupération du type d'utilisation de la fenetre et de l'objet sur lequel ont travail
            var list = e.Parameter as List<object>;
            if (list != null)
            {
               //en premier on récupère le délégate de la méthode d'ajout
                _ajoutTimer = (MainPageViewModel.DelegateAjoutTimer) list[0];

                NomPageTextBlock.Text = ResourceLoader.GetForCurrentView().GetString("TextAjouterCaR");
                if (list.Count != 2 || !(list[1] is Timer)) return;
                NomPageTextBlock.Text = ResourceLoader.GetForCurrentView().GetString("TextModifierCaR");
                var chrono = list[1] as Timer;
                _idTimerModif = chrono.Id;
                TitreCaRTextBox.Text = chrono.Titre;
                var temps = DateUtils.ConvertirSecondesHeures(chrono.NbSecondes);
                HeureCaRTextBox.Text = temps[2].ToString();
                MinuteCaRTextBox.Text = temps[1].ToString();
                SecondeCaRTextBox.Text = temps[0].ToString();
            }
            
        }





        #region gestion compte à rebours
        private void ValidCaRButton_OnClick(object sender, RoutedEventArgs e)
        {
            //controle des données
            var ok = true;
            if (string.IsNullOrWhiteSpace(TitreCaRTextBox.Text))
            {
                TitreCaRTextBox.Background = new SolidColorBrush(Color.FromArgb(255, 150, 0, 0));
                ok = false;
            }
            else
            {
                TitreCaRTextBox.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }

            var heure = 0;
            var minutes = 0;
            var secondes = 0;
            if (!string.IsNullOrWhiteSpace(HeureCaRTextBox.Text) && !int.TryParse(HeureCaRTextBox.Text, out heure) || heure < 0)
            {
                HeureCaRTextBox.Background = new SolidColorBrush(Color.FromArgb(255, 150, 0, 0));
                ok = false;
            }
            else
            {
                HeureCaRTextBox.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }

            if (!string.IsNullOrWhiteSpace(MinuteCaRTextBox.Text) && (!int.TryParse(MinuteCaRTextBox.Text, out minutes) || (int.TryParse(MinuteCaRTextBox.Text, out minutes) && (minutes > 59 || minutes < 0))))
            {
                MinuteCaRTextBox.Background = new SolidColorBrush(Color.FromArgb(255, 150, 0, 0));
                ok = false;
            }
            else
            {
                MinuteCaRTextBox.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }

            if (!String.IsNullOrWhiteSpace(SecondeCaRTextBox.Text) && (!int.TryParse(SecondeCaRTextBox.Text, out secondes) || (int.TryParse(SecondeCaRTextBox.Text, out secondes) && (secondes > 59 || secondes < 0))))
            {
                SecondeCaRTextBox.Background = new SolidColorBrush(Color.FromArgb(255, 150, 0, 0));
                ok = false;
            }
            else
            {
                SecondeCaRTextBox.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }

            if (String.IsNullOrWhiteSpace(SecondeCaRTextBox.Text) && String.IsNullOrWhiteSpace(MinuteCaRTextBox.Text) &&
                String.IsNullOrWhiteSpace(HeureCaRTextBox.Text))
            {
                SecondeCaRTextBox.Background = new SolidColorBrush(Color.FromArgb(255, 150, 0, 0));
                MinuteCaRTextBox.Background = new SolidColorBrush(Color.FromArgb(255, 150, 0, 0));
                HeureCaRTextBox.Background = new SolidColorBrush(Color.FromArgb(255, 150, 0, 0));
            }

            //si tout est ok on créer le timer et on l'envoi à MainPage pour l'envoi à ViewModel
            if (!ok) return;

            var nbSec = DateUtils.ConvertirHeureSecondes(heure, minutes, secondes);
            var car = new CaR
            {
                Titre = TitreCaRTextBox.Text,
                NbSecondes = nbSec,
                TempsDefaut = nbSec
            };
            //si c'est une modif, ont remet l'ID
            if (_idTimerModif > 0)
            {
                car.Id = _idTimerModif;
            }
            _ajoutTimer(car);
            //retour à MainPage
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void AnnulerCaRButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
        #endregion
        
    }
}
