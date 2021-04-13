using System;
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
        /// Teste l'égalité entre deux éléments.
        /// </summary>
        /// <param name="obj">La deuxième opérande.</param>
        /// <returns>Vrai si les deux éléments sont égaux.</returns>
        public override bool Equals(object obj)
        {
            return obj is Element element &&
                   Name == element.Name &&
                   CreationDate == element.CreationDate &&
                   LastModificationDate == element.LastModificationDate;
        }

        /// <summary>
        /// Code de hachage d'un élément.
        /// </summary>
        /// <returns>Le hashcode d'un élément.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, CreationDate, LastModificationDate);
        }
    }
}
