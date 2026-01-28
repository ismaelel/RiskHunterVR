using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Réglages du Niveau")]
    public int scorePourGagner = 500; // Total des points à obtenir (5 objets x 100)
    public string nomProchainNiveau = "Level2"; // Nom de la scène suivante
    
    [Header("État du Jeu")]
    public int scoreActuel = 0;
    private bool niveauFini = false;

    [Header("Interface UI")]
    public Text scoreText;
    public GameObject panneauVictoire; // Le panneau "Bravo"

    
    void Start() {
        // On demande à la mémoire globale quel est l'ID choisi dans le menu
        int idJoueur = PlayerPrefs.GetInt("ID_Joueur_Actif", 0); // 0 par défaut

        if (idJoueur != 0) {
            Debug.Log("🎮 Jeu lancé avec le profil ID : " + idJoueur);
            // Ici, tu peux dire à ton DatabaseManager local de charger les infos de cet ID
            // databaseManager.userIdConnecte = idJoueur;
        }
    }
    
    void Awake()
    {
        Instance = this;
    }

    public void AddScore(int amount)
    {
        if (niveauFini) return;

        scoreActuel += amount;
        
        // Mise à jour du texte
        if(scoreText != null) 
            scoreText.text = "Score : " + scoreActuel + " / " + scorePourGagner;

        // Vérification de la victoire
        if (scoreActuel >= scorePourGagner)
        {
            Victoire();
        }
    }

    void Victoire()
    {
        niveauFini = true;
        Debug.Log("NIVEAU 1 TERMINÉ !");
        
        // Affiche le panneau BRAVO
        if (panneauVictoire != null) 
            panneauVictoire.SetActive(true);

        // Charge le niveau 2 après 3 secondes
        Invoke("ChargerNiveauSuivant", 3f);
    }

    void ChargerNiveauSuivant()
    {
        SceneManager.LoadScene(nomProchainNiveau);
    }
}