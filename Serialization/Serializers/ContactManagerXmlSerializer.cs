using Data;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Serialization.Serializers
{
    /// <summary>
    /// Sérialisation XML d'un <see cref="ContactManager"/>.
    /// </summary>
    public class ContactManagerXmlSerializer : IContactManagerSerializer
    {
        private static readonly XmlSerializer XML_SERIALIZER = new XmlSerializer(
            typeof(ContactManager), 
            new Type[] { typeof(Folder), typeof(Contact) }
        );

        /// <summary>
        /// Déserialise un <see cref="ContactManager"/> ; enregistré sous format
        /// XML dans le flux <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Le flux de lecture</param>
        /// <returns>La session du manager de contact déserialisée.</returns>
        public ContactManager Deserialize(Stream stream)
        {
            return (ContactManager)XML_SERIALIZER.Deserialize(stream);
        }

        /// <summary>
        /// Sérialise le <see cref="ContactManager"/> <paramref name="stream"/> 
        /// en format XML dans le flux <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Le flux d'écriture</param>
        /// <param name="manager">La session du manager à sauvegarder</param>
        public void Serialize(Stream stream, ContactManager manager)
        {
            XML_SERIALIZER.Serialize(stream, manager);
        }
    }
}
