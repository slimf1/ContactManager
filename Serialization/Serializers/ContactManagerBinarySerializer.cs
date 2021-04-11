using Data;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Serialization.Serializers
{
    public class ContactManagerBinarySerializer : IContactManagerSerializer
    {
        public ContactManager Deserialize(Stream stream) // 4x => /2 si lors de l'appel
        {
            ContactManager contactManager = null;
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                contactManager = (ContactManager) formatter.Deserialize(stream);
            } catch (SerializationException e)
            {
                Console.Error.WriteLine($"La serialization binaire a échouée: {e.Message}");
            }
            return contactManager;
        }

        public void Serialize(Stream stream, ContactManager manager)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, manager);
            } catch(SerializationException e)
            {
                Console.Error.WriteLine($"La serialization binaire a échouée: {e.Message}");
            }
        }
    }
}
