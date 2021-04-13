using System;
using System.Text.RegularExpressions;

namespace Data
{
    /// <summary>
    /// Représentation d'un contact.
    /// </summary>
    [Serializable]
    public class Contact : Element
    {
        /// <summary>
        /// L'expréssion régulière représentant une adresse email (simplifiée).
        /// </summary>
        private static readonly Regex EMAIL_REGEX = new Regex(@"^\w+(?:\.\w+)*@\w+(?:\.\w+)*\.[A-Za-z]{2,}$");

        /// <summary>
        /// Adresse email
        /// </summary>
        private string _email;

        /// <summary>
        /// Le prénom d'un contact
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// L'adresse email d'un contact
        /// </summary>
        public string Email 
        {
            get => _email;
            set 
            {
                if (value != null && !EMAIL_REGEX.IsMatch(value))
                    throw new ArgumentException("L'adresse email n'est pas valide");
                _email = value;
            } 
        }

        /// <summary>
        /// L'entreprise du contact
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Le lien que l'on a avec le contact
        /// </summary>
        public Link ContactLink { get; set; }

        /// <summary>
        /// Constructeur d'un contact
        /// </summary>
        /// <param name="name">Le nom du contact</param>
        /// <param name="firstName">Le prénom du contact</param>
        /// <param name="email">L'adresse email du contact</param>
        /// <param name="company">L'entreprise du contact</param>
        /// <param name="contactLink">Le lien que l'on a avec le contact</param>
        public Contact(string name, string firstName, string email, string company, Link contactLink) : base(name)
        {
            //Enum.Parse(typeof(Link), linkString)
            FirstName = firstName;
            Email = email;
            Company = company;
            ContactLink = contactLink;
        }

        /// <summary>
        /// Constructeur par défaut d'un contact. 
        /// </summary>
        /// <remarks>Nécessaire pour la sérialisation XML.</remarks>
        public Contact() : this(null, null, null, null, Link.Friend)
        {

        }

        /// <summary>
        /// Représentation en une chaîne d'un contact
        /// </summary>
        /// <returns>Un contact sous la forme d'une chaîne</returns>
        public override string ToString()
        {
            return $"[C] {Name}, {FirstName} ({Company}), " +
                $"Email: {Email}, Link: {ContactLink} ";
        }

        /// <summary>
        /// Teste l'égalité entre deux contacts.
        /// </summary>
        /// <param name="obj">La deuxième opérande.</param>
        /// <returns>Vrai si les deux contacts son égaux, faux sinon.</returns>
        public override bool Equals(object obj)
        {
            return obj is Contact contact &&
                   base.Equals(obj) &&
                   Name == contact.Name &&
                   CreationDate == contact.CreationDate &&
                   LastModificationDate == contact.LastModificationDate &&
                   _email == contact._email &&
                   FirstName == contact.FirstName &&
                   Email == contact.Email &&
                   Company == contact.Company &&
                   ContactLink == contact.ContactLink;
        }

        /// <summary>
        /// Valeur de hachage d'un contact.
        /// </summary>
        /// <returns>Le hash d'un contact.</returns>
        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(base.GetHashCode());
            hash.Add(Name);
            hash.Add(CreationDate);
            hash.Add(LastModificationDate);
            hash.Add(_email);
            hash.Add(FirstName);
            hash.Add(Email);
            hash.Add(Company);
            hash.Add(ContactLink);
            return hash.ToHashCode();
        }
    }
}
