using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Data
{
    /// <summary>
    /// Dossier du manager de contact. 
    /// Conteneur d'éléments de tout type.
    /// </summary>
    [Serializable]
    public class Folder : Element
    {
        /// <summary>
        /// Les éléments d'un dossier.
        /// </summary>
        [XmlArrayItem("Contact", typeof(Contact)), XmlArrayItem("Folder", typeof(Folder))]
        public List<Element> Elements { get; set; }

        /// <summary>
        /// Le dossier parent.
        /// </summary>
        [XmlIgnore]
        public Folder Parent { get; set; }
        
        /// <summary>
        /// Constructeur d'un dossier.
        /// </summary>
        /// <param name="name">Le nom du dossier.</param>
        /// <param name="parent">Le dossier parent.</param>
        public Folder(string name, Folder parent) : base(name)
        {
            Parent = parent;
            Elements = new List<Element>();
        }

        /// <summary>
        /// Constructeur d'un dossier.
        /// </summary>
        /// <param name="name">Le nom du dossier.</param>
        public Folder(string name) : this(name, null)
        {
        }

        /// <summary>
        /// Constructeur sans argument d'un dossier.
        /// </summary>
        /// <remarks>Nécessaire pour la sérialisation XML.</remarks>
        public Folder() : this("")
        {
        }

        /// <summary>
        /// Ajoute l'élément <paramref name="element"/> au dossier.
        /// </summary>
        /// <param name="element">L'élément à ajouter.</param>
        public void Add(Element element)
        {
            Elements.Add(element);
        }

        /// <summary>
        /// Recherche d'un élément dans un dossier
        /// à partir de son nom.
        /// </summary>
        /// <param name="elementName">Le nom de l'élément recherché.</param>
        /// <returns>L'élément s'il a été trouvé, null sinon.</returns>
        public Element Find(string elementName)
        {
            return Elements.FirstOrDefault(e => e.Name == elementName);
        }

        /// <summary>
        /// Représentation d'un dossier.
        /// </summary>
        /// <returns>Une chaîne représentant un dossier.</returns>
        public override string ToString()
        {
            return $"[D] {Name} (Created at {LastModificationDate})";
        }

        /// <summary>
        /// Affiche les éléments du dossier en une structure d'arbre
        /// en utilisant un parcours en profondeur.
        /// </summary>
        /// <returns>Une chaîne représentant le contenu du dossier 
        /// sous forme d'arbre.</returns>
        public string DisplayElements()
        {
            StringBuilder builder = new StringBuilder();
            int level = 0;
            Stack<Tuple<Element, int>> stack = new Stack<Tuple<Element, int>>();

            // Sauvegarde de l'élément et du niveau courant 
            stack.Push(Tuple.Create<Element, int>(this, level));

            while (stack.Count > 0)
            {
                Tuple<Element, int> element = stack.Pop();

                // Indentation 
                level = element.Item2;
                builder.AppendFormat("{0} {1}",
                    string.Concat(Enumerable.Repeat(" |", level)),
                    element.Item1.ToString());
                builder.AppendLine();

                // Sous éléments d'un dossier 
                if (element.Item1 is Folder subFolder)
                {
                    level++;
                    foreach (Element subElement in subFolder.Elements)
                    {
                        stack.Push(Tuple.Create(subElement, level));
                    }
                }
            }

            return builder.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is Folder folder &&
                   base.Equals(obj) &&
                   Name == folder.Name &&
                   CreationDate == folder.CreationDate &&
                   LastModificationDate == folder.LastModificationDate &&
                   EqualityComparer<List<Element>>.Default.Equals(Elements, folder.Elements);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Name, CreationDate, LastModificationDate, Elements);
        }
    }
}
