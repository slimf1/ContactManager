using System;
using Serialization.Serializers;

namespace Serialization
{
    /// <summary>
    /// Fabrique d'un <see cref="IContactManagerSerializer"/> pour 
    /// la sérialisation et déseralisation.
    /// </summary>
    public class SerializerFactory
    {
        /// <summary>
        /// Crée un nouveau serializer en fonction du type 
        /// <paramref name="serializationType"/> passé en paramètre.
        /// </summary>
        /// <param name="serializationType">Le type du serializer</param>
        /// <returns>Un serializer d'un manager de contact.</returns>
        public static IContactManagerSerializer Create(string serializationType)
        {
            Enum.TryParse(typeof(SerializerType), serializationType, true, out object type);

            if (type != null)
            {
                switch ((SerializerType)type)
                {
                    case SerializerType.Binary:
                        return new ContactManagerBinarySerializer();
                    case SerializerType.Xml:
                        return new ContactManagerXmlSerializer();
                }
            }
            throw new ArgumentException("Type de serializer invalide.");
        }
    }
}
