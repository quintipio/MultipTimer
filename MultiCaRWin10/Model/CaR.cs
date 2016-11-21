using System;
using System.Globalization;
using System.Xml.Serialization;
using Windows.ApplicationModel.Resources;
using MultiCaRWin10.Utils;

namespace MultiCaRWin10.Model
{
    [XmlRoot("CaR")]
    public class CaR : Timer
    {
        /// <summary>
        /// Temps par defaut tu timer en nombre de secondes (pour le reset)
        /// </summary>
        [XmlElement("tempsDefaut")]
        public long TempsDefaut { get; set; }

        /// <summary>
        /// Id de la notification associé
        /// </summary>
        [XmlElement("idNotif")]
        public int IdNotification { get; set; }

        public CaR()
        {
            TempsDefaut = 0;
        }


        /// <summary>
        /// Démarre le compte à rebours
        /// </summary>
        public override void Start()
        {
            if (!EnPause) return;
            Date = DateUtils.AjouterSecondeDate(DateUtils.GetMaintenant(), NbSecondes);
            NbSecondes = 0;
            EnPause = false;
            IdNotification = new Random().Next(0, 10000000);
            NotifUtils.LancerToast(ResourceLoader.GetForCurrentView().GetString("FinCaRText"), Titre, Date, IdNotification, App.ListTimer.MinuteRepeatToast, Convert.ToUInt32(App.ListTimer.NbRepeatToast));
        }

        /// <summary>
        /// Arrete le compte à rebours
        /// </summary>
        public override void Stop()
        {
            ArretCaR();
            NotifUtils.StopperToast(IdNotification);
        }

        /// <summary>
        /// Arrete un compte à rebours en laissant le toast
        /// </summary>
        public void StopAvecToast()
        {
            ArretCaR();
        }

        /// <summary>
        /// arreter un compte à rebours
        /// </summary>
        private void ArretCaR()
        {
            if (EnPause) return;
            NbSecondes = DateUtils.IntervalleEntreDeuxDatesSec(DateUtils.GetMaintenant(), Date);
            Date = new DateTime();
            EnPause = true;
        }

        /// <summary>
        /// Controle les données dans le compte à rebours
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

        /// <summary>
        /// réinitilialise le compte à rebours
        /// </summary>
        public void Reset()
        {
            Stop();
            NbSecondes = TempsDefaut;
            Valider = false;
        }


    }
}
