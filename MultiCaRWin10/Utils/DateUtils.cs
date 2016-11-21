using System;
using System.Collections.Generic;

namespace MultiCaRWin10.Utils
{
    /// <summary>
    /// Utilitaire pour gérer des dates
    /// </summary>
    public static class DateUtils
    {
        /// <summary>
        /// retourne l'objet DateTime à une heure précise
        /// </summary>
        /// <returns>La date précise</returns>
        public static DateTime GetMaintenant()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// Permet d'obtenir l'intervalle de jours entre deux dates
        /// </summary>
        /// <param name="oldDate"> la plus veille date</param>
        /// <param name="newDate">date la plus récente</param>
        /// <returns>Le nombre de secondes</returns>
        public static long IntervalleEntreDeuxDatesSec(DateTime oldDate, DateTime newDate)
        {
            var ts = newDate - oldDate;
            return (long)ts.TotalSeconds;
        }

        /// <summary>
        /// Additionne un nombre de secondes à une Date
        /// </summary>
        /// <param name="date">la date dé départ</param>
        /// <param name="secondes">le nombre de secondes à ajouter</param>
        /// <returns>la date obtenue</returns>
        public static DateTime AjouterSecondeDate(DateTime date, long secondes)
        {
            return date.AddSeconds(secondes);
        }

        /// <summary>
        /// converti des heures minutes secondes en seconde
        /// </summary>
        /// <param name="heure">les heures</param>
        /// <param name="minutes">les minutes</param>
        /// <param name="secondes">les secondes</param>
        /// <returns>le nombre de seconde totale</returns>
        public static long ConvertirHeureSecondes(int heure, int minutes, int secondes)
        {
            return (heure * 3600) + (minutes * 60) + secondes;
        }


        /// <summary>
        /// Converti un nombre de secondes en heure/minutes/secondes
        /// </summary>
        /// <param name="secondes">le nombre de seconde</param>
        /// <returns>List[2]= heure; List[1] = min; List[0] = s;</returns>
        public static List<long> ConvertirSecondesHeures(long secondes)
        {
            return new List<long> { secondes % 60, (secondes / 60) % 60, secondes / (60 * 60) };
        }


        /// <summary>
        /// permet d'obtenir la chaine de caractère pour un affichage correct dans chaque panel de chrono
        /// </summary>
        /// <param name="nbSecondes">le nombre de secondes à convertir en string XXhXXmXXs</param>
        /// <returns>la chaine de caractèrer</returns>
        public static string ConvertNbSecondesEnDateString(long nbSecondes)
        {
            var ts = TimeSpan.FromSeconds(nbSecondes);
            if (nbSecondes < 86400)
            {
                return ts.Hours + "h " + ts.Minutes + "m " + ts.Seconds + "s ";
            }
            return ts.Days + "d "+ ts.Hours + "h " + ts.Minutes + "m " + ts.Seconds + "s ";
        }

        /// <summary>
        /// soustrait deux dates et donne le temps de différence en XXhXXmXXsXXms
        /// </summary>
        /// <param name="oldDate">la première date</param>
        /// <param name="newDate">la deuxième date</param>
        /// <returns>la chaine de caractères</returns>
        public static string ConvertDifferenceDateenStringAvecMs(DateTime oldDate, DateTime newDate)
        {
            var ts = newDate - oldDate;
            return ts.Hours + "h " + ts.Minutes + "m " + ts.Seconds + "s " + ts.Milliseconds;
        }

        /// <summary>
        /// Converti un nombre de Milisecondes en temps à afficher en XXhXXmXXsXXms
        /// </summary>
        /// <param name="ms">le temps en milisecondes</param>
        /// <returns>la chaine de caractères</returns>
        public static string ConvertiNbSecondesEnStringAvecMs(long ms)
        {
            var ts = TimeSpan.FromMilliseconds(ms);
            //si c'est max une journéee ont affiche que les heures max
            if (ms < 86400000)
            {
                return ts.Hours + "h " + ts.Minutes + "m " + ts.Seconds + "s " + ts.Milliseconds;
            }
            return ts.Days+ "d "+ ts.Hours + "h " + ts.Minutes + "m " + ts.Seconds + "s " + ts.Milliseconds;
        }

        /// <summary>
        /// Permet d'obtenir l'intervalle de jours entre deux dates en millisecondes
        /// </summary>
        /// <param name="oldDate"> la plus veille date</param>
        /// <param name="newDate">date la plus récente</param>
        /// <returns>Le nombre de Milisecondes</returns>
        public static long IntervalleEntreDeuxDatesMs(DateTime oldDate, DateTime newDate)
        {
            var ts = newDate - oldDate;
            return (long)ts.TotalMilliseconds;
        }

        /// <summary>
        /// Converti une chaine de caractère au format XXhXXmXXsXXms en nombre de milisecondes
        /// </summary>
        /// <param name="temps">la chaine à convertir</param>
        /// <returns>le nombre de milisecondes</returns>
        public static long ConvertirStringEnNbMilisec(string temps)
        {
            int heureint;
            int minint;
            int secint;
            int msint;

            int.TryParse(temps.Substring(0, temps.IndexOf('h')), out heureint);
            var min = temps.Substring(temps.IndexOf('h') + 1, ((temps.IndexOf('m') - 1) - (temps.IndexOf('h'))));
            int.TryParse(min, out minint);
            var sec = temps.Substring(temps.IndexOf('m') + 1, ((temps.IndexOf('s') - 1) - (temps.IndexOf('m'))));
            int.TryParse(sec, out secint);
            var ms = temps.Substring(temps.IndexOf('s') + 1, ((temps.IndexOf("ms", StringComparison.Ordinal) - 1) - (temps.IndexOf('s'))));
            int.TryParse(ms, out msint);

            var time = new TimeSpan(heureint, minint, secint);
            var timeb = new TimeSpan(0, 0, 0, 0, msint);
            time.Add(timeb);

            return (long)time.TotalMilliseconds;
        }
    }
}
