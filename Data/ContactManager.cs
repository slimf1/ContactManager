using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Data
{
    /// <summary>
    /// Instance du manager de contact
    /// </summary>
    [XmlRoot("ContactManager")]
    [Serializable]
    [XmlInclude(typeof(Folder)), XmlInclude(typeof(Contact))]
    public class ContactManager
    {
        private Folder _rootFolder;
        private Folder _currentFolder;

        /// <summary>
        /// Le dossier courant du manager
        /// </summary>
        [XmlIgnore]
        public Folder CurrentFolder
        {
            get => _currentFolder;
            set { _currentFolder = value; }
        }

        /// <summary>
        /// Le dossier racine de la hiérarchie
        /// </summary>
        public Folder RootFolder
        {
            get => _rootFolder;
            set { _rootFolder = value; }
        }

        /// <summary>
        /// Constructeur d'un manager
        /// </summary>
        public ContactManager()
        {
            _rootFolder = new Folder("Root");
            _currentFolder = _rootFolder;
        }

        /// <summary>
        /// Reconstruit la hiérarchie du système de fichier en
        /// fixant le dossier parent qui n'a pas été sérialisé.
        /// </summary>
        /// <remarks>Utilisé seulement pour la sérialisation XML.</remarks>
        /// <param name="parent">Le dossier racine de la hiérarchie à reconstruire</param>
        public void RemakeTree(Folder parent)
        {
            foreach(Element element in parent.Elements)
            {
                if (element is Folder subFolder && subFolder.Parent == null)
                {
                    RemakeTree(subFolder);
                    subFolder.Parent = parent;
                }
            }
        }

        /// <summary>
        /// Ajoute un noueau dossier de nom <paramref name="folderName"/>
        /// au dossier courant.
        /// </summary>
        /// <param name="folderName">Le nom du dossier à ajouter</param>
        /// <returns>Vrai si le dossier a effectivement été ajouté, faux sinon</returns>
        public bool AddFolder(string folderName)
        {
            Element element = _currentFolder.Find(folderName);
            if (element != null && element is Folder)
            {
                return false;
            } 
            else 
            {
                Folder newFolder = new Folder(folderName, _currentFolder);
                _currentFolder.Add(newFolder);
                _currentFolder = newFolder;
                return true;
            }
        }

        /// <summary>
        /// Affichage du dossier courant dans la hiérarchie.
        /// </summary>
        /// <remarks>Equivalent de la commande pwd d'un bash.</remarks>
        /// <returns>Le chemin du dossier courant.</returns>
        public string GetPrompt()
        {
            Folder currentFolder = _currentFolder;
            List<string> folders = new List<string>();
            while (currentFolder != null)
            {
                folders.Add(currentFolder.Name);
                currentFolder = currentFolder.Parent;
            }
            folders.Reverse();
            return $"{string.Join('/', folders)}> ";
        }

        /// <summary>
        /// Ajoute un nouveau contact au dossier courant
        /// </summary>
        /// <param name="firstName">Le prénom du contact</param>
        /// <param name="lastName">Le nom du contact</param>
        /// <param name="email">L'adresse email du contact</param>
        /// <param name="company">L'entreprise du contact</param>
        /// <param name="link">Le lien que l'on a avec le contact</param>
        /// <returns>Vrai si le contact a effectivement été ajouté, faux sinon</returns>
        public bool AddContact(string firstName, string lastName, string email, string company, Link link)
        {
            _currentFolder.Add(new Contact(lastName, firstName, email, company, link));
            return true;
        }

        /// <summary>
        /// Représentation du manager ; se basant sur la représentation
        /// de l'arbre du dossier parent
        /// </summary>
        /// <returns>La représentation du maanger</returns>
        public override string ToString()
        {
            return _rootFolder.DisplayElements();
        }

        /// <summary>
        /// Modifie le dossier courant pour aller au chemin <paramref name="path"/> spécifié.
        /// </summary>
        /// <param name="path">Le chemin du nouveau dossier courant</param>
        public void ChangeDirectory(string path)
        {
            Folder currentFolder;

            if (path[0] == '/')
            {
                currentFolder = _rootFolder;
                path = path.Substring(1);
            }
            else
            {
                currentFolder = _currentFolder;
            }
            foreach (string folderName in path.Split('/'))
            {
                if (folderName == ".." && currentFolder.Parent != null)
                {
                    currentFolder = currentFolder.Parent;
                }
                else
                {
                    Element element = currentFolder.Find(folderName);
                    if (element != null && element is Folder subFolder)
                    {
                        currentFolder = subFolder;
                    }
                }
            }
            _currentFolder = currentFolder;
        }
    }
}
