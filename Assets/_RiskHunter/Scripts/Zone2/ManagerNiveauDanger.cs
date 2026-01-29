using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;
using TMPro; 

public class ManagerNiveauDanger : MonoBehaviour
{
    [Header("---- CONFIGURATION ----")]
    [Tooltip("Si coché, compte tout seul les scripts Hazard présents dans la scène")]
    public bool compterAutomatiquement = true;
    public int totalDangersAValider = 3; // Sera écrasé si le mode auto est activé
    
    // ID du niveau pour la BDD (ex: 2 = Sécurité/Risques)
    public int idDuNiveauBDD = 2; 
    public string nomSceneMenu = "MenuPrincipal";

    [Header("---- UI EN JEU (HUD) ----")]
    public TextMeshProUGUI texteScoreEnJeu; // Le HUD face caméra

    [Header("---- UI FIN DE NIVEAU ----")]
    public GameObject panneauFinNiveau;
    public TextMeshProUGUI texteScoreFinal; 
    public GameObject boutonVoirRecords;    
    public AudioSource sonVictoire;         

    [Header("---- BASE DE DONNÉES ----")]
    public DatabaseManager monDatabaseManager; 

    // Compteur interne
    private int dangersResolus = 0;
    private bool niveauFini = false;

    void Start()
    {
        // 1. Connexion BDD
        if(monDatabaseManager == null) 
            monDatabaseManager = FindFirstObjectByType<DatabaseManager>();
        
        // 2. Masquer le panneau de fin
        if(panneauFinNiveau) 
            panneauFinNiveau.SetActive(false);

        // 3. Calcul Automatique
        if (compterAutomatiquement)
        {
            // On compte les zones à protéger (Huile, Câble, Ventilo)
            // Note : On cherche tous les objets qui ont le script 'Hazard_ZoneAProteger'
            int nbProtections = FindObjectsByType<Hazard_ZoneAProteger>(FindObjectsSortMode.None).Length;
            
            // On compte les zones encombrées (Extincteur) s'il y en a
            int nbDebarras = FindObjectsByType<Hazard_ZoneEncombre>(FindObjectsSortMode.None).Length;

            totalDangersAValider = nbProtections + nbDebarras;
            Debug.Log($"Calcul Auto : {totalDangersAValider} dangers trouvés.");
        }

        MettreAJourHUD();
    }

    // ---------------------------------------------------------
    // --- ACTION DU JEU  ---
    // ---------------------------------------------------------

    public void ValiderUnDanger()
    {
        dangersResolus++;
        
        // Feedback Immédiat (Texte flottant)
        if (FeedbackManager.Instance != null) 
            FeedbackManager.Instance.Afficher("SÉCURISÉ ! (+1)", Color.green);

        MettreAJourHUD();
        VerifierFinNiveau();
    }

    // ---------------------------------------------------------
    // --- GESTION DE L'AFFICHAGE ---
    // ---------------------------------------------------------

    void MettreAJourHUD()
    {
        if (texteScoreEnJeu != null)
        {
            texteScoreEnJeu.text = "SÉCURITÉ : " + dangersResolus + " / " + totalDangersAValider;
        }
    }

    void VerifierFinNiveau()
    {
        if (dangersResolus >= totalDangersAValider && !niveauFini)
        {
            StartCoroutine(SequenceFinDeNiveau());
        }
    }

    IEnumerator SequenceFinDeNiveau()
    {
        niveauFini = true;
        int scoreTotal = dangersResolus;

        // --- SAUVEGARDE BDD ---
        if (monDatabaseManager != null)
        {
            monDatabaseManager.SauvegarderScoreSiMeilleur(idDuNiveauBDD, scoreTotal);
            monDatabaseManager.AjouterAuScoreTotalProfil(monDatabaseManager.userIdConnecte, scoreTotal);
        }

        if (FeedbackManager.Instance != null) 
            FeedbackManager.Instance.Afficher("ZONE SÉCURISÉE !", Color.cyan);
        
        if (sonVictoire) sonVictoire.Play();

        yield return new WaitForSeconds(2f); 

        // Affichage du panneau
        if(panneauFinNiveau != null) 
        {
            panneauFinNiveau.SetActive(true);
            if(boutonVoirRecords) boutonVoirRecords.SetActive(true);

            if(texteScoreFinal)
            {
                texteScoreFinal.text = "RAPPORT FINAL SÉCURITÉ\n\n" +
                                       "Dangers traités : " + dangersResolus + " / " + totalDangersAValider + "\n" +
                                       "----------------\n" +
                                       "<size=120%>SCORE : <color=green>" + scoreTotal + "</color></size>";
            }
        }
    }

    // ---------------------------------------------------------
    // --- BOUTONS UI ---
    // ---------------------------------------------------------

    public void AfficherClassement()
    {
        if (monDatabaseManager != null)
        {
            string top3 = monDatabaseManager.GetTop3ScoresTexte(idDuNiveauBDD);
            if(texteScoreFinal)
            {
                texteScoreFinal.text = "<color=orange>MEILLEURS AGENTS</color>\n\n" + 
                                       "<size=80%>" + top3 + "</size>";
            }
            if(boutonVoirRecords != null) boutonVoirRecords.SetActive(false);
        }
    }

    public void RetourMenu()
    {
        if (ChargeurScene.Instance != null)
            ChargeurScene.Instance.ChargerScene(nomSceneMenu);
        else
            SceneManager.LoadScene(nomSceneMenu);
    }
}