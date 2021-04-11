using Data;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Serialization.Serializers
{
    public class ContactManagerXmlSerializer : IContactManagerSerializer
    {
        public ContactManager Deserialize(Stream stream)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ContactManager), new Type[] { typeof(Folder), typeof(Contact) });
            ContactManager contactManager = null;
            try
            {
                contactManager = (ContactManager)xmlSerializer.Deserialize(stream);
            } catch (SerializationException e)
            {
                Console.Error.WriteLine($"Erreur lors de la deserialisation XML: {e.Message}");
            }
            return contactManager;
        }

        public void Serialize(Stream stream, ContactManager manager)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ContactManager), new Type[] { typeof(Folder), typeof(Contact) });
                xmlSerializer.Serialize(stream, manager);
            } catch (SerializationException e)
            {
                Console.Error.WriteLine($"Erreur lors de la serialisation XML: {e.Message}");
            }
        }
    }
}
