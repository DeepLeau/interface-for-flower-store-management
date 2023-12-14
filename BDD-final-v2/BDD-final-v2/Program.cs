using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Xml;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using Org.BouncyCastle.Crypto;
using Newtonsoft.Json;
using System.Globalization;

namespace BDD_final_v2
{

    class Boutique
    {
        public string nom_boutique;
        public string adresse_boutique;
        public int id_boutique;

        public Boutique(string nom, string adresse, int id_boutique)
        {
            this.nom_boutique = nom;
            this.adresse_boutique = adresse;
            this.id_boutique = id_boutique;
        }

        public static void AfficherListeBoutiques(List<Boutique> boutiques)
        {
            Console.WriteLine("Liste des boutiques :");
            foreach (Boutique boutique in boutiques)
            {
                Console.WriteLine($"Id : {boutique.id_boutique} Nom : {boutique.nom_boutique}, Adresse : {boutique.adresse_boutique}");
            }
        }
        public void AjouterEnBaseDeDonnees()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Boutique (nom_boutique, adresse_boutique, id_boutique) VALUES (@nom_boutique, @adresse_boutique, @id_boutique)";
            command.Parameters.AddWithValue("@nom_boutique", nom_boutique);
            command.Parameters.AddWithValue("@adresse_boutique", adresse_boutique);
            command.Parameters.AddWithValue("@id_boutique", id_boutique);
            command.ExecuteNonQuery();

            connection.Close();
        }

        public static List<Boutique> RecupererToutesLesBoutiques()
        {
            List<Boutique> boutiques = new List<Boutique>();

            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT nom_boutique, adresse_boutique, id_boutique FROM Boutique";

            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string nom = reader.GetString("nom_boutique");
                string adresse = reader.GetString("adresse_boutique");
                int id = reader.GetInt32("id_boutique");

                Boutique boutique = new Boutique(nom, adresse, id);
                boutiques.Add(boutique);
            }

            reader.Close();
            connection.Close();

