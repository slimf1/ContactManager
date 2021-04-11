using Data;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Serialization.Serializers
{
    public class ContactManagerXmlSerializer : IContactManagerSerializer
    {
        private static readonly XmlSerializer XML_SERIALIZER = new XmlSerializer(
            typeof(ContactManager), 
            new Type[] { typeof(Folder), typeof(Contact) }
        );

        public ContactManager Deserialize(Stream stream)
        {
            return (ContactManager)XML_SERIALIZER.Deserialize(stream);
        }

        public void Serialize(Stream stream, ContactManager manager)
        {
            XML_SERIALIZER.Serialize(stream, manager);
        }
    }
}
