using System;
using System.Globalization;
using System.Xml.Serialization;
using MultiCaRWin10.Utils;

namespace MultiCaRWin10.Model
{
    /// <summary>
    /// Un chrono / Compte a rebours / Alarme (définit par le numéro du mode)
    /// </summary>
    [XmlRoot("Chrono")]
    public class Chrono : Timer
    {
        /// <summary>
        /// l'id du chronomètre (celui pour un groupe et non pas son identifiant unique)
        /// </summary>
        [XmlElement("idChrono")]
        public int IdChrono { get; set; }
        
        /// <summary>
        /// indique s c'est un chrono ou si un arret de chrono pour un compte tours
        /// </summary>
        [XmlElement("isCompteTours")]
        public bool IsParent { get; set; }
        
        /// <summary>
        /// le temps de différence entre ce chrono et le précédent
        /// </summary>
        [XmlElement("nbSecondesDiff")]
        public long NbSecondesDiff { get; set; }

        /// <summary>
        /// indique si on doit afficher ou non le temps de différence
        /// </summary>
        [XmlElement("affichNbSecondesDiff")]
        public bool AffichNbSecondesDiff { get; set; }

        private string _titreTmp;

        public string TitreTmp
        {
            get { return _titreTmp; }

            set
            {
                _titreTmp = value;
                OnPropertyChanged("TitreTmp");
            }
        }

        [XmlIgnore]
        private bool _modifTitreVisible;

        /// <summary>
        /// Indique si oui ou non le titre est affiché pour être modifié
        /// </summary>

        [XmlIgnore]
        public bool ModifTitreVisible
        {
            get { return _modifTitreVisible; }

            set
            {
                _modifTitreVisible = value;
                OnPropertyChanged("ModifTitreVisible");
            }
        }

        /// <summary>
        /// indique si oui ou non il s'agit d'u chrono le plus rapide
        /// </summary>
        private bool _isFastestTimer;

        /// <summary>
        /// indique si oui ou non il s'agit d'u chrono le plus rapide
        /// </summary>
        public bool IsFastestTimer
        {
            get { return _isFastestTimer; }
            set
            {
                if (_isFastestTimer != value)
                {
                    _isFastestTimer = value;
                    OnPropertyChanged("IsFastestTimer");
                }
            }
        }


        /// <summary>
        /// Constructeur
        /// </summary>
        public Chrono()
        {
            ModifTitreVisible = false;
            TitreTmp = "";
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="chrono">le chrono à copier</param>
        public Chrono(Chrono chrono) : base(chrono)
        {
            IsParent = chrono.IsParent;
            IdChrono = chrono.IdChrono;
            ModifTitreVisible = false;
            TitreTmp = chrono.Titre;
        }

        /// <summary>
        /// Démarre un chronomètre
        /// </summary>
        public override void Start()
        {
            if (!EnPause) return;
            Date =  DateTime.Now - TimeSpan.FromMilliseconds(NbSecondes);
            NbSecondes = 0;
            EnPause = false;
        }
        
        /// <summary>
        /// Arrete un chronomètre
        /// </summary>
        public override void Stop()
        {
            if (EnPause) return;
            NbSecondes = DateUtils.IntervalleEntreDeuxDatesMs(Date, DateUtils.GetMaintenant());
            Date = new DateTime();
            EnPause = true;
        }

        /// <summary>
        /// Controle les informations du chrono et les corriges si nécéssaire
        /// </summary>
        public override void Control()
        {
           if (string.IsNullOrEmpty(Titre))
            {
                Titre = " ";
            }

            if (NbSecondes < 0)
            {
                NbSecondes = 0;
                EnPause = true;
                Date = new DateTime();
                Valider = false;
            }

            if (NbSecondes >= 99999999999999)
            {
                NbSecondes = 0;
            }

            DateTime d;
            if (!(DateTime.TryParse(Date.ToString(CultureInfo.InvariantCulture), out d)))
            {
                Date = new DateTime();
                NbSecondes = 0;
                EnPause = true;
                Valider = false;
            }
            else
            {
                if ((Date != new DateTime()) && DateUtils.IntervalleEntreDeuxDatesMs(Date, DateUtils.GetMaintenant()) >= 99999999999999)
                {
                    Date = new DateTime();
                    NbSecondes = 0;
                    EnPause = true;
                    Valider = false;
                }
            }
            
        }
    }
}
