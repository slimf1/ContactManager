using Data;
using System.IO;

namespace Serialization
{
    /// <summary>
    /// Représente un serializer quelconque d'un contact manager. 
    /// </summary>
    public interface IContactManagerSerializer
    {
        /// <summary>
        /// Sérialise dans le flux <paramref name="stream"/> le contenu du
        /// contact manager <paramref name="manager"/>.
        /// </summary>
        /// <param name="stream">Le flux dans lequel écrire le contneu du manager.</param>
        /// <param name="manager">Le manager à sérialiser.</param>
        void Serialize(Stream stream, ContactManager manager);

        /// <summary>
        /// Déserialisation d'un contact manager contenu dans un flux
        /// <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Le flux où lire le contact manager.</param>
        /// <returns>Le contact manager déserialisé.</returns>
        ContactManager Deserialize(Stream stream);
    }
}
