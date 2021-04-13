using System;
using System.Linq;
using Data;
using Serialization;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.Serialization;

namespace ContactManagerApplication
{
    /// <summary>
    /// Interface utilisateur console pour l'utilisation d'un <see cref="ContactManager"/>.
    /// </summary>
    class ConsoleUi
    {
        /// <summary>
        /// Essais maximum d'un mdp erroné 
        /// </summary>
        private const int MAX_PASSWORD_TRIES = 3;
        
        /// <summary>
        /// Le format de sérialisation par défaut
        /// </summary>
        private const string DEFAULT_SERIALIZATION_TYPE = "XML";

        /// <summary>
        /// L'instance courante du manager de contact
        /// </summary>
        private ContactManager _contactManager;
        /// <summary>
        /// Le nombre d'essais courants pour les mdp erronés
        /// </summary>
        private int _passwordTries;
        /// <summary>
        /// Le type de sérialisation utilisé.
        /// </summary>
        private string _serializationType;

        /// <summary>
        /// Constructeur d'un <see cref="ConsoleUi"/>.
        /// </summary>
        public ConsoleUi()
        {
            _contactManager = new ContactManager();
            _passwordTries = 0;
            _serializationType = DEFAULT_SERIALIZATION_TYPE;
        }

        /// <summary>
        /// Affiche l'introduction en console.
        /// </summary>
        private void ShowIntroduction()
        {
            Console.WriteLine(
                "=============================\n" +
                "*      CONTACT MANAGER      *\n" +
                "*---------------------------*\n" +
                "* Gestionnaire de contacts  *\n" +
                "*---------------------------*\n" +
                "* .NET C# - Slimane Fakani  *\n" +
                "*---------------------------*\n" +
                "* Tapez help pour avoir la  *\n" +
                "* liste des commandes       *\n" +
                "=============================");
        }

        /// <summary>
        /// Affiche la liste des commandes.
        /// </summary>
        private void ShowHelp()
        {
            Console.WriteLine("Help: \n" +
                "afficher - Affiche la structure\n" +
                "enregistrer - Enregistre la session\n" +
                "charger - Charge une session\n" +
                "ajouterdossier [nom] - Ajoute un sous-dossier au dossier courant\n" +
                "ajoutercontact [nom] [prenom] [email] [entr] [lien] - Ajoute un contact au dossier courant\n" +
                "cd [chemin] - Change le dossier courant\n" +
                "typeserialisation [type] - Change le type de la sérialisation\n" + 
                "sortir - Stoppe l'exécution du programme\n");
        }

        /// <summary>
        /// Lit et interprète la commande de l'utilisateur.
        /// </summary>
        /// <returns>Un booléen qui indique si l'utilisateur continue l'exécution.</returns>
        private bool ParseAnswer()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(_contactManager.GetPrompt());
            Console.ResetColor();
            string userResponse = Console.ReadLine();
            string command = userResponse.Split(" ")[0];
            string[] arguments = userResponse.Split(" ").Skip(1).ToArray();

            switch (command)
            { 
                case "afficher":
                    Console.WriteLine(_contactManager);
                    break;

                case "enregistrer":
                    SaveToFile(_serializationType);
                    break;

                case "charger":
                    LoadFromFile(_serializationType);
                    break;

                case "ajouterdossier":
                case "addfolder":
                    if (arguments.Length == 0)
                        Console.WriteLine("Syntaxe incorrecte. Utilisez: ajouterdossier nom");
                    else
                        _contactManager.AddFolder(arguments[0]);
                    break;

                case "ajoutercontact":
                case "addcontact":
                    if (arguments.Length < 5)
                        Console.WriteLine("Syntaxe incorrecte. Utilisez: ajoutercontact nom prenom email entreprise lien");
                    else 
                    {
                        try
                        {
                            if (_contactManager.AddContact(arguments[0], arguments[1], arguments[2], arguments[3], (Link)Enum.Parse(typeof(Link), arguments[4])))
                                Console.WriteLine("Contact ajouté");
                            else
                                Console.Error.WriteLine("Format de l'email invalide");
                        } 
                        catch(ArgumentException e)
                        {
                            Console.Error.WriteLine($"Ajout du contact impossible: {e.Message}");
                        }
                    }
                    break;

                case "cd":
                    if (arguments.Length == 0)
                        Console.WriteLine("Syntaxe incorrecte. Utilisez: cd chemin. Par exemple: cd ../../dossier1/mondossier");
                    else
                        _contactManager.ChangeDirectory(arguments[0]);
                    break;

                case "typeserialisation":
                    if (arguments.Length == 0)
                        Console.WriteLine("Syntaxe incorrecte. Utilisez: typeserialisation type. Par exemple: typeserialisation xml");
                    else
                    {
                        _serializationType = arguments[0];
                        Console.WriteLine($"Le type de serialisation utilisé a été modifié en: {_serializationType}");
                    }
                    break;

                case "sortir":
                case "quit":
                    return false;

                case "":
                case "help":
                    ShowHelp();
                    break;

                default:
                    Console.WriteLine($"La commande \"{command}\" n'a pas été reconnue. Faites help pour avoir la liste.");
                    break;
            }
            return true;
        }

        /// <summary>
        /// Chemin du fichier de sauvegarde de la base.
        /// </summary>
        /// <returns>Le fichier où est sauvegardé l'instance sérialisée du contact manager.</returns>
        private static string GetFilePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                $"{Environment.UserName}_ContactManager.db");
        }
        
        /// <summary>
        /// Sauvegarde l'instance du manager de contact en utilisant la méthode de
        /// sérialisation spécifiée.
        /// </summary>
        /// <param name="serialisationMethod">La méthode de sérialisation à utiliser pour la sérialisation.</param>
        private void SaveToFile(string serialisationMethod)
        {
            string filePath = GetFilePath();
            Console.WriteLine("Ajoutez un mot de passe (Peut être vide, utilisera le SID): ");
            string password = Console.ReadLine();

            byte[] encryptionKey = ContactManagerCryptography.GetKeyFromPassword(password);

            try
            {
                IContactManagerSerializer serializer = SerializerFactory.Create(serialisationMethod);
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    using (CryptoStream cryptoStream = ContactManagerCryptography.EncryptionStream(encryptionKey, fileStream))
                    {
                        serializer.Serialize(cryptoStream, _contactManager);
                        Console.WriteLine($"Contacts sauvegardés dans: \"{filePath}\"");
                    }
                }
            }
            catch (SerializationException e)
            {
                Console.WriteLine($"Erreur de sauvegarde: {e.Message}");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Type de sérialisation invalide.");
            }
        }

        /// <summary>
        /// Charge une instance du manager de contact.
        /// </summary>
        /// <param name="deserializationMethod">La méthode de déserialisation à utiliser</param>
        private void LoadFromFile(string deserializationMethod)
        {
            string filePath = GetFilePath();

            if (!File.Exists(filePath))
            {
                Console.Error.WriteLine("Le fichier de base n'existe pas.");
                return;
            }

            Console.WriteLine("Entrez le mot de passe :");
            string password = Console.ReadLine();
            byte[] decryptionKey = ContactManagerCryptography.GetKeyFromPassword(password);

            try
            {
                IContactManagerSerializer deserializer = SerializerFactory.Create(deserializationMethod);
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    using (CryptoStream cryptoStream = ContactManagerCryptography.DecryptionStream(decryptionKey, fileStream))
                    {
                        _contactManager = deserializer.Deserialize(cryptoStream);
                        _contactManager.RemakeTree(_contactManager.RootFolder);
                        _contactManager.CurrentFolder = _contactManager.RootFolder;
                        Console.WriteLine($"Contacts chargés depuis: \"{filePath}\"");
                        _passwordTries = 0;
                    }
                }
            }
            catch (Exception e) when (
            e is SerializationException ||
            e is CryptographicException)
            {
                Console.Error.WriteLine($"Le chargement a échoué: Le mot de passe est incorrect.\n" +
                    $"Nombre d'essais: {++_passwordTries}, Essais restants: {MAX_PASSWORD_TRIES - _passwordTries}");
                if (_passwordTries >= MAX_PASSWORD_TRIES)
                {
                    Console.Error.WriteLine("Le nombre d'essais maximum a été dépassé. Le fichier va être supprimé.");
                    File.Delete(filePath);
                    _passwordTries = 0;
                }
            } catch(ArgumentException)
            {
                Console.WriteLine("Type de sérialisation invalide");
            }
        }

        /// <summary>
        /// Main loop de l'interface console.
        /// </summary>
        public void DisplayMenu()  
        {
            ShowIntroduction();
            bool keep;
            do
            {
                keep = ParseAnswer();
            } while (keep);
        }
    }
}
