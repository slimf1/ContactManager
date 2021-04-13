using Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Serialization.Serializers
{
    /// <summary>
    /// Sérialisation binaire d'un <see cref="ContactManager"/>.
    /// </summary>
    public class ContactManagerBinarySerializer : IContactManagerSerializer
    {
        private static readonly BinaryFormatter BINARY_FORMATTER = new BinaryFormatter();

        /// <summary>
        /// Sérialise un <see cref="ContactManager"/> sous format binaire dans
        /// le flux <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Le flux de lecture</param>
        /// <returns>La session du manager deserialisée.</returns>
        public ContactManager Deserialize(Stream stream) 
        {
            return (ContactManager)BINARY_FORMATTER.Deserialize(stream);
        }

        /// <summary>
        /// Sérialise un <see cref="ContactManager"/> sous format binaire dans
        /// le flux <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Le flux d'écriture</param>
        /// <param name="manager">La session du manager de contact.</param>
        public void Serialize(Stream stream, ContactManager manager)
        {
            BINARY_FORMATTER.Serialize(stream, manager);
        }
    }
}
