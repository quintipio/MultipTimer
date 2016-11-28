using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Media;
using MultiCaRWin10.Com;
using MultiCaRWin10.Context;
using MultiCaRWin10.Utils;

namespace MultiCaRWin10.Model
{
    [XmlRoot("ListeTimer")]
    public class ListTimer : INotifyPropertyChanged
    {
        /// <summary>
        /// Fichier pour le chargement ou la sauvegarde (à ignorer pour la serialization)
        /// </summary>
        [XmlIgnore]
        private ComFile _fichierData;

        /// <summary>
        /// Liste des compte à rebours
        /// </summary>
        [XmlArray("listeCaR")] [XmlArrayItem("timerCaR")]
        private ObservableCollection<CaR> _listeCaR;

        /// <summary>
        /// Liste des chronomètres
        /// </summary>
        [XmlArray("listeChrono")] [XmlArrayItem("timerChrono")]
        private ObservableCollection<Chrono> _listeChrono;

        /// <summary>
        ///  Indique si oui ou non les Comptes à Rebours sont affichés en tri par temps
        /// </summary>
        [XmlElement("orderByTimer")]
        public bool OrderByTimer { get; set; }

        /// <summary>
        /// Nombre de minute d'intervalle avant la répétition d'un Toast
        /// </summary>
        [XmlElement("minuteRepeatTimer")]
        public int MinuteRepeatToast { get; set; }

        /// <summary>
        /// le nombre de fois que le toast se répétera
        /// </summary>
        [XmlElement("nbRepeatToast")]
        public int NbRepeatToast { get; set; }

        /// <summary>
        /// Numéro de la couleur de fond
        /// </summary>
        [XmlElement("CouleurFond")]
        public int CouleurFond { get; set; }


        /// <summary>
        /// Initialise les listes et le fichier à charger
        /// </summary>
        public void Init()
        {
            _listeCaR = new ObservableCollection<CaR>();
            _listeChrono = new ObservableCollection<Chrono>();
            _fichierData = new ComFile(ContextStatic.NomFichier + "." + ContextStatic.Extension, ComFile.PlaceEcriture.LocalState); 
        }

        /// <summary>
        /// Retourne la couleur de fond d'écran de l'appli
        /// </summary>
        /// <returns>la couleur  à appliquer</returns>
        public SolidColorBrush GetBackGroundColor()
        {
            if (CouleurFond >= ContextStatic.ListeCouleur.Count)
            {
                CouleurFond = 0;
            }
            var color = ContextStatic.ListeCouleur[CouleurFond];
                var hex = string.Format("{0:X}", color);
                return
                    new SolidColorBrush(Color.FromArgb(byte.Parse(hex.Substring(0, 2), NumberStyles.AllowHexSpecifier),
                        byte.Parse(hex.Substring(2, 2), NumberStyles.AllowHexSpecifier),
                        byte.Parse(hex.Substring(4, 2), NumberStyles.AllowHexSpecifier),
                        byte.Parse(hex.Substring(6, 2), NumberStyles.AllowHexSpecifier)));
        }

        #region Proriétés

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Liste des comptes à rebours
        /// </summary>
        public ObservableCollection<CaR> ListeCaR
        {
            get { return _listeCaR; }
            set
            {
                if (_listeCaR != value)
                {
                    _listeCaR = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// liste des chronomètres
        /// </summary>
        public ObservableCollection<Chrono> ListeChronos
        {
            get { return _listeChrono; }
            set
            {
                if (_listeChrono != value)
                {
                    _listeChrono = value;
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
        
        #region Modification des paramètres
        /// <summary>
        /// Change la façon dont sont trier les timers
        /// </summary>
        public void ChangerOrdreTri()
        {
            OrderByTimer = !OrderByTimer;
        }

        /// <summary>
        /// tri la liste des Comptes à rebours par pordre alphabétique ou par ordre de temps restant
        /// </summary>
        public void TrierListeCaR()
        {
            var listeTri = new List<CaR>();
            //si le tri est par Timer on le fait par LINQ
            if (OrderByTimer)
            {
                listeTri.AddRange(ListeCaR.Where(c => !c.EnPause).OrderBy(c => c.Date).ToList());
                listeTri.AddRange(ListeCaR.Where(c => c.EnPause).OrderBy(c => c.NbSecondes).ToList());
            }
            //sinon (par titre) on le fait par le CompareTo de Chrono
            else
            {
                listeTri = (from item in ListeCaR
                                orderby item.Titre
                                select item).ToList(); 
                listeTri.Sort();
                listeTri.Reverse();
            }
            ListeCaR.Clear();
            foreach (var t in listeTri)
            {
                ListeCaR.Add(t);
            }
        }

        #endregion

        #region gestion des timers

        /// <summary>
        /// ajoute un timer à sa liste
        /// </summary>
        /// <param name="chrono"></param>
        public void AjouterModifier(Timer chrono)
        {
            if (chrono is CaR)
            {
                var car = chrono as CaR;

                if (ListeCaR.Contains(car))
                {
                    var index = ListeCaR.IndexOf(car);
                    ListeCaR.Remove(car);
                    ListeCaR.Insert(index,car);
                }
                else //sinon on ajoute
                {
                    //récup de l'id max et ajoute à la liste
                    var res = (ListeCaR.Count == 0) ? 0 : _listeCaR.OrderByDescending(i => i.Id).FirstOrDefault().Id;
                    car.Id = res + 1;
                    ListeCaR.Add(car);
                }
            }

            if (chrono is Chrono)
            {
                var ch = chrono as Chrono;
                //si il n'a pas d'id, c'est un nouveau chrono
                if (ch.Id == 0)
                {
                    ch.Id = ((ListeChronos.Count == 0)
                        ? 0
                        : ListeChronos.OrderByDescending(i => i.Id).FirstOrDefault().Id) + 1;
                    ch.IdChrono = ((ListeChronos.Count == 0)
                        ? 0
                        : ListeChronos.OrderByDescending(i => i.IdChrono).FirstOrDefault().IdChrono) + 1;
                    ch.Valider = false;
                    ch.AffichNbSecondesDiff = false;
                    ch.IsParent = true;
                    ch.Stop();
                    ListeChronos.Add(ch);

                }
                //si il a un Id, ont l'ajoute à la suite des Id Chrono
                else
                {
                    ch.Id = ((ListeChronos.Count == 0)? 0: ListeChronos.OrderByDescending(i => i.Id).FirstOrDefault().Id) + 1;
                    ch.Stop();
                    
                    //récupération ddu temps du chrono précédent (id max de l'idChrono)
                    ch.NbSecondesDiff = ch.NbSecondes - ListeChronos.Where(listeChrono => listeChrono.IdChrono == ch.IdChrono).OrderByDescending(i => i.Id).FirstOrDefault().NbSecondes;

                    //on vérifie si c'est le le chrono le plus rapide, et si c'est le cas on met à jour les données
                    var res = ListeChronos.Count(listeChrono => listeChrono.IdChrono == ch.IdChrono && !listeChrono.IsParent && listeChrono.NbSecondesDiff < ch.NbSecondesDiff );
                    if (res == 0)
                    {
                        ch.IsFastestTimer = true;

                        foreach (var item in ListeChronos.Where(item => item.IdChrono == ch.IdChrono))
                        {
                            item.IsFastestTimer = false;
                        }
                    }

                    ch.AffichNbSecondesDiff = true;
                    ch.IsParent = false;
                    var tt = ListeChronos.FirstOrDefault(listeChrono => listeChrono.IdChrono == ch.IdChrono && listeChrono.IsParent);
                    var indexOri = ListeChronos.IndexOf(tt);
                    ListeChronos.Insert(indexOri+1,ch);
                }

            }
                  
        }

        /// <summary>
        /// Supprime un timer de sal liste
        /// </summary>
        /// <param name="chrono">le timer à supprimer</param>
        public void Supprimer(Timer chrono)
        {
            if (chrono is CaR)
            {
                ListeCaR.Remove((CaR)chrono);
            }

            if (chrono is Chrono)
            {
                var ch = chrono as Chrono;
                foreach (var c in ListeChronos.ToList().Where(c => c.IdChrono == ch.IdChrono))
                {
                    ListeChronos.Remove(c);
                }
            }
        }

        /// <summary>
        /// Modifie le titre d'un chronomètre
        /// </summary>
        /// <param name="chrono">le chronomètre à modifier</param>
        /// <param name="titre">le nouveau titre</param>
        public void ModifierTitreChrono(Chrono chrono,string titre)
        {
            if (chrono.IsParent)
            {
                var liste = _listeChrono.Where(x => x.IdChrono == chrono.IdChrono).ToList();
                foreach (var ch in liste)
                {
                    ch.Titre = titre;
                    ch.TitreTmp = titre;
                }
            }
            else
            {
                chrono.Titre = titre;
                chrono.TitreTmp = titre;
            }
        }
        #endregion

        #region Sauvegarde et chargement du fichier

        /// <summary>
        /// Compte le nombre de chronomètre et de compte à rebours actif pour l'afficher sur la tuile
        /// </summary>
        private void MajBadge()
        {
            int compteur;
            compteur = ListeCaR.Count(caR => !caR.EnPause);
            compteur += ListeChronos.Count(chrono => !chrono.EnPause);

            if (compteur != 0)
            {
                NotifUtils.AfficherChiffreBadge(compteur);
            }
            else
            {
                NotifUtils.EffacerBadge();
            }
        }


        /// <summary>
        /// sauvegarde les comptes à rebours dans le fichier
        /// </summary>
        public async Task SaveFile()
        {
            //serialization et écriture dans le fichier
            var xs = new XmlSerializer(typeof(ListTimer));
            using (var wr = new StringWriter())
            {
                xs.Serialize(wr, this);
                if (_fichierData != null)
                {
                   await _fichierData.Ecrire(wr.ToString(), CreationCollisionOption.ReplaceExisting, true);
                    MajBadge();
                }
            }
        }

        /// <summary>
        /// Charge le fichier des timers
        /// </summary>
        public async Task<bool> LoadFile()
        {
            try
            {
                //lecture et déserialization
                var xsb = new XmlSerializer(typeof(ListTimer));
                if (await _fichierData.FileExist() && ListeCaR != null &&  ListeChronos != null && ListeCaR.Count <= 0 && ListeChronos.Count <= 0)
                {
                    using (var rd = new StringReader(await _fichierData.LireString()))
                    {
                        var listTmp = xsb.Deserialize(rd) as ListTimer;
                        if (listTmp != null)
                        {
                            _listeCaR = new ObservableCollection<CaR>(listTmp._listeCaR);
                            _listeChrono = new ObservableCollection<Chrono>(listTmp._listeChrono);

                            var emplacement = 0;

                            foreach (var c in _listeCaR)
                            {
                                emplacement++;
                                //control des données de chaque compte à rebours
                                c.Control();

                                //on controle et corrige les éventuels doublon d'id
                                for (var i = emplacement; i < _listeCaR.Count; i++)
                                {
                                    if (_listeCaR[i].Id == c.Id)
                                    {
                                        _listeCaR[i].Id = ((_listeCaR.Count == 0)
                                            ? 0
                                            : _listeCaR.OrderByDescending(s => s.Id).FirstOrDefault().Id) + 1;
                                    }
                                }
                            }

                            foreach (var c in _listeChrono)
                            {
                                emplacement++;
                                //control des données de chaque compte à rebours
                                c.Control();

                                //on controle et corrige les éventuels doublon d'id
                                for (var i = emplacement; i < _listeCaR.Count; i++)
                                {
                                    if (_listeCaR[i].Id == c.Id)
                                    {
                                        _listeCaR[i].Id = ((_listeCaR.Count == 0)
                                            ? 0
                                            : _listeCaR.OrderByDescending(s => s.Id).FirstOrDefault().Id) + 1;
                                    }
                                }
                            }

                            OrderByTimer = listTmp.OrderByTimer;
                            MinuteRepeatToast = listTmp.MinuteRepeatToast;
                            NbRepeatToast = listTmp.NbRepeatToast;
                            CouleurFond = listTmp.CouleurFond;

                        }
                        else
                        {
                            _listeCaR = new ObservableCollection<CaR>();
                            _listeChrono = new ObservableCollection<Chrono>();
                        }
                    }   
                }
            }
            catch
            {
                //en cas d'erreur on réinitilise tout
                ListeCaR = new ObservableCollection<CaR>();
                _listeChrono = new ObservableCollection<Chrono>();
                OrderByTimer = false;
                return false;
            }
            TrierListeCaR();
            MajBadge();
            return true;
        }
        #endregion
    }
}
