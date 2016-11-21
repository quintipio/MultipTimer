using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using MultiCaRWin10.Context;
using MultiCaRWin10.Strings;

namespace MultiCaRWin10.Views
{
    /// <summary>
    /// Vue pour la gestion des toasts
    /// </summary>
    public sealed partial class ParamToastPage : INotifyPropertyChanged
    {

        #region propriétés
        private bool _canChangeLangue;

        private ObservableCollection<ListeLangues.LanguesStruct> _listeLangue;

        public ObservableCollection<ListeLangues.LanguesStruct> ListeLangue
        {
            get { return _listeLangue; }

            set
            {
                _listeLangue = value;
                OnPropertyChanged();
            }
        }

        private ListeLangues.LanguesStruct _selectedLangue;

        public ListeLangues.LanguesStruct SelectedLangue
        {
            get { return _selectedLangue; }

            set
            {
                _selectedLangue = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// pour le InotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public ParamToastPage()
        {
            InitializeComponent();
            _canChangeLangue = false;
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AppNomAppTextBlock.Text = ContextStatic.NomAppli;
            Background = App.ListTimer.GetBackGroundColor();

            TempsRepeatTextBox.Text = App.ListTimer.MinuteRepeatToast.ToString();
            NbRepeatTextBox.Text = App.ListTimer.NbRepeatToast.ToString();

            ListeLangue = new ObservableCollection<ListeLangues.LanguesStruct>(ListeLangues.GetListesLangues());

            //init du tableau de couleurs
            var listeCouleur = new ObservableCollection<SolidColorBrush>();
            foreach (var couleur in ContextStatic.ListeCouleur)
                listeCouleur.Add(GetColor(couleur));
            ListeColorGridView.ItemsSource = listeCouleur;
            _canChangeLangue = true;

        }

        private void CloseButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
        

        private async void AppValidButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            int resA;
            int resB;
            var okTemps = false;
            var okRepeat = false;
            if (!int.TryParse(TempsRepeatTextBox.Text, out resA))
            {
                TempsRepeatTextBox.Background = new SolidColorBrush(Color.FromArgb(255, 150, 0, 0));
            }
            else
            {
                if (resA < 0)
                {
                    TempsRepeatTextBox.Background = new SolidColorBrush(Color.FromArgb(255, 150, 0, 0));
                }
                else
                {
                    TempsRepeatTextBox.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    okTemps = true;
                }
            }

            if (!int.TryParse(NbRepeatTextBox.Text, out resB))
            {
                NbRepeatTextBox.Background = new SolidColorBrush(Color.FromArgb(255, 150, 0, 0));
            }
            else
            {
                if (resB < 0)
                {
                    NbRepeatTextBox.Background = new SolidColorBrush(Color.FromArgb(255, 150, 0, 0));
                }
                else
                {
                    NbRepeatTextBox.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    okRepeat = true;
                }
            }

            if (okRepeat && okTemps)
            {
                App.ListTimer.MinuteRepeatToast = resA;
                App.ListTimer.NbRepeatToast = resB;
                await App.ListTimer.SaveFile();
            }

            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }


        /// <summary>
        ///     Retourne le solidColorBrush à appliquer à un rectangle à partir de son code couleur
        /// </summary>
        /// <param name="color">la couleur</param>
        /// <returns></returns>
        private static SolidColorBrush GetColor(uint color)
        {
            var hex = string.Format("{0:X}", color);
            return new SolidColorBrush(Color.FromArgb(byte.Parse(hex.Substring(0, 2), NumberStyles.AllowHexSpecifier),
                byte.Parse(hex.Substring(2, 2), NumberStyles.AllowHexSpecifier),
                byte.Parse(hex.Substring(4, 2), NumberStyles.AllowHexSpecifier),
                byte.Parse(hex.Substring(6, 2), NumberStyles.AllowHexSpecifier)));
        }

        //change la couleur du thème
        private async void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var solidColor = ((SolidColorBrush)((Rectangle)sender).Tag);
            var color =
                (uint)
                    ((solidColor.Color.A << 24) | (solidColor.Color.R << 16) | (solidColor.Color.G << 8) |
                     (solidColor.Color.B << 0));

            App.ListTimer.CouleurFond = ContextStatic.ListeCouleur.IndexOf(color);
            await App.ListTimer.SaveFile();
            Background = solidColor;
        }


        #region langue

        private void SelectLangueCombo()
        {
            SelectedLangue = ListeLangues.GetLangueEnCours();
            ComboListeLangue.SelectedIndex = SelectedLangue.Id - 1;
        }

        //change la langue de l'appli à partir de la comboBox
        private void comboListeLangue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_canChangeLangue && ComboListeLangue.SelectedItem is ListeLangues.LanguesStruct)
            {
                _canChangeLangue = false;
                SelectedLangue = (ListeLangues.LanguesStruct)ComboListeLangue.SelectedItem;
                ListeLangues.ChangeLangueAppli(SelectedLangue);
                _canChangeLangue = true;
            }
        }
        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SelectLangueCombo();
        }

        #endregion

    }
}