            return boutiques;
        }
    }
    class Client
    {
        private string nom_client;
        private string prenom_client;
        private string courriel_client;
        private string telephone_client;
        private string mot_de_passe;
        private string adresse_facturation;
        private string carte_credit;

        public string getCourriel_Client()
        {
            return courriel_client;
        }

        public Client(string nom, string prenom, string email, string telephone, string motDePasse, string adresseFacturation, string carteDeCredit)
        {
            this.nom_client = nom;
            this.prenom_client = prenom;
            this.courriel_client = email;
            this.telephone_client = telephone;
            this.mot_de_passe = motDePasse;
            this.adresse_facturation = adresseFacturation;
            this.carte_credit = carteDeCredit;
        }

        public Client(string courriel, string motDePasse)
        {
            this.courriel_client = courriel;
            this.mot_de_passe = motDePasse;
        }

        public Historique ObtenirHistorique(string email)
        {
            List<dynamic> achats = new List<dynamic>();
            List<int> mois = new List<int>();
            string fidelite = "";

            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT achat, fidelite, mois FROM Historique WHERE email = @email";
                command.Parameters.AddWithValue("@email", email);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string achat = reader.GetString("achat");
                        fidelite = reader.GetString("fidelite");
                        int moisItem = reader.GetInt32("mois");

                        achats.Add(achat);
                        mois.Add(moisItem);
                    }
                }
            }

            Historique historique = new Historique(achats, email, fidelite, mois);
            return historique;
        }


        public void PasserCommande(Bouquet bouquet)
        {
            Historique historique = ObtenirHistorique(this.courriel_client);
            historique.AjouterAchat(bouquet);
        }

        public void AfficherHistoriqueAchats()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT achat FROM Historique WHERE email = @courriel_client";
                command.Parameters.AddWithValue("@courriel_client", courriel_client);

                MySqlDataReader reader = command.ExecuteReader();

                Console.WriteLine("--- Historique des achats ---");
                while (reader.Read())
                {
                    string achat = reader.GetString("achat");
                    Console.WriteLine(achat);
                }

                reader.Close();
            }
        }

        public Client CreerNouveauClient()
        {
            Console.WriteLine("Création d'un nouveau client");
            Console.Write("Entrez votre nom : ");
            string nom = Console.ReadLine();
            Console.Write("Entrez votre prénom : ");
            string prenom = Console.ReadLine();
            Console.Write("Entrez votre adresse email : ");
            string email = Console.ReadLine();
            Console.Write("Entrez votre numéro de téléphone : ");
            string telephone = Console.ReadLine();
            Console.Write("Entrez votre mot de passe : ");
            string motDePasse = Console.ReadLine();
            Console.Write("Entrez votre adresse de facturation : ");
            string adresseFacturation = Console.ReadLine();
            Console.Write("Entrez votre carte de crédit : ");
            string carteDeCredit = Console.ReadLine();

            if (VerifierEmailExiste(email))
            {
                Console.WriteLine("Un compte avec cet email existe déjà.");
                return null;
            }

            Client nouveauClient = new Client(nom, prenom, email, telephone, motDePasse, adresseFacturation, carteDeCredit);
            nouveauClient.AjouterClientEnBaseDeDonnees();
            Console.WriteLine("Compte créé avec succès.");
            return nouveauClient;
        }

        private static bool VerifierEmailExiste(string email)
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM client WHERE courriel_client = @email";
            command.Parameters.AddWithValue("@email", email);

            int nombreResultats = Convert.ToInt32(command.ExecuteScalar());

            connection.Close();

            return nombreResultats > 0;
        }

        private void AjouterClientEnBaseDeDonnees()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO client (nom_client, prenom_client, courriel_client, telephone_client, mot_de_passe, adresse_facturation, carte_credit) VALUES (@nom_client, @prenom_client, @courriel_client, @telephone_client, @mot_de_passe, @adresse_facturation, @carte_credit)";
            command.Parameters.AddWithValue("@nom_client", nom_client);
            command.Parameters.AddWithValue("@prenom_client", prenom_client);
            command.Parameters.AddWithValue("@courriel_client", courriel_client);
            command.Parameters.AddWithValue("@telephone_client", telephone_client);
            command.Parameters.AddWithValue("@mot_de_passe", mot_de_passe);
            command.Parameters.AddWithValue("@adresse_facturation", adresse_facturation);
            command.Parameters.AddWithValue("@carte_credit", carte_credit);

            command.ExecuteNonQuery();
            connection.Close();
        }
        public Client SeConnecter()
        {
            Console.WriteLine("--- Connexion ---");
            Console.Write("Courriel : ");
            string courriel = Console.ReadLine();
            Console.Write("Mot de passe : ");
            string motDePasse = Console.ReadLine();
            if (VerifierInformationsConnexion(courriel, motDePasse))
            {
                Console.WriteLine("Connexion réussie !");
                return new Client(courriel, motDePasse);
            }
            else
            {
                Console.WriteLine("Échec de la connexion. Veuillez vérifier vos informations de connexion.");
                return null;
            }
        }
        private static bool VerifierInformationsConnexion(string courriel, string motDePasse)
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM Client WHERE courriel_client = @courriel AND mot_de_passe = @motDePasse";
                command.Parameters.AddWithValue("@courriel", courriel);
                command.Parameters.AddWithValue("@motDePasse", motDePasse);

                int nombreResultats = Convert.ToInt32(command.ExecuteScalar());

                return nombreResultats > 0;
            }
        }
    }


    class Employe
    {
        private int id_employe;
        private string nom_employe;
        private string prenom_employe;
        private string mot_de_passe;
        public Employe(int id_employe, string nom_employe, string prenom_employe, string mot_de_passe)
        {
            this.id_employe = id_employe;
            this.nom_employe = nom_employe;
            this.prenom_employe = prenom_employe;
            this.mot_de_passe = mot_de_passe;
        }

        public void SeConnecter()
        {
            Console.WriteLine("--- Connexion Employé ---");
            Console.Write("Identifiant : ");
            int id = Convert.ToInt32(Console.ReadLine());
            Console.Write("Mot de passe : ");
            string motDePasse = Console.ReadLine();

            if (VerifierInformationsConnexion(id, motDePasse))
            {
                Console.WriteLine("Connexion réussie !");
                AfficherMenuEmploye();
            }
            else
            {
                Console.WriteLine("Échec de la connexion. Veuillez vérifier vos informations de connexion.");
            }
        }

        private bool VerifierInformationsConnexion(int id, string motDePasse)
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Employe WHERE id_employe = @id AND mot_de_passe = @motDePasse";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@motDePasse", motDePasse);

            int count = Convert.ToInt32(command.ExecuteScalar());

            connection.Close();

            return count > 0;
        }

        public void MettreAJourCommande(Commande commande)
        {
            Console.WriteLine("écrivez l'état que vous voulez mettre à la commande : ");
            string etat = Console.ReadLine();
            Console.WriteLine("Donnez la description de l'état");
            string etat_nom = Console.ReadLine();
            commande.MettreAJourEtat(etat, etat_nom);
        }
        public void AfficherMenuEmploye()
        {
            Console.WriteLine("--- Espace Employé ---");
            Console.WriteLine("1. Afficher l'inventaire");
            Console.WriteLine("2. Mettre à jour l'état d'une commande");
            Console.WriteLine("3. Ajouter un item à un inventaire");

            int choix = Convert.ToInt32(Console.ReadLine());

            switch (choix)
            {
                case 1:
                    Console.WriteLine();
                    Console.WriteLine("Sélectionnez l'identifiant de la boutique où vous voulez observer l'inventaire");
                    int id_inventaire = Convert.ToInt32(Console.ReadLine());
                    AfficherInventaire(id_inventaire);
                    break;
                case 2:
                    Console.WriteLine();
                    Console.WriteLine("Saisissez l'identifiant de la commande que vous voulez modifiez : ");
                    int id_commande = Convert.ToInt32(Console.ReadLine());
                    Commande commande = new Commande(id_commande);
                    MettreAJourCommande(commande);
                    break;
                case 3:
                    Console.WriteLine();
                    Console.WriteLine("Sélectionnez l'identifiant de la boutique où vous voulez ajouter un item");
                    int id_boutique = Convert.ToInt32(Console.ReadLine());
                    Inventaire inventaire = new Inventaire("test", 0, "test", "test");
                    inventaire.AjouterItem(id_boutique);
                    Console.WriteLine("Item ajouté avec succès !");
                    break;
                default:
                    Console.WriteLine();
                    Console.WriteLine("Choix invalide. Veuillez réessayer.");
                    break;
            }
        }
        public void AfficherInventaire(int idBoutique)
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            string query = "SELECT * FROM Inventaire WHERE id_boutique = @id_boutique";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id_boutique", idBoutique);

                    MySqlDataReader reader = command.ExecuteReader();

                    Console.WriteLine("----- Inventaire -----");
                    Console.WriteLine("Nom Fleur\tPrix\tDisponibilité\tCategorie");
                    Console.WriteLine("-------------------------------------------------");

                    while (reader.Read())
                    {
                        string nomFleur = reader.GetString("nom_fleur");
                        int prixFleur = reader.GetInt32("prix_fleur");
                        string disponibiliteFleur = reader.GetString("disponibilite_fleur");
                        string categorie = reader.GetString("type_fleur");

                        Console.WriteLine($"{nomFleur}\t {prixFleur}\t{disponibiliteFleur}\t{categorie}");
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur s'est produite : " + ex.Message);
            }
        }
    }

    class Bouquet_Standard
    {
        public string nom_bouquet;
        public string composition_bouquet;
        public int prix;
        public string categorie;

        public Bouquet_Standard(string nom_bouquet, string composition_bouquet, int prix, string categorie)
        {
            this.nom_bouquet = nom_bouquet;
            this.composition_bouquet = composition_bouquet;
            this.prix = prix;
            this.categorie = categorie;
        }
        public static List<Bouquet_Standard> RecupererTousLesBouquetsStandards()
        {
            List<Bouquet_Standard> bouquets = new List<Bouquet_Standard>();

            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT nom_bouquet, composition_bouquet, prix, categorie FROM Bouquet_Standard";

            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string nom_bouquet = reader.GetString("nom_bouquet");
                string composition_bouquet = reader.GetString("composition_bouquet");
                int prix = reader.GetInt32("prix");
                string categorie = reader.GetString("categorie");

                Bouquet_Standard bouquet = new Bouquet_Standard(nom_bouquet, composition_bouquet, prix, categorie);
                bouquets.Add(bouquet);
            }

            reader.Close();
            connection.Close();

            return bouquets;
        }
        private void AjouterBouquetStandardEnBaseDeDonnees()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Bouquet_Standard (nom_bouquet, composition_bouquet, prix, categorie) VALUES (@nom_bouquet, @composition_bouquet, @prix, @categorie)";
            command.Parameters.AddWithValue("@nom_bouquet", nom_bouquet);
            command.Parameters.AddWithValue("@composition_bouquet", composition_bouquet);
            command.Parameters.AddWithValue("@prix", prix);
            command.Parameters.AddWithValue("@categorie", categorie);

            command.ExecuteNonQuery();
            connection.Close();
        }

        public static Bouquet_Standard CreerNouveauBouquetStandard()
        {
            Console.WriteLine("Création d'un nouveau bouquet standard");
            Console.Write("Entrez le nom du bouquet voulu : ");
            string nom_bouquet = Console.ReadLine();
            Console.Write("Entrez la composition du bouquet voulu : ");
            string composition = Console.ReadLine();
            Console.Write("Entrez le prix du bouquet voulu : ");
            int prix = Convert.ToInt32(Console.ReadLine());
            Console.Write("Entrez la catégorie du bouquet voulu : ");
            string categorie = Console.ReadLine();

            Bouquet_Standard nouveaubouquet = new Bouquet_Standard(nom_bouquet, composition, prix, categorie);
            nouveaubouquet.AjouterBouquetStandardEnBaseDeDonnees();
            Console.WriteLine("Bouquet ajouté avec succès.");
            return nouveaubouquet;
        }

        public static void AfficherTousLesBouquetsStandards()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT nom_bouquet, composition_bouquet, prix, categorie FROM Bouquet_Standard";

            MySqlDataReader reader = command.ExecuteReader();

            Console.WriteLine("----- Bouquets standards -----");
            while (reader.Read())
            {
                string nom_bouquet = reader.GetString("nom_bouquet");
                string composition_bouquet = reader.GetString("composition_bouquet");
                int prix = reader.GetInt32("prix");
                string categorie = reader.GetString("categorie");

                Console.WriteLine($"Nom : {nom_bouquet}, Composition : {composition_bouquet}, Prix : {prix}, Catégorie : {categorie}");
            }

            reader.Close();
            connection.Close();
        }

        public string GetBouquet_standard()
        {
            string final = $"{nom_bouquet} {composition_bouquet} {categorie} {prix}";
            return final;
        }
    }

    class Bouquet_personnalise
    {
        private int budget;
        private string description;

        public int GetBudget()
        {
            return this.budget;
        }

        public Bouquet_personnalise(int budget, string description)
        {
            this.budget = budget;
            this.description = description;
        }
        public string GetBouquet_personnalise()
        {
            string final = this.budget.ToString() + description;
            return final;
        }

        public static Bouquet_personnalise CreerNouveauBouquetPersonnalise()
        {
            Console.WriteLine("Création d'un nouveau bouquet personnalisé");
            Console.Write("Entrez votre budget : ");
            int budget = Convert.ToInt32(Console.ReadLine());
            Console.Write("Entrez la description : ");
            string description = Console.ReadLine();

            Bouquet_personnalise nouveauBouquet = new Bouquet_personnalise(budget, description);
            nouveauBouquet.AjouterBouquetPersonnaliseEnBaseDeDonnees();

            Console.WriteLine("Bouquet personnalisé ajouté avec succès.");

            return nouveauBouquet;
        }

        public void AjouterBouquetPersonnaliseEnBaseDeDonnees()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Bouquet_personnalise (budget, description) VALUES (@budget, @description)";
                command.Parameters.AddWithValue("@budget", budget);
                command.Parameters.AddWithValue("@description", description);

                command.ExecuteNonQuery();
            }
        }

    }
    class Bouquet
    {
        private Bouquet_Standard Bouquet_Standard;
        private Bouquet_personnalise Bouquet_Personnalise;

        public Bouquet_Standard GetBouquet_Standard()
        {
            return this.Bouquet_Standard;
        }
        public Bouquet_personnalise GetBouquet_Personnalise()
        {
            return this.Bouquet_Personnalise;
        }
        public Bouquet(Bouquet_Standard bouquet_Standard, Bouquet_personnalise bouquet_Personnalise)
        {
            this.Bouquet_Standard = bouquet_Standard;
            this.Bouquet_Personnalise = bouquet_Personnalise;
        }
    }
    class Historique
    {
        private List<dynamic> achats;
        public string email;
        private string fidelite;
        private List<int> mois;

        public Historique(List<dynamic> achats, string email, string fidelite, List<int> mois)
        {
            this.achats = achats;
            this.email = email;
            this.fidelite = fidelite;
            this.mois = mois;
        }

        public void AjouterAchat(Bouquet achat)
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string achat_final = "";

                if (achat.GetBouquet_Standard() != null)
                {
                    achats.Add(achat.GetBouquet_Standard());
                    achat_final = achat.GetBouquet_Standard().GetBouquet_standard();
                }
                if (achat.GetBouquet_Personnalise() != null)
                {
                    achats.Add(achat.GetBouquet_Personnalise());
                    achat_final = achat.GetBouquet_Personnalise().GetBouquet_personnalise();
                }

                int mois_final = DateTime.Now.Month;
                mois.Add(mois_final);

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Historique (achat, fidelite, email, mois) VALUES ( @achat_final, @fidelite, @email, @mois)";
                command.Parameters.AddWithValue("@achat_final", achat_final);
                command.Parameters.AddWithValue("@fidelite", fidelite);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@mois", mois_final);
                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        private void AppliquerFidelite()
        {

            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            int mois_precedent = mois[mois.Count - 2];
            int compteur = 0;
            int compteur2 = 0;

            for (int i = mois.Count - 2; i >= 0; i--)
            {
                if (mois[i] == mois_precedent)
                {
                    compteur++;
                }
                compteur2++;
            }

            if (compteur >= 5)
            {
                fidelite = "Or";
            }
            else if (compteur2 >= 12 && compteur2 < 60)
            {
                fidelite = "Bronze";
            }
            else
            {
                fidelite = "";
            }

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Historique SET fidelite = @fidelite WHERE email = @email";
            command.Parameters.AddWithValue("@fidelite", fidelite);
            command.Parameters.AddWithValue("@email", email);
            command.ExecuteNonQuery();

            connection.Close();
        }

    }

    class Commande
    {
        public string code_etat;
        public string etat_nom;
        public int id_commande;

        public Commande(string code_etat, string etat_nom, int id_commande)
        {
            this.code_etat = code_etat;
            this.etat_nom = etat_nom;
            this.id_commande = id_commande;
        }

        public Commande(int nom)
        {
            this.id_commande = nom;
        }
        public void MettreAJourEtatCommande()
        {
            int idCommande = 0;
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    MySqlCommand selectCommand = connection.CreateCommand();
                    selectCommand.CommandText = "SELECT numero_commande, date_commande, date_livraison FROM bon_de_commande WHERE numero_commande = @id_commande";
                    selectCommand.Parameters.AddWithValue("@id_commande", id_commande);

                    MySqlDataReader reader = selectCommand.ExecuteReader();

                    if (reader.Read())
                    {
                        idCommande = reader.GetInt32(0);
                        DateTime dateCommande = reader.GetDateTime(1);
                        DateTime dateLivraison = DateTime.Parse(reader.GetString(2));

                        if (dateLivraison >= dateCommande.AddDays(3))
                        {
                            code_etat = "CC";
                        }
                    }

                    reader.Close();

                    MySqlCommand updateCommand = connection.CreateCommand();
                    updateCommand.CommandText = "UPDATE Commande SET code_etat = @code_etat, etat_nom = @etat_nom WHERE id_commande = @id_commande";
                    updateCommand.Parameters.AddWithValue("@code_etat", string.IsNullOrEmpty(code_etat) ? "VINV" : code_etat);
                    updateCommand.Parameters.AddWithValue("@etat_nom", string.IsNullOrEmpty(etat_nom) ? "Commande standard pour laquelle un employé doit vérifier l’inventaire" : etat_nom);
                    updateCommand.Parameters.AddWithValue("@id_commande", id_commande);

                    updateCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur s'est produite : " + ex.Message);
            }
        }



        public void MettreAJourEtat(string nouvelEtat, string etat_description)
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand updateCommand = connection.CreateCommand();
            updateCommand.CommandText = "UPDATE Commande SET code_etat = @nouvelEtat, etat_nom = @etat_description WHERE id_commande = @id_commande";
            updateCommand.Parameters.AddWithValue("@nouvelEtat", nouvelEtat);
            updateCommand.Parameters.AddWithValue("@id_commande", id_commande);
            updateCommand.Parameters.AddWithValue("@etat_description", etat_description);
            updateCommand.ExecuteNonQuery();

            connection.Close();
        }

        public void AfficherEtatCommande(int numeroCommande)
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT c.etat_nom " +
                                   "FROM floral.bon_de_commande bc " +
                                   "JOIN floral.Commande c ON bc.code_etat = c.code_etat " +
                                   "WHERE bc.numero_commande = @numeroCommande";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@numeroCommande", numeroCommande);

                    string etatCommande = command.ExecuteScalar() as string;

                    if (etatCommande != null)
                    {
                        Console.WriteLine($"L'état de la commande numéro {numeroCommande} : {etatCommande}");
                    }
                    else
                    {
                        Console.WriteLine($"La commande numéro {numeroCommande} n'existe pas ou n'a pas d'état associé.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur s'est produite : " + ex.Message);
            }
        }
    }

    class BonDeCommande
    {
        private string adresse_livraison;
        private string date_livraison;
        private string message_floral;
        private string email;

        public BonDeCommande(string adresse_livraison, string date_livraison, string message_floral, string email)
        {
            this.adresse_livraison = adresse_livraison;
            this.date_livraison = date_livraison;
            this.message_floral = message_floral;
            this.email = email;
        }

        public int CreerNouveauBonDeCommande()
        {
            Console.WriteLine("----- Création d'un nouveau bon de commande -----");
            Console.Write("Adresse de livraison : ");
            string adresseLivraison = Console.ReadLine();
            Console.Write("Date de livraison (yyyy-mm-dd) : ");
            string dateLivraison = Console.ReadLine();
            Console.Write("Message floral : ");
            string messageFloral = Console.ReadLine();

            int idCommande = 0;

            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO bon_de_commande (adresse_livraison, date_livraison, message_floral) VALUES (@adresse_livraison, @date_livraison, @message_floral); SELECT LAST_INSERT_ID();";
                command.Parameters.AddWithValue("@adresse_livraison", adresseLivraison);
                command.Parameters.AddWithValue("@date_livraison", dateLivraison);
                command.Parameters.AddWithValue("@message_floral", messageFloral);

                idCommande = Convert.ToInt32(command.ExecuteScalar());
            }

            Console.WriteLine("Bon de commande ajouté avec succès.");

            return idCommande;
        }


        public void AjouterEnBaseDeDonnees()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO bon_de_commande (adresse_livraison, date_livraison, message_floral, email) VALUES (@adresse_livraison, @date_livraison, @message_floral, @email)";
                command.Parameters.AddWithValue("@adresse_livraison", adresse_livraison);
                command.Parameters.AddWithValue("@date_livraison", date_livraison);
                command.Parameters.AddWithValue("@message_floral", message_floral);
                command.Parameters.AddWithValue("@email", email);

                command.ExecuteNonQuery();
            }
        }
    }

    class Inventaire
    {
        private string nom_fleur;
        private int prix_fleur;
        private string disponibilite_fleur;
        private string type_fleur;

        public Inventaire(string nom_fleur, int prix_fleur, string disponibilite_fleur, string type_fleur)
        {
            this.nom_fleur = nom_fleur;
            this.prix_fleur = prix_fleur;
            this.disponibilite_fleur = disponibilite_fleur;
            this.type_fleur = type_fleur;
        }

        public void AjouterItem(int idBoutique)
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                Console.Write("Entrez le nom de la fleur : ");
                string nomFleur = Console.ReadLine();
                Console.Write("Entrez le prix de la fleur : ");
                int prixFleur = Convert.ToInt32(Console.ReadLine());
                Console.Write("Entrez la disponibilité de la fleur : ");
                string disponibiliteFleur = Console.ReadLine();
                Console.WriteLine("Entrez la catégorie de la fleur");
                string type_fleur = Console.ReadLine();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Inventaire (nom_fleur, prix_fleur, disponibilite_fleur, id_boutique, type_fleur) VALUES (@nom_fleur, @prix_fleur, @disponibilite_fleur, @id_boutique, @type_fleur)";
                command.Parameters.AddWithValue("@nom_fleur", nomFleur);
                command.Parameters.AddWithValue("@prix_fleur", prixFleur);
                command.Parameters.AddWithValue("@disponibilite_fleur", disponibiliteFleur);
                command.Parameters.AddWithValue("@id_boutique", idBoutique);
                command.Parameters.AddWithValue("@type_fleur", type_fleur);

                command.ExecuteNonQuery();
            }
        }


    }

    class Paiement
    {
        private int id_paiement;
        private int prix;
        private int id_boutique;
        private string nom_bouquet;

        public Paiement(int id_paiement, int prix, int id_boutique, string nom_bouquet)
        {
            this.id_paiement = id_paiement;
            this.prix = prix;
            this.id_boutique = id_boutique;
            this.nom_bouquet = nom_bouquet;
        }

        public void AjouterPaiement()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Paiement (id_paiement, prix, id_boutique, nom_bouquet) VALUES (@id_paiement, @prix, @id_boutique, @nom_bouquet)";
                command.Parameters.AddWithValue("@id_paiement", this.id_paiement);
                command.Parameters.AddWithValue("@prix", this.prix);
                command.Parameters.AddWithValue("@id_boutique", this.id_boutique);
                command.Parameters.AddWithValue("@nom_bouquet", this.nom_bouquet);

                command.ExecuteNonQuery();
            }
        }

    }

    internal class Program
    {
        static void meilleur_bouquet()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText =
             " Select nom_bouquet, count(nom_bouquet) as total_vendu from Paiement group by nom_bouquet order by total_vendu desc limit 1;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();


            while (reader.Read())
            {
                string meilleur = (string)reader["nom_bouquet"];
                Console.WriteLine("Le bouquet standard le plus vendu est {0} ", meilleur);
            }
            connection.Close();
        }
        static void prixmoyenpaiement()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText =
             " SELECT avg(prix) from paiement ;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            float prix_moyen;
            while (reader.Read())
            {
                prix_moyen = reader.GetFloat(0);
                Console.WriteLine("Le prix moyen des bouquets achetés est de {0} euros ", prix_moyen);
            }
            connection.Close();
        }
        static void meilleur_chiffre()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText =
             " SELECT nom_boutique, SUM(prix) AS TotalCA FROM boutique natural join paiement GROUP BY nom_boutique ORDER BY TotalCA DESC limit 1;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            if (reader.Read())
            {
                string magasin = (string)reader["nom_boutique"];
                decimal totalCA = (decimal)reader["TotalCA"];
                Console.WriteLine("Le magasin qui a généré le plus de chiffre d'affaires est la {0}, avec un total de {1} euros.", magasin, totalCA);
            }
            else
            {
                Console.WriteLine("Aucune vente n'a été trouvée.");
            }

            connection.Close();

        }
        static void meilleur_client_annee()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            Console.WriteLine("Entrez l'année :");
            int annee = Convert.ToInt32(Console.ReadLine());

            MySqlCommand command = connection.CreateCommand();
            command.CommandText =
             " Select count(Achat) as TotalAchat,email FROM Historique Group By email ORDER BY TotalAchat DESC";

            command.Parameters.AddWithValue("@Annee", annee);

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            if (reader.Read())
            {
                string email = reader.GetString("Email");
                int totalAchat = reader.GetInt32("TotalAchat");
                Console.WriteLine("Le meilleur client de l'année {0} est {1}, avec un total d'achat de {2}.", annee, email, totalAchat);
            }
            else
            {
                Console.WriteLine("Aucun achat n'a été trouvé pour l'année {0}.", annee);
            }

            connection.Close();
        }
        static void meilleur_client_mois()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            Console.WriteLine("Entrez le numéro du mois entre 1 et 12 :");
            int mois = Convert.ToInt32(Console.ReadLine());

            MySqlCommand command = connection.CreateCommand();
            command.CommandText =
             " SELECT email, Achat FROM Historique WHERE Mois = @Mois ORDER BY Achat DESC limit 1 ;";

            command.Parameters.AddWithValue("@mois", mois);

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            if (reader.Read())
            {
                string email = (string)reader["Email"];
                string achat = (string)reader["Achat"];
                Console.WriteLine("Le meilleur client du mois {0} est {1}, avec un achat de {2}.", mois, email, achat);
            }
            else
            {
                Console.WriteLine("Aucun achat n'a été trouvé pour le mois {0}.", mois);
            }

            connection.Close();
        }
        static void MettreEnXML()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string query = "SELECT c.nom_client, c.prenom_client, COUNT(H.achat) as nombre_commandes FROM Client c JOIN Historique H ON c.courriel_client = H.email WHERE H.mois <= DATE_SUB(CURDATE(), INTERVAL 1 MONTH) GROUP BY H.email HAVING nombre_commandes > 0 ;";

            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            XmlDocument xmlDocument = new XmlDocument();
            XmlElement rootElement = xmlDocument.CreateElement("clients");
            xmlDocument.AppendChild(rootElement);

            foreach (DataRow row in dataTable.Rows)
            {
                XmlElement clientElement = xmlDocument.CreateElement("client");

                XmlElement nomElement = xmlDocument.CreateElement("nom");
                nomElement.InnerText = row["nom_client"].ToString();
                clientElement.AppendChild(nomElement);

                XmlElement prenomElement = xmlDocument.CreateElement("prenom");
                prenomElement.InnerText = row["prenom_client"].ToString();
                clientElement.AppendChild(prenomElement);

                XmlElement nombreCommandesElement = xmlDocument.CreateElement("nombre_commandes");
                nombreCommandesElement.InnerText = row["nombre_commandes"].ToString();
                clientElement.AppendChild(nombreCommandesElement);

                rootElement.AppendChild(clientElement);
            }

            string cheminFichier = @"C:\Users\Thomas\Desktop\Interoperabilite\clients.xml";
            xmlDocument.Save(cheminFichier);

            connection.Close();

            Console.WriteLine("Export fini !");
        }

        static void ClientNonCommandeDepuis6MoisJson()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=floral;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string query = "SELECT c.nom_client, c.prenom_client, c.telephone_client, c.courriel_client" +
                           "FROM Client c " +
                           "LEFT JOIN bon_de_commande b ON c.courriel_client = b.email " +
                           "WHERE b.date_commande IS NULL OR b.date_commande < DATE_SUB(CURDATE(), INTERVAL 6 MONTH)";

            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            string json = JsonConvert.SerializeObject(dataTable, Newtonsoft.Json.Formatting.Indented);
            string filePath = @"C:\Users\lucas\Desktop\ESILV\S6\BDD\projet\.json";
            File.WriteAllText(filePath, json);

            connection.Close();

            Console.WriteLine("Export fini !");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("----- Qui êtes vous ? -----");
            Console.WriteLine("1 - Espace Client");
            Console.WriteLine("2 - Espace Employé");
            Console.WriteLine("3 - Espace Gérant");
            int choix = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();
            if (choix == 1)
            {
                Console.WriteLine("Vous êtes dans l'espace Client, souhaitez-vous créer un compte ou vous connecter à un compte existant ?");
                Console.WriteLine("1 - Créer un compte");
                Console.WriteLine("2 - Se connecter à un compte existant");
                int choix_compte = Convert.ToInt32(Console.ReadLine());
                Client Client_final = new Client("test", "test");
                Console.WriteLine();
                if (choix_compte == 1)
                {
                    Client Nouveau_client = new Client("test", "test");
                    Client_final = Nouveau_client.CreerNouveauClient();
                    while (Client_final == null)
                    {
                        Client_final = Nouveau_client.CreerNouveauClient();
                    }
                }
                else
                {
                    Client Client_inter = new Client("test", "test");
                    Client_final = Client_inter.SeConnecter();
                    while (Client_final == null)
                    {
                        Client_final = Client_final.SeConnecter();
                    }
                }
                Console.WriteLine();
                Console.WriteLine(" Quel boutique voulez-vous visiter ? ");
                List<Boutique> boutiques = Boutique.RecupererToutesLesBoutiques();

                Console.WriteLine("----- Liste des boutiques -----");
                foreach (Boutique boutique in boutiques)
                {
                    Console.WriteLine($"ID : {boutique.id_boutique} Nom : {boutique.nom_boutique}, Adresse : {boutique.adresse_boutique}");
                }
                Console.WriteLine();
                Console.WriteLine("Veuillez saisir un nombre");
                int id_boutique = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine();
                Console.WriteLine("----- Que voulez-vous faire ? -----");
                Console.WriteLine("1 - Consulter l'historique de vos achats");
                Console.WriteLine("2 - Acheter un bouquet");
                int choix_client = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine();
                if (choix_client == 1)
                {
                    Client_final.AfficherHistoriqueAchats();
                }
                else
                {
                    Console.WriteLine("Voulez-vous un bouquet standard ou personnalisé ?");
                    Console.WriteLine("1 - Bouquet Standard");
                    Console.WriteLine("2 - Bouquet Personnalisé");
                    int choix_bouquet = Convert.ToInt32(Console.ReadLine());
                    if (choix_bouquet == 1)
                    {
                        List<Bouquet_Standard> bouquets = Bouquet_Standard.RecupererTousLesBouquetsStandards();
                        Console.WriteLine("----- Liste des bouquets standards -----");
                        int numero_bouquet = 1;
                        foreach (Bouquet_Standard bouquet in bouquets)
                        {
                            Console.WriteLine($"Numero : {numero_bouquet}  Nom bouquet : {bouquet.nom_bouquet}, Composition : {bouquet.composition_bouquet}, Prix : {bouquet.prix}, categorie : {bouquet.categorie}");
                            numero_bouquet++;
                        }
                        Console.WriteLine("Saisissez le numéro du bouquet que vous souhaitez acheter");
                        int choix_bouquet_Standard = Convert.ToInt32(Console.ReadLine());
                        Bouquet bouquet_inter = new Bouquet(bouquets[choix_bouquet_Standard - 1], null);
                        Client_final.PasserCommande(bouquet_inter);
                        BonDeCommande bon = new BonDeCommande("test", "test", "test", "test");
                        int id_Commande_finale = bon.CreerNouveauBonDeCommande();
                        Console.WriteLine("Commande passée !");
                        Commande bouquetStandard = new Commande("test", "test", 0);
                        bouquetStandard.MettreAJourEtatCommande();
                        Paiement nouveauPaiement = new Paiement(id_Commande_finale, bouquets[choix_bouquet_Standard - 1].prix, id_boutique, bouquets[choix_bouquet_Standard - 1].nom_bouquet);
                        nouveauPaiement.AjouterPaiement();
                    }
                    else
                    {
                        Bouquet_personnalise bouquet_perso = Bouquet_personnalise.CreerNouveauBouquetPersonnalise();
                        Bouquet bouquet_inter = new Bouquet(null, bouquet_perso);
                        Client_final.PasserCommande(bouquet_inter);
                        BonDeCommande bon = new BonDeCommande("test", "test", "test", "test");
                        int id_Commande_finale = bon.CreerNouveauBonDeCommande();
                        Console.WriteLine("Commande passée !");
                        Commande bouquetStandard = new Commande("test", "test", 0);
                        bouquetStandard.MettreAJourEtatCommande();
                        Paiement nouveauPaiement = new Paiement(id_Commande_finale, bouquet_perso.GetBudget(), id_boutique, null);
                        nouveauPaiement.AjouterPaiement();
                    }
                }
            }
            if (choix == 2)
            {
                Employe employe = new Employe(0, "test", "test", "test");
                employe.SeConnecter();
            }
            if (choix == 3)
            {
                Console.WriteLine("----- Que voulez-vous faire ? -----");
                Console.WriteLine("1 - Voir toutes vos boutiques ");
                Console.WriteLine("2 - Voir les statistiques");
                Console.WriteLine("3 - Ajouter une boutique");
                int choix_gerant = Convert.ToInt32(Console.ReadLine());
                if (choix_gerant == 1)
                {
                    List<Boutique> boutiques = Boutique.RecupererToutesLesBoutiques();
                    Boutique.AfficherListeBoutiques(boutiques);
                }
                if (choix_gerant == 2)
                {
                    Console.WriteLine();
                    Console.WriteLine("----- Quels statistiques voulez vous observer ? -----");
                    Console.WriteLine("1 - Voir qui est le meilleur client de l'année");
                    Console.WriteLine("2 - Voir qui est le meilleur client du mois");
                    Console.WriteLine("3 - Voir quel boutique a fait le plus gros chiffre d'affaire");
                    Console.WriteLine("4 - Voir le prix moyen des commandes");
                    Console.WriteLine("5 - Voir le bouquet standard le plus vendu");
                    int choix_stats = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine();
                    if (choix_stats == 1)
                    {
                        meilleur_client_annee();
                    }
                    if (choix_stats == 2)
                    {
                        meilleur_client_mois();
                    }
                    if (choix_stats == 3)
                    {
                        meilleur_chiffre();
                    }
                    if (choix_stats == 4)
                    {
                        prixmoyenpaiement();
                    }
                    if (choix_stats == 5)
                    {
                        meilleur_bouquet();
                    }
                }
                if (choix_gerant == 3)
                {
                    Console.WriteLine("Quel est le nom de la boutique ?");
                    string nom_boutique = Console.ReadLine();
                    Console.WriteLine("Quel est l'adresse de la boutique ?");
                    string adresse = Console.ReadLine();
                    Console.WriteLine("Quel identifiant voulez-vous mettre à votre boutique ?");
                    int id_nouvelleBoutique = Convert.ToInt32(Console.ReadLine());
                    Boutique NouvelleBoutique = new Boutique(nom_boutique, adresse, id_nouvelleBoutique);
                    NouvelleBoutique.AjouterEnBaseDeDonnees();
                }
            }
            MettreEnXML();

        }
    }
}
