using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;
using TMPro; 

public class ManagerNiveauTri : MonoBehaviour
{
    [Header("---- CONFIGURATION ----")]
    public bool compterAutomatiquement = true;
    public int totalCartonsATrier = 5; 
    public int totalRisquesATrouver = 3; 

    [Header("---- SYSTÈME DE POINTS ----")]
    public int pointsParCarton = 100;  
    public int pointsParRisque = 50;   
    
    [Header("---- CHRONOMÈTRE ----")]
    public float tempsDepart = 120f; 
    public int pointsParSeconde = 1;   
    
    private float tempsRestant;
    private bool chronoActif = false;
    
    // Variable pour stocker les points perdus
    private int scoreMalus = 0; 

    public int idDuNiveauBDD = 1; 
    public string nomSceneMenu = "MenuPrincipal";

    [Header("---- UI EN JEU (HUD) ----")]
    public TextMeshProUGUI texteScoreEnJeu; 

    [Header("---- UI FIN DE NIVEAU ----")]
    public GameObject panneauFinNiveau;
    public TextMeshProUGUI texteScoreFinal; 
    public GameObject boutonVoirRecords;    
    public AudioSource sonVictoire;         

    [Header("---- BASE DE DONNÉES ----")]
    public DatabaseManager monDatabaseManager; 

    private int cartonsFaits = 0;
    private int risquesTrouves = 0;
    private bool niveauFini = false;

    void Start()
    {
        tempsRestant = tempsDepart;
        chronoActif = true;
        scoreMalus = 0; 

        if(monDatabaseManager == null) 
            monDatabaseManager = FindFirstObjectByType<DatabaseManager>();
        
        if(panneauFinNiveau) 
            panneauFinNiveau.SetActive(false);

        if (compterAutomatiquement)
        {
            int fragiles = GameObject.FindGameObjectsWithTag("Fragile").Length;
            int inflammables = GameObject.FindGameObjectsWithTag("Inflammable").Length;
            totalCartonsATrier = fragiles + inflammables;
        }

        MettreAJourHUD();
    }

    void Update()
    {
        if (chronoActif && !niveauFini)
        {
            if (tempsRestant > 0)
            {
                tempsRestant -= Time.deltaTime;
                MettreAJourHUD();
            }
            else
            {
                tempsRestant = 0;
                chronoActif = false;
            }
        }
    }

    // --- ACTIONS ---

    public void AjouterCarton()
    {
        cartonsFaits++;
        MettreAJourHUD();
        VerifierFinNiveau();
    }

    public void AjouterRisque()
    {
        risquesTrouves++;
        MettreAJourHUD();
    }

    public void AppliquerPenalite(int pointsPerdus, string raison)
    {
        scoreMalus += pointsPerdus;
        
        if (FeedbackManager.Instance != null)
        {
            FeedbackManager.Instance.Afficher(raison + " (-" + pointsPerdus + ")", Color.red);
        }

        MettreAJourHUD();
    }

    // --- AFFICHAGE ---

    void MettreAJourHUD()
    {
        if (texteScoreEnJeu != null)
        {
            float minutes = Mathf.FloorToInt(tempsRestant / 60);
            float secondes = Mathf.FloorToInt(tempsRestant % 60);
            string tempsFormatte = string.Format("{0:00}:{1:00}", minutes, secondes);
            string couleurTemps = tempsRestant < 30 ? "<color=red>" : "<color=white>";

            // Calcul du score (On accepte les négatifs maintenant !)
            int scoreActuel = (cartonsFaits * pointsParCarton) + (risquesTrouves * pointsParRisque) - scoreMalus;
            
            // Couleur du score : Rouge si négatif, Blanc si positif
            string couleurScore = scoreActuel < 0 ? "<color=red>" : "<color=white>";

            texteScoreEnJeu.text = "Tri : " + cartonsFaits + "/" + totalCartonsATrier + "\n" +
                                   "Risques : " + risquesTrouves + "/" + totalRisquesATrouver + "\n" +
                                   "Pénalités : -" + scoreMalus + "\n" +
                                   "Score : " + couleurScore + scoreActuel + " pts</color>\n" +
                                   couleurTemps + "Temps : " + tempsFormatte + "</color>";
        }
    }

    void VerifierFinNiveau()
    {
        if (cartonsFaits >= totalCartonsATrier && !niveauFini)
        {
            StartCoroutine(SequenceFinDeNiveau());
        }
    }

    IEnumerator SequenceFinDeNiveau()
    {
        niveauFini = true;
        chronoActif = false;

        // --- CALCUL FINAL ---
        int ptsCartons = cartonsFaits * pointsParCarton;
        int ptsRisques = risquesTrouves * pointsParRisque;
        int ptsBonusTemps = Mathf.FloorToInt(tempsRestant * pointsParSeconde);
        
        // On accepte le score négatif ici aussi
        int scoreTotal = (ptsCartons + ptsRisques + ptsBonusTemps) - scoreMalus;
        
        Debug.Log($"Total: {scoreTotal} (Malus: {scoreMalus})");

        if (monDatabaseManager != null)
        {
            monDatabaseManager.SauvegarderScoreSiMeilleur(idDuNiveauBDD, scoreTotal);
            monDatabaseManager.AjouterAuScoreTotalProfil(monDatabaseManager.userIdConnecte, scoreTotal);
        }

        if (FeedbackManager.Instance != null) 
            FeedbackManager.Instance.Afficher("TERMINÉ ! Score: " + scoreTotal, Color.cyan);
        
        if (sonVictoire) sonVictoire.Play();

        yield return new WaitForSeconds(2f); 

        if(panneauFinNiveau != null) 
        {
            panneauFinNiveau.SetActive(true);
            if(boutonVoirRecords) boutonVoirRecords.SetActive(true);

            if(texteScoreFinal)
            {
                float m = Mathf.FloorToInt(tempsRestant / 60);
                float s = Mathf.FloorToInt(tempsRestant % 60);
                string tpsFinal = string.Format("{0:00}:{1:00}", m, s);
                
                // Si le score est négatif, on l'affiche en rouge
                string couleurFinale = scoreTotal < 0 ? "red" : "yellow";

                texteScoreFinal.text = "RAPPORT DE FIN DE POSTE\n\n" +
                                       "Gains : " + (ptsCartons + ptsRisques) + " pts\n" +
                                       "Bonus Temps : +" + ptsBonusTemps + " (" + tpsFinal + ")\n" +
                                       "<color=red>Pénalités : -" + scoreMalus + "</color>\n" +
                                       "----------------\n" +
                                       "<size=120%>SCORE FINAL : <color=" + couleurFinale + ">" + scoreTotal + "</color></size>";
            }
        }
    }

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
            if(boutonVoirRecords != null) 
                boutonVoirRecords.SetActive(false);
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