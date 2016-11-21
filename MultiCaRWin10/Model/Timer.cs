using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace MultiCaRWin10.Model
{
    /// <summary>
    /// Un chrono / Compte a rebours / Alarme (définit par le numéro du mode)
    /// </summary>
    [XmlRoot("Timer")]
    public abstract class Timer : IComparable, INotifyPropertyChanged
    {
        #region Element du Timer
        /// <summary>
        /// id du timer
        /// </summary>
        [XmlElement("id")]
        public int Id { get; set; }

        /// <summary>
        /// Date de fin du compte a rebours (ou alarme) ou début pour un chrono
        /// </summary>
        [XmlElement("date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// nombre de secondes avant la fin du compte à rebours
        /// </summary>
        [XmlElement("nbSecondes")]
        public long NbSecondes { get; set; }

        /// <summary>
        /// nom du timer
        /// </summary>
        [XmlElement("titre")]
        public string Titre { get; set; }

        /// <summary>
        /// indique si le compte à rebours est en pause
        /// </summary>
        [XmlElement("enPause")]
        public bool EnPause { get; set; }

        /// <summary>
        /// indique si le compte à rebours a été validé
        /// </summary>
        [XmlElement("valider")]
        public bool Valider { get; set; }
        #endregion

        #region element Affichage
        /// <summary>
        /// le temps à affiché sur la View
        /// </summary>
        [XmlIgnore]
        private string _timeToAffich;

        /// <summary>
        /// La couleur du Timer pour l'affichage
        /// </summary>
        [XmlIgnore]
        private string _colorTimer;


        /// <summary>
        /// Propriété du temps à afficher dans la View
        /// </summary>
        [XmlIgnore]
        public string TimeToAffich
        {
            get { return _timeToAffich; }
            set
            {
                if (_timeToAffich == value) return;
                _timeToAffich = value;
                OnPropertyChanged("TimeToAffich");
            }
        }

        /// <summary>
        /// Propriété de la couleur à afficher
        /// </summary>
       [XmlIgnore]
        public string ColorTimer
        {
            get { return _colorTimer; }
            set
            {
                if (_colorTimer != value)
                {
                    _colorTimer = value;
                    OnPropertyChanged("ColorTimer");
                }
            }
        }

        #endregion

        #region Constructeurs et Surdéfinition

        /// <summary>
        /// Constructeur
        /// </summary>
        protected Timer()
        {
            Id = 0;
            Titre = "";
            Date = new DateTime();
            NbSecondes = 0;
            EnPause = true;
            Valider = false;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="timer">Timer à copier</param>
        protected Timer(Timer timer)
        {
            Id = timer.Id;
            Titre = timer.Titre;
            Date = timer.Date;
            NbSecondes = timer.NbSecondes;
            EnPause = timer.EnPause;
            Valider = timer.Valider;
        }


        /// <summary>
        /// Méthode pour comparer un chronomètre à un autre
        /// </summary>
        /// <param name="obj">un chronomètre</param>
        /// <returns>true si égaux</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Timer)obj);
        }

        /// <summary>
        /// Chrono toString
        /// </summary>
        /// <returns>le titre</returns>
        public override string ToString()
        {
            return Titre;
        }

        /// <summary>
        /// Méthode pour comparer un chronomètre à un autre
        /// </summary>
        /// <param name="other">le chronomètre</param>
        /// <returns>true si égaux</returns>
        private bool Equals(Timer other)
        {
            return Id == other.Id;
        }

        /// <summary>
        /// gethashcode
        /// </summary>
        /// <returns>hashcode ID</returns>
        public override int GetHashCode()
        {
            return Id;
        }

        /// <summary>
        /// Méthode utilisé par Sort pour le tri des chrono
        /// </summary>
        /// <param name="obj">le chrono à comparer</param>
        /// <returns>la valeur aidant au tri</returns>
        int IComparable.CompareTo(object obj)
        {
            var chrono = obj as Timer;
            return chrono != null ? string.Compare(chrono.Titre, Titre, StringComparison.Ordinal) : 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;


        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Outils
        /// <summary>
        /// inverse l'état d'activation du chrono
        /// </summary>
        public void ChangerEtatValidation()
        {
            Valider = !Valider;
        }
        #endregion

        #region Méthodes abstraites

        public abstract void Start();

        public abstract void Stop();

        public abstract void Control();

        #endregion
    }
}
