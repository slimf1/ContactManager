using Data;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Serialization.Serializers
{
    public class ContactManagerBinarySerializer : IContactManagerSerializer
    {
        private static readonly BinaryFormatter BINARY_FORMATTER = new BinaryFormatter();

        public ContactManager Deserialize(Stream stream) 
        {
            return (ContactManager)BINARY_FORMATTER.Deserialize(stream);
        }

        public void Serialize(Stream stream, ContactManager manager)
        {
            BINARY_FORMATTER.Serialize(stream, manager);
        }
    }
}
