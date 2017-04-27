using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using MultiCaRWin10.Context;
using MultiCaRWin10.Model;
using MultiCaRWin10.Utils;

namespace MultiCaRWin10.ViewModel
{
    /// <summary>
    /// Controleur e la vue principale
    /// </summary>
    public class MainPageViewModel
    {
        /// <summary>
        /// La liste des Timers
        /// </summary>
        private ListTimer _listTimer;
        
        /// <summary>
        /// Timer pour vérifier chaque Timer
        /// </summary>
        private DispatcherTimer _timer;

        /// <summary>
        /// Delegate pour envoyer à la page de gestion des timer
        /// </summary>
        /// <param name="car">le timer à modifier</param>
        public delegate Task DelegateAjoutTimer(Timer car);

        /// <summary>
        /// Pour le changement des propriétés
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Constructeur
        /// </summary>
        public MainPageViewModel()
        {
            //démarrage du timer de Control
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(ContextStatic.IntervalTimerMs) };
            _timer.Tick += timer_Tick;
            _timer.Start();

            ListTimer = App.ListTimer;

            //mise en place des couleurs
            foreach (var timer in ListTimer.ListeCaR)
            {
                GetColorEtatChrono(timer);
            }

            foreach (var timer in ListTimer.ListeChronos)
            {
                GetColorEtatChrono(timer);
            }
        }

        #region propriété

