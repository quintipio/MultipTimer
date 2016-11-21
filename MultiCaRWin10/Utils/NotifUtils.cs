using System;
using System.Linq;
using Windows.UI.Notifications;
using MultiCaRWin10.Notifications;

namespace MultiCaRWin10.Utils
{
    /// <summary>
    /// Classe d'utilitaire pour gérer les notitifcations toasts
    /// </summary>
    public static class NotifUtils
    {
        /// <summary>
        /// Permet d'enregistrer un toast à afficher à une date précise
        /// </summary>
        /// <param name="titre">le titre du Toast</param>
        /// <param name="phrase">La phrase du toast</param>
        /// <param name="activationTime">la date d'activation</param>
        /// <param name="idNotif">l'id du toast</param>
        /// <param name="minute">le nombre de minutes avant une répétition (0 pour aucun)</param>
        /// <param name="repeat">le nombre de répétition (0 pour aucun)</param>
        public static void LancerToast(string titre, string phrase, DateTime activationTime, int idNotif, int minute, uint repeat)
        {
            //création du toast
            var toastContent = ToastContentFactory.CreateToastText02();
            toastContent.TextHeading.Text = titre;
            toastContent.TextBodyWrap.Text = phrase;
            ScheduledToastNotification toast;
            if (minute > 0 && repeat > 0 )
            {
                toast = new ScheduledToastNotification(toastContent.GetXml(), activationTime,TimeSpan.FromMinutes(minute),repeat) { Id = idNotif.ToString() };
            }
            else
            {
                toast = new ScheduledToastNotification(toastContent.GetXml(), activationTime) { Id = idNotif.ToString() };
            }
            ToastNotificationManager.CreateToastNotifier().AddToSchedule(toast);
        }

        /// <summary>
        /// Arreter un toast
        /// </summary>
        /// <param name="idToast">l'id du toast à arreter</param>
        public static void StopperToast(int idToast)
        {
            var toastNotifier = ToastNotificationManager.CreateToastNotifier();
            var scheduledToasts = toastNotifier.GetScheduledToastNotifications();
            foreach (var scheduledToastNotification in scheduledToasts.Where(scheduledToastNotification => scheduledToastNotification.Id == idToast.ToString()))
            {
                toastNotifier.RemoveFromSchedule(scheduledToastNotification);
            }
        }

        /// <summary>
        /// Affiche un nombre compris entre 0 et 99 dans la tuile
        /// </summary>
        /// <param name="number"></param>
        public static void AfficherChiffreBadge(int number)
        {
            // Create a string with the badge template xml.
            var badgeXmlString = "<badge value='" + number + "'/>";
            var badgeDom = new Windows.Data.Xml.Dom.XmlDocument();
            try
            {
                // Create a DOM.
                badgeDom.LoadXml(badgeXmlString);

                // Load the xml string into the DOM, catching any invalid xml characters.
                var badge = new BadgeNotification(badgeDom);

                // Create a badge notification and send it to the application’s tile.
                BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);

            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// réinitialise le badge
        /// </summary>
        public static void EffacerBadge()
        {
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();
        }
    }
}
