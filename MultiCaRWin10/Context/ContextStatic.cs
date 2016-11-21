using System.Collections.Generic;

namespace MultiCaRWin10.Context
{
    /// <summary>
    /// variable de l'appli modifiable uniquement par le développeur
    /// </summary>
    public static class ContextStatic
    {
        /// <summary>
        /// le nom de l'application
        /// </summary>
        public const string NomAppli = "Multip'Timer";

        /// <summary>
        /// adresse de support
        /// </summary>
        public const string Support = "xxxxx@xxx.xx";

        /// <summary>
        /// version de l'application
        /// </summary>
        public const string Version = "2.0.1";

        /// <summary>
        /// nom du développeur
        /// </summary>
        public const string Developpeur = "XXXXXXX";

        /// <summary>
        /// le nom par défaut du fichier 
        /// </summary>
        public const string NomFichier = "listeChrono";

        /// <summary>
        /// l'extension par défaut
        /// </summary>
        public const string Extension = "car";
        
        /// <summary>
        /// Intervalle en Ms entre deux éxécution du Timer
        /// </summary>
        public const int IntervalTimerMs = 50;

        /// <summary>
        /// liste des couleurs pour le fond de l'appli
        /// </summary>
        public static readonly List<uint> ListeCouleur = new List<uint> { 0xFFFFFFFF, 0xFF895A5A, 0xFF536434, 0xFF346464, 0xFF344364, 0xFF4C3464, 0xFF64344E };
    }
}