        public ListTimer ListTimer
        {
            get { return _listTimer; }
            private set
            {
                if (_listTimer != value)
                {
                    _listTimer = value;
                    OnPropertyChanged();
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Outils
        // Tick du Timer pour mettre à jour l'affichage
        private async void timer_Tick(object sender, object e)
        {
            //pour chaque Compte à rebours
            if (ListTimer.ListeCaR != null)
            {
                foreach (var timerCar in ListTimer.ListeCaR.ToList())
                {
                    //on affiche le temps
                    timerCar.TimeToAffich = GetTempsString(timerCar);
                    //si la lecture vient de finir
                    if (!(DateTime.Compare(timerCar.Date, DateUtils.GetMaintenant()) >= 0) && !timerCar.EnPause)
                    {
                        //on arrete le timer
                        await StopCaR(timerCar,true);
                    }
                }

                foreach (var chrono in ListTimer.ListeChronos.ToList())
                {
                    chrono.TimeToAffich = GetTempsString(chrono);
                }
            }
        }

        
        

        /// <summary>
        /// Retourne le temps à afficher en fonction du mode de fonctionnement du Timer
        /// </summary>
        /// <param name="chrono">le chrono dont ont souhaite récupérer la donnée</param>
        /// <returns>le temps à afficher</returns>
        private static string GetTempsString(Timer chrono)
        {
            if (chrono is CaR)
            {
                if (chrono.EnPause)
                {
                    return DateUtils.ConvertNbSecondesEnDateString(chrono.NbSecondes > 0 ? chrono.NbSecondes : 0);
                }
                var res = DateUtils.IntervalleEntreDeuxDatesSec(DateUtils.GetMaintenant(), chrono.Date);
                return DateUtils.ConvertNbSecondesEnDateString(res > 0 ? res : 0);
            }

            if (chrono is Chrono)
            {
                return (chrono.EnPause) ?
                        DateUtils.ConvertiNbSecondesEnStringAvecMs(chrono.NbSecondes) :
                        DateUtils.ConvertDifferenceDateenStringAvecMs(chrono.Date, DateUtils.GetMaintenant());
            }
            return "";
        }

        /// <summary>
        /// Déternine l'état d'un timer et fonne la couleur à afficher
        /// </summary>
        /// <param name="chrono">le chrono à traiter</param>
        private static void GetColorEtatChrono(Timer chrono)
        {
            //couleur par défaut
            chrono.ColorTimer = "White";
            if (chrono is CaR)
            {
                 //si valider
                    if (chrono.Valider)
                    {
                        chrono.ColorTimer = "SteelBlue";
                    }
                    else
                    {
                        //si en lecture
                        if (!chrono.EnPause && chrono.Date != new DateTime() && chrono.NbSecondes == 0)
                        {
                            chrono.ColorTimer = "OliveDrab";
                        }

                        //si le chrono est en pause
                        if (chrono.EnPause && chrono.Date == new DateTime() && chrono.NbSecondes >= 0)
                        {
                            chrono.ColorTimer = "Tomato";
                        }

                        //si le chrono est fini
                        if (chrono.EnPause && chrono.Date == new DateTime() && chrono.NbSecondes <= 0)
                        {
                            chrono.ColorTimer = "Red";
                        }
                    }
            }

            if ((chrono is Chrono))
            {
                var ch = chrono as Chrono;
                if (ch.Valider)
                {
                    chrono.ColorTimer = "SteelBlue";
                }
                else
                {
                    if (ch.IsParent)
                    {
                        chrono.ColorTimer = chrono.EnPause ? "Tomato" : "OliveDrab";
                    }
                    else
                    {
                        chrono.ColorTimer = "Red";
                    }
                }   
            }
        }


        /// <summary>
        /// Ajoute un Timer à la liste
        /// </summary>
        /// <param name="car">le nouveau Timer à ajouter</param>
        public async Task SaveChrono(Timer car)
        {
            ListTimer.AjouterModifier(car);
            GetColorEtatChrono(car);
            ListTimer.TrierListeCaR();
            await ListTimer.SaveFile();
            }

        #endregion

        #region Gestion des comptes à rebours
        
        /// <summary>
        /// Lance un Timer
        /// </summary>
        /// <param name="chrono">Le timer à lancer</param>
        public async Task StartCaR(CaR chrono)
        {
            if (chrono.EnPause && chrono.Date == new DateTime() && chrono.NbSecondes <= 0) return;
            chrono.Start();
            GetColorEtatChrono(chrono);
            ListTimer.TrierListeCaR();
            await ListTimer.SaveFile();
        }

        /// <summary>
        /// arrete le Timer
        /// </summary>
        /// <param name="chrono">Le timer à arreté</param>
        /// <param name="affichToast">indique si le toast dalerte doit être laisser ou non</param>
        public async Task StopCaR(CaR chrono,bool affichToast)
        {
            if (affichToast)
            {
                chrono.StopAvecToast();
            }
            else
            {
                chrono.Stop();
            }
            GetColorEtatChrono(chrono);
            ListTimer.TrierListeCaR();
            await ListTimer.SaveFile();
        }

        /// <summary>
        /// reinitialise un Timer
        /// </summary>
        /// <param name="chrono">le timer à réinitialisé</param>
        public async Task ResetCaR(CaR chrono)
        {
            chrono.Reset();
            GetColorEtatChrono(chrono);
            ListTimer.TrierListeCaR();
            await ListTimer.SaveFile();
        }

        /// <summary>
        /// change l'état de validation d'un timer
        /// </summary>
        /// <param name="chrono">le timer à changer</param>
        public async Task ValidCaR(CaR chrono)
        {
            chrono.ChangerEtatValidation();
            GetColorEtatChrono(chrono);
            ListTimer.TrierListeCaR();
            await ListTimer.SaveFile();
        }

        /// <summary>
        /// Supprime un chrono de sa liste
        /// </summary>
        /// <param name="chrono">le chrono à supprimer</param>
        public async Task SupCaR(CaR chrono)
        {
            chrono.Stop();
            ListTimer.Supprimer(chrono);
            ListTimer.TrierListeCaR();
            await ListTimer.SaveFile();
        }

        /// <summary>
        /// Lance un tri des timers du comtpe à rebours soit par titre, soit par temps
        /// </summary>
        public async Task SortCaR()
        {
            ListTimer.ChangerOrdreTri();
            ListTimer.TrierListeCaR();
            await ListTimer.SaveFile();
        }
        #endregion

        #region Gestion des chronos

        /// <summary>
        /// Ajoute un chronomètre
        /// </summary>
        public async Task AddChrono()
        {
            var chrono = new Chrono {Id = 0};
            ListTimer.AjouterModifier(chrono);
            GetColorEtatChrono(chrono);
            await ListTimer.SaveFile();
        }
        /// <summary>
        /// Démarrage du chronomètre
        /// </summary>
        public async Task StartChrono(Chrono chrono)
        {
            chrono.Start();
            GetColorEtatChrono(chrono);
            await ListTimer.SaveFile();
        }
        
        /// <summary>
        /// Mise en pause du chronomètre
        /// </summary>
        /// <param name="chrono">le chronomètre dont l"état doit changer</param>
        public async Task PauseChrono(Chrono chrono)
        {
            chrono.Stop();
            GetColorEtatChrono(chrono);
            await ListTimer.SaveFile();
        }

        
        /// <summary>
        /// Ajout d'un temps à la liste
        /// </summary>
        /// <param name="chrono">le chronomètre à copier</param>
        public async Task SaveChrono(Chrono chrono)
        {
            if (ListTimer.ListeChronos.Count <= 0) return;
            var ch = new Chrono(chrono);
            ListTimer.AjouterModifier(ch);
            GetColorEtatChrono(ch);
            await ListTimer.SaveFile();
        }

        /// <summary>
        /// valide ou non un chronomètre
        /// </summary>
        /// <param name="chrono">le chronomètre dont l"état doit changer</param>
        public async Task ValidChrono(Chrono chrono)
        {
            if (chrono == null) return;
            chrono.ChangerEtatValidation();
            GetColorEtatChrono(chrono);
            await ListTimer.SaveFile();
        }

        /// <summary>
        /// Supprimer un chronomètre et sa liste de temps associées
        /// </summary>
        /// <param name="chrono">le chronomètre à supprimer</param>
        public async Task SupChrono(Chrono chrono)
        {
            if (chrono == null) return;
            ListTimer.Supprimer(chrono);
            await ListTimer.SaveFile();
        }

        /// <summary>
        /// Réinitialise la liste des chronomètres
        /// </summary>
        public async Task ResetChrono()
        {
            ListTimer.ListeChronos.Clear();
            await ListTimer.SaveFile();
        }

        /// <summary>
        /// Remet à zéro un chronomètre
        /// </summary>
        /// <param name="chrono"></param>
        /// <returns></returns>
        public async Task ResetChronoTime(Chrono chrono)
        {
            await PauseChrono(chrono);

            var liste = ListTimer.ListeChronos.Where(x => x.IdChrono == chrono.IdChrono && x.IsParent == false).ToList();
            foreach (var ch in liste)
            {
                ListTimer.ListeChronos.Remove(ch);
            }
            chrono.ResetTime();
            await ListTimer.SaveFile();
        }

        /// <summary>
        /// Modifie le titre d'un chrono et de tous ses sous chronos
        /// </summary>
        /// <param name="chrono">le chrono dont le titre est à modifier</param>
        public async Task ChangerNomChrono(Chrono chrono)
        {
            if (chrono == null) return;
            ListTimer.ModifierTitreChrono(chrono,chrono.TitreTmp);
            await ListTimer.SaveFile();
            chrono.ModifTitreVisible = false;
        }

        /// <summary>
        ///Modifie la vue pour afficher la partie de modification du titre du chrono
        /// </summary>
        /// <param name="chrono">le chrono dont le titre est à modifier</param>
        public void OpenChangerNomChrono(Chrono chrono)
        {
            chrono.ModifTitreVisible = true;
        }
        #endregion
    }
}
