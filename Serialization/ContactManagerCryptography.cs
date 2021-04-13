using System;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Security.Cryptography;

namespace Serialization
{
    /// <summary>
    /// Gestion du chiffrement du fichier du ContactManager
    /// </summary>
    public class ContactManagerCryptography
    {
        /// <summary>
        /// Taille d'une clé de chiffrement
        /// </summary>
        private const int KEY_SIZE = 256;

        /// <summary>
        /// Taille du vecteur d'initialisation
        /// </summary>
        private const int IV_SIZE = 16;

        /// <summary>
        /// Salage pour la génération de la clé
        /// </summary>
        private static readonly byte[] SALT = new byte[] { 
            0x26, 0xdc, 0xff, 0x00, 0xad, 0xed, 0x7a, 0xee, 
            0xc5, 0xfe, 0x07, 0xaf, 0x4d, 0x08, 0x22, 0x3c 
        };

        /// <summary>
        /// Génération d'une clé de chiffrement à partir 
        /// du mot de passe <paramref name="password"/>. Si le mot de passe est nul,
        /// va générer une clé à partir du SID de l'utilisateur courant.
        /// </summary>
        /// <remarks>Se base sur la RFC2898 qui définit l'algorithme de génération des clés.</remarks>
        /// <param name="password">Le mot de passe utilisé pour générer la clé.</param>
        /// <returns>La clé de chiffrement sous forme d'un tableau de 32 octets.</returns>
        public static byte[] GetKeyFromPassword(string password)
        {
            if (password == string.Empty)
            {
                password = UserPrincipal.Current.Sid.ToString();
            }
            Rfc2898DeriveBytes byteDerivator = new Rfc2898DeriveBytes(password, SALT);
            return byteDerivator.GetBytes(32);
        }

        /// <summary>
        /// Créé un nouveau <see cref="CryptoStream"/> qui applique une transformation de
        /// Rijndael sur le flux <paramref name="stream"/> passé en paramètre. 
        /// </summary>
        /// <param name="key">La clé de chiffrement de l'algorithme Rijnadel.</param>
        /// <param name="stream">Le flux à chiffrer</param>
        /// <returns>Le <see cref="CryptoStream" /> qui appliquera la transformation pour chiffrer le contenu du flux.</returns>
        public static CryptoStream EncryptionStream(byte[] key, Stream stream)
        {
            byte[] iv = new byte[IV_SIZE];
            
            // Définit le vecteur d'initialisation avec un 
            // générateur de nombres aléatoires crypto secure
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetNonZeroBytes(iv);
            }

            // Ecriture du vecteur d'initialisation au début du flux
            stream.Write(iv, 0, iv.Length);

            // Algorithme de chiffrement utilisé 
            // Rijndael A.K.A AES
            Rijndael rijndael = new RijndaelManaged
            {
                KeySize = KEY_SIZE
            };

            return new CryptoStream(
                stream,
                rijndael.CreateEncryptor(key, iv), // Transformation
                CryptoStreamMode.Write
            );
        }

        /// <summary>
        /// Créé un nouveau <see cref="CryptoStream"/> qui applique une
        /// transformation de Rijndael sur le flux <paramref name="stream"/> pour le déchiffrement.
        /// </summary>
        /// <param name="key">La clé de chiffrement (tableau de 32 octets).</param>
        /// <param name="stream">Le flux à déserialiser</param>
        /// <returns>Un flux pour le déchiffrement.</returns>
        public static CryptoStream DecryptionStream(byte[] key, Stream stream)
        {
            byte[] iv = new byte[IV_SIZE];

            // Lecture du vecteur d'initialisation au début du flux 
            if (stream.Read(iv, 0, iv.Length) != iv.Length)
                throw new Exception("IV invalid");

            Rijndael rijndael = new RijndaelManaged
            {
                KeySize = KEY_SIZE
            };

            return new CryptoStream(
                stream,
                rijndael.CreateDecryptor(key, iv),
                CryptoStreamMode.Read
            );
        }
    }
}
