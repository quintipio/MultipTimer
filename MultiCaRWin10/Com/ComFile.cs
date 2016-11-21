using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MultiCaRWin10.Com
{
    /// <summary>
    /// Classe permettant de gérer un fichier
    /// </summary>
    public class ComFile
    {
        /// <summary>
        /// enum pou localiser dans quel dossier est situé le fichier
        /// </summary>
        public enum PlaceEcriture{
            LocalState = 1,
            Roaming = 2,
        }

        private readonly String _path;
        private readonly PlaceEcriture _placeEcriture;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="path">le chemin du fichier</param>
        /// <param name="placeEcriture">son répertoire</param>
        public ComFile(String path, PlaceEcriture placeEcriture)
        {
            _path = path;
            _placeEcriture = placeEcriture;
        }

        /// <summary>
        /// ecrit un byte array dans un fichier
        /// </summary>
        /// <param name="data">les donénes à écrire</param>
        /// <param name="mode">le mode d'ouverture du fichier</param>
        /// <returns>true si ok</returns>
        public async Task<bool> Ecrire(byte[] data, CreationCollisionOption mode)
        {
            var retour = false;
            StorageFile file = null;
            switch(_placeEcriture)
            {
                case PlaceEcriture.LocalState:
                    file = await ApplicationData.Current.LocalFolder.CreateFileAsync(_path, mode);
                    break;

                case PlaceEcriture.Roaming:
                    file = await ApplicationData.Current.RoamingFolder.CreateFileAsync(_path, mode);
                    break;
            }
            if(file != null)
            {
                await FileIO.WriteBytesAsync(file, data);
                retour = true;
            }
            return retour;
        }

        /// <summary>
        /// ecrit unne chaine de caractère dans un fichier
        /// </summary>
        /// <param name="data">la chaine de caractère à écrire</param>
        /// <param name="mode">le mode d'ouverture du fichier</param>
        /// <param name="ecraser">ecrase le fichier ou rajoute à la suite</param>
        /// <returns>true si ok</returns>
        public async Task<bool> Ecrire(String data, CreationCollisionOption mode, bool ecraser)
        {
            var retour = false;
            StorageFile file = null;
            switch (_placeEcriture)
            {
                case PlaceEcriture.LocalState:
                    file = await ApplicationData.Current.LocalFolder.CreateFileAsync(_path, mode);
                    break;

                case PlaceEcriture.Roaming:
                    file = await ApplicationData.Current.RoamingFolder.CreateFileAsync(_path, mode);
                    break;
            }

            if (file != null)
            {
                if (ecraser)
                {
                    await FileIO.WriteTextAsync(file, data, UnicodeEncoding.Utf8);
                }
                else
                {
                    await FileIO.AppendTextAsync(file, data, UnicodeEncoding.Utf8);
                }
                retour = true;
            }
            return retour;
        }

        /// <summary>
        /// écrit une liste de string dans un fichier
        /// </summary>
        /// <param name="data">les données à écrire</param>
        /// <param name="mode">le mode d'ouverture du fichier</param>
        /// <param name="ecraser">écrase le fichier ou rajotue à la suite</param>
        /// <returns>true si ok</returns>
        public async Task<bool> Ecrire(List<String> data, CreationCollisionOption mode,bool ecraser)
        {
            var retour = false;
            StorageFile file = null;
            switch (_placeEcriture)
            {
                case PlaceEcriture.LocalState:
                    file = await ApplicationData.Current.LocalFolder.CreateFileAsync(_path, mode);
                    break;

                case PlaceEcriture.Roaming:
                    file = await ApplicationData.Current.RoamingFolder.CreateFileAsync(_path, mode);
                    break;
            }

            if (file != null)
            {
                if (ecraser)
                {
                    await FileIO.WriteLinesAsync(file, data, UnicodeEncoding.Utf8);
                }
                else
                {
                    await FileIO.AppendLinesAsync(file, data, UnicodeEncoding.Utf8);
                }
                retour = true;
            }
            return retour;
        }

        /// <summary>
        /// écrit un buffer dans un fichier
        /// </summary>
        /// <param name="data">les données àécrire</param>
        /// <param name="mode">le mode d'ouverture du fichier</param>
        /// <returns></returns>
        public async Task<bool> Ecrire(IBuffer data, CreationCollisionOption mode)
        {
            var retour = false;
            StorageFile file = null;
            switch (_placeEcriture)
            {
                case PlaceEcriture.LocalState:
                    file = await ApplicationData.Current.LocalFolder.CreateFileAsync(_path, mode);
                    break;

                case PlaceEcriture.Roaming:
                    file = await ApplicationData.Current.RoamingFolder.CreateFileAsync(_path, mode);
                    break;
            }

            if (file != null)
            {
                await FileIO.WriteBufferAsync(file, data);
                retour = true;
            }
            return retour;
        }

        /// <summary>
        /// lit un fichier en format binaire
        /// </summary>
        /// <returns>le contenu du fichier en byte array</returns>
        public async Task<byte[]> LireByteArray()
        {
            StorageFile file = null;
            switch (_placeEcriture)
            {
                case PlaceEcriture.LocalState:
                    file = await ApplicationData.Current.LocalFolder.GetFileAsync(_path);
                    break;

                case PlaceEcriture.Roaming:
                    file = await ApplicationData.Current.RoamingFolder.GetFileAsync(_path);
                    break;
            }

            if (file != null)
            {
                var buffer = await FileIO.ReadBufferAsync(file);
                return buffer.ToArray();
            }
            return null;
        }

        /// <summary>
        /// lit un fichier et le palce dans une chaine de caractère
        /// </summary>
        /// <returns>le contenu du fichier</returns>
        public async Task<String> LireString()
        {
            StorageFile file = null;
            switch (_placeEcriture)
            {
                case PlaceEcriture.LocalState:
                    file = await ApplicationData.Current.LocalFolder.GetFileAsync(_path);
                    break;

                case PlaceEcriture.Roaming:
                    file = await ApplicationData.Current.RoamingFolder.GetFileAsync(_path);
                    break;
            }
           
            return (file != null)? await FileIO.ReadTextAsync(file, UnicodeEncoding.Utf8):null;


        }

        /// <summary>
        /// lit un fichier et retourne ses lignes dans une liste de chaine de caractères
        /// </summary>
        /// <returns>le contenu du fichier</returns>
        public async Task<IList<String>> LireListString()
        {
            StorageFile file = null;
            switch (_placeEcriture)
            {
                case PlaceEcriture.LocalState:
                    file = await ApplicationData.Current.LocalFolder.GetFileAsync(_path);
                    break;

                case PlaceEcriture.Roaming:
                    file = await ApplicationData.Current.RoamingFolder.GetFileAsync(_path);
                    break;
            }
             return (file != null)? await FileIO.ReadLinesAsync(file, UnicodeEncoding.Utf8):null;
        }

        /// <summary>
        /// retourne le chemin du fichier
        /// </summary>
        /// <returns></returns>
        public String GetPath()
        {
            return _path;
        }

        /// <summary>
        /// permet de vérifier l'existence d'un fichier
        /// </summary>
        /// <returns>true si existe</returns>
        public async Task<bool> FileExist()
        {
                /* Version Windows phone 8
                try {
                    switch (_placeEcriture)
                    {
                        case PlaceEcriture.LocalState:
                            await ApplicationData.Current.LocalFolder.GetFileAsync(_path);
                            break;

                        case PlaceEcriture.Roaming:
                            await ApplicationData.Current.RoamingFolder.GetFileAsync(_path);
                            break;
                    }
		            return true;
	            } catch {
		            return false;
	            }*/
            IStorageItem item = null;

            switch (_placeEcriture)
            {
                case PlaceEcriture.LocalState:
                    item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(_path);
                    break;

                case PlaceEcriture.Roaming:
                    item = await ApplicationData.Current.RoamingFolder.TryGetItemAsync(_path);
                    break;
            }
            return item != null;
        }

        /// <summary>
        /// retourne la dernière date de modification d'un fichier
        /// </summary>
        /// <returns>la date de dernière modification</returns>
        public async Task<DateTimeOffset> GetDateModified()
        {
            StorageFile file = null;
            switch (_placeEcriture)
            {
                case PlaceEcriture.LocalState:
                    file = await ApplicationData.Current.LocalFolder.GetFileAsync(_path);
                    break;

                case PlaceEcriture.Roaming:
                    file = await ApplicationData.Current.RoamingFolder.GetFileAsync(_path);
                    break;
            }
            if(await FileExist())
            {
                return file != null ? (await file.GetBasicPropertiesAsync()).DateModified : new DateTimeOffset();
            }
            return new DateTimeOffset();
        }

        /// <summary>
        /// recherche dans un fichier (de type csv par exemple) la valeur d'une variable
        /// </summary>
        /// <param name="chaineDebut">le chaine de caractère écrite avant le résultat souhaité</param>
        /// <param name="caractereFin">le caractère de fin de recherche</param>
        /// <returns>le résultat sinon null</returns>
        public async Task<String> FindElementInFile(String chaineDebut,char caractereFin)
        {
            StorageFile file = null;
            switch (_placeEcriture)
            {
                case PlaceEcriture.LocalState:
                    file = await ApplicationData.Current.LocalFolder.GetFileAsync(_path);
                    break;

                case PlaceEcriture.Roaming:
                    file = await ApplicationData.Current.RoamingFolder.GetFileAsync(_path);
                    break;
            }

            if(file != null)
            {
                var data = await FileIO.ReadLinesAsync(file, UnicodeEncoding.Utf8);

                return (from mot in data where mot.Contains(chaineDebut) let fin = mot.LastIndexOf(caractereFin) let debut = mot.IndexOf(chaineDebut, StringComparison.Ordinal) + chaineDebut.Length select mot.Substring(debut, fin - debut)).FirstOrDefault();
            }
            return null;
        }

    }
}
