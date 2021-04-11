using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Serialization;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.Serialization;

namespace ContactManagerApplication
{
    class ConsoleUi
    {
        private const int MAX_PASSWORD_TRIES = 3;
        private const string DEFAULT_SERIALIZATION_TYPE = "XML";

        private ContactManager _contactManager;
        private int _passwordTries;
        private string _serializationType;

        public ConsoleUi()
        {
            _contactManager = new ContactManager();
            _passwordTries = 0;
            _serializationType = DEFAULT_SERIALIZATION_TYPE;
        }

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

        private void ShowHelp() // Todo toutes les commandes + ERREURS !s
        {
            Console.WriteLine("Help: \n" +
                "afficher - Affiche la structure\n" +
                "ajouterdossier [nom] - Ajoute un sous-dossier au dossier courant\n" +
                "ajoutercontact [nom] [prenom] [email] [entr] [lien] - Ajoute un contact au dossier courant\n" +
                "cd [chemin] - Change le dossier courant\n" + // Delete !!! DeleteContact
                "sortir - Stoppe l'exécution du programme\n");
        }

        private bool ParseAnswer()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            //Console.Write(">");
            Console.Write(_contactManager.GetPrompt());
            Console.ResetColor();
            string userResponse = Console.ReadLine();
            // Sub methodes ??
            string command = userResponse.Split(" ")[0];
            string[] arguments = userResponse.Split(" ").Skip(1).ToArray();

            switch (command) // Gestion erreurs ! + Methodes pour args
            { // Faire fichier "console ui" + faire une itf graph ?
              //Tout mettre sur github et faire un readme 
                case "afficher": // Dire dans le cr (mail) que ça s'applique bien a une appli winform et pk 
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
                    // Et si parsing fail ? et si nb args invalide ??
                    // TODO
                    if (arguments.Length < 5)
                        Console.WriteLine("Syntaxe incorrecte. Utilisez: ajoutercontact nom prenom email entreprise lien");
                    else 
                    {
                        if (_contactManager.AddContact(arguments[0], arguments[1], arguments[2], arguments[3], (Link)Enum.Parse(typeof(Link), arguments[4])))
                            Console.WriteLine("Contact ajouté");
                        else
                            Console.Error.WriteLine("Format de l'email invalide");
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
                    } // CHECK SI EXISTE COMME TYPE
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

        private static string GetFilePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                $"{Environment.UserName}_ContactManager.db");
        }
        
        private void SaveToFile(string serialisationMethod)
        {
            string filePath = GetFilePath();
            Console.WriteLine("Ajoutez un mot de passe (Peut être vide, utilisera le SID): ");
            string password = Console.ReadLine();

            byte[] encryptionKey = ContactManagerCryptography.GetKeyFromPassword(password);

            IContactManagerSerializer serializer = SerializerFactory.Create(serialisationMethod);
            try
            {
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
        }

        private void LoadFromFile(string serializationMethod)
        {
            string filePath = GetFilePath();

            if (!File.Exists(filePath))
            {
                Console.Error.WriteLine("Le fichier de base n'existe pas.");
                return;
            }

            IContactManagerSerializer deserializer = SerializerFactory.Create(serializationMethod);
            Console.WriteLine("Entrez le mot de passe :");
            string password = Console.ReadLine();
            byte[] decryptionKey = ContactManagerCryptography.GetKeyFromPassword(password);

            try
            {
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
            // FAIRE GAFFE EXCEPTIONS QUI STOP LE PROGRAMME, CF SUJET
            catch (Exception e) when (
            e is SerializationException ||
            e is CryptographicException)// Gestion de la serialisation : si mdp mauvais => exception depend de la serialisation (expliquer dans le mail) => ce que j'ai fait !! 
            // catch des deux exceptions, fixe le type de seria utilisé ?
            {
                Console.Error.WriteLine($"Le chargement a échoué: Le mot de passe est incorrect.\n" +
                    $"Nombre d'essais: {++_passwordTries}, Essais restants: {MAX_PASSWORD_TRIES - _passwordTries}");
                if (_passwordTries >= MAX_PASSWORD_TRIES)
                {
                    Console.Error.WriteLine("Le nombre d'essais maximum a été dépassé. Le fichier va être supprimé.");
                    File.Delete(filePath);
                    _passwordTries = 0;
                }
            }
        }
        private void AddContactCommand(string[] arguments)
        {
            if (arguments.Length == 5)
            {

            }
        }

        public void DisplayMenu() // gestion errerus 
        {
            ShowIntroduction();
            bool keep = true;
            do
            {
                try
                {
                    keep = ParseAnswer();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Une erreur est survenue: {e.Message}");
                }
            } while (keep);
        }
    }


}
