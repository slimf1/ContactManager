using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Data
{
    /// <summary>
    /// Element du système de management de contact.
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(Folder)), XmlInclude(typeof(Contact))]
    public abstract class Element
    {
        /// <summary>
        /// Le nom de l'élément.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// L'instant de création d'un élément.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// L'instant de la dernière modification d'un élément.
        /// </summary>
        public DateTime LastModificationDate { get; set; }

        /// <summary>
        /// Constructeur d'élément sans argument, fixe
        /// son nom à null.
        /// </summary>
        /// <remarks>Nécessaire pour la sérialisation XML.</remarks>
        public Element() : this(null)
        {
        }

        /// <summary>
        /// Constructeur d'un élément, utilisable par les
        /// sous-classes.
        /// </summary>
        /// <param name="name">Le nom de l'élément.</param>
        public Element(string name)
        {
            Name = name;
            CreationDate = DateTime.Now;
            LastModificationDate = DateTime.Now;
        }

        /// <summary>
        /// Met à jour la date de dernière modification.
        /// </summary>
        public void Touch()
        {
            LastModificationDate = DateTime.Now;
        }

        public override bool Equals(object obj)
        {
            return obj is Element element &&
                   Name == element.Name &&
                   CreationDate == element.CreationDate &&
                   LastModificationDate == element.LastModificationDate;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, CreationDate, LastModificationDate);
        }
    }
}
