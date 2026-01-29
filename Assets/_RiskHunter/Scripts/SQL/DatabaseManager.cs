using UnityEngine;
using System.Data;
using Mono.Data.SqliteClient;
using System.IO;


public class DatabaseManager : MonoBehaviour
{
    [Header("---- CONFIGURATION ----")]
    private string dbPath;
    public int userIdConnecte = 0; 
    public int levelIdActuel = 1;  

    /

    void Awake()
    {
        // 1. On prépare le chemin de la BDD
        dbPath = "URI=file:" + Path.Combine(Application.persistentDataPath, "RiskhunterSave.db");
        CreerTables();

        // 2. On charge l'ID du joueur IMMÉDIATEMENT (avant que les portes ne s'affichent)
        if (PlayerPrefs.HasKey("ID_Joueur_Actif"))
        {
            userIdConnecte = PlayerPrefs.GetInt("ID_Joueur_Actif");
            Debug.Log("📂 DatabaseManager : Joueur ID " + userIdConnecte + " chargé.");
        }
        else
        {
            userIdConnecte = 0; 
        }
    }

 

   

    // --- CRÉATION BDD ---
    void CreerTables()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS Joueur (id INTEGER PRIMARY KEY, nom TEXT, niveau INT, score INT);";
                command.ExecuteNonQuery();
                command.CommandText = "CREATE TABLE IF NOT EXISTS Scores (player_id INTEGER, level_id INTEGER, high_score INTEGER, PRIMARY KEY (player_id, level_id));";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    // --- FONCTIONS DE SAUVEGARDE (Appelées par ManagerNiveauTri) ---

    public void SauvegarderScoreSiMeilleur(int levelId, int nouveauScore)
    {
        // On n'affiche rien ici, on sauvegarde juste !
        int ancienScore = GetMeilleurScore(levelId);

        if (nouveauScore > ancienScore)
        {
            ExecuteRequete("INSERT OR REPLACE INTO Scores (player_id, level_id, high_score) VALUES (" + userIdConnecte + ", " + levelId + ", " + nouveauScore + ");");
            Debug.Log("🏆 Nouveau Record BDD sauvegardé : " + nouveauScore);
        }
    }

    public int GetMeilleurScore(int levelId)
    {
        if (userIdConnecte == 0) return 0;
        int score = 0;
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT high_score FROM Scores WHERE player_id = " + userIdConnecte + " AND level_id = " + levelId + ";";
                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read()) score = reader.GetInt32(0);
                }
            }
            connection.Close();
        }
        return score;
    }

    public void AjouterAuScoreTotalProfil(int idSlot, int pointsGagnes)
    {
        ExecuteRequete("UPDATE Joueur SET score = score + " + pointsGagnes + " WHERE id = " + idSlot + ";");
    }

    // --- GESTION PROFILS ---
    public void CreerOuSauverProfil(int idSlot, string nom, int niveau, int score)
    {
        ExecuteRequete("INSERT OR REPLACE INTO Joueur (id, nom, niveau, score) VALUES (" + idSlot + ", '" + nom + "', " + niveau + ", " + score + ");");
    }

    public void SupprimerProfil(int idSlot)
    {
        ExecuteRequete("DELETE FROM Joueur WHERE id = " + idSlot + ";");
        ExecuteRequete("DELETE FROM Scores WHERE player_id = " + idSlot + ";");
    }

    public string GetNomProfil(int idSlot)
    {
        string nom = "Vide";
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT nom FROM Joueur WHERE id = " + idSlot + ";";
                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read()) nom = reader.GetString(0);
                }
            }
            connection.Close();
        }
        return nom;
    }

    // --- OUTILS & TOP 3 ---
    void ExecuteRequete(string sql)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }
    
    public string GetTop3ScoresTexte(int levelId)
    {
        string classement = "";
        int position = 1;

        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Joueur.nom, Scores.high_score FROM Scores JOIN Joueur ON Scores.player_id = Joueur.id WHERE Scores.level_id = " + levelId + " ORDER BY Scores.high_score DESC LIMIT 3;";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string nomJoueur = reader.GetString(0); 
                        int scoreJoueur = reader.GetInt32(1);   
                        classement += position + ". " + nomJoueur + " : " + scoreJoueur + "\n";
                        position++;
                    }
                }
            }
            connection.Close();
        }
        if (classement == "") return "Aucun score enregistré.";
        return classement;
    }
    
    
}