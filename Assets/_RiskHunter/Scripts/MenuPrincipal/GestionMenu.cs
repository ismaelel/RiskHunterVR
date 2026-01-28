using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GestionMenu : MonoBehaviour
{
    [Header("---- REFERENCES GENERALES ----")]
    public DatabaseManager bdd;
    // Le script qui met le menu devant les yeux (optionnel mais recommandé)
    public MenuFaceCamera scriptPosition; 

    [Header("---- PANNEAUX (A décocher dans l'Inspector) ----")]
    public GameObject panelProfils;       
    public GameObject panelMenuPrincipal; 
    public GameObject panelCreation;      
    public GameObject panelConfirmDelete; 

    [Header("---- UI PROFILS (Les 3 Slots) ----")]
    public Button[] boutonsSlots;  
    public TMP_Text[] textesSlots; 
    public Button[] boutonsDelete; 

    [Header("---- UI CREATION (Noms Prédéfinis) ----")]
    // Glisse ici tes boutons "Alpha", "Bravo", etc.
    public GameObject[] boutonsNomsPredefinis; 

    [Header("---- UI MENU PRINCIPAL ----")]
    public TMP_Text textJoueurActuel; 
    
    [Header("---- LE HUB (Les Portes 3D) ----")]
    public PorteInfo[] lesPortes; 

    private int slotEnCoursDeTraitement = 0; 

    void Start()
    {
        // 1. Initialisation : On s'assure que le temps tourne
        Time.timeScale = 1f; 
        
        // On cache tout au démarrage
        panelProfils.SetActive(false);
        panelMenuPrincipal.SetActive(false);
        panelCreation.SetActive(false);
        panelConfirmDelete.SetActive(false);

        // 2. Tentative d'Auto-Connexion
        bool aReussi = TenterAutoConnexion();

        if (aReussi)
        {
            Debug.Log("🏠 Retour au Hub - Mode immersif activé (Pas de menu)");
        }
        else
        {
            Debug.Log("👤 Inconnu - Ouverture du choix de profil");
            OuvrirMenuProfils3D();
        }
    }

    // --- LOGIQUE AUTO-LOGIN ---
    bool TenterAutoConnexion()
    {
        if (PlayerPrefs.HasKey("ID_Joueur_Actif"))
        {
            int id = PlayerPrefs.GetInt("ID_Joueur_Actif");
            string nom = bdd.GetNomProfil(id);

            if (nom != "Vide")
            {
                // TRUE = Mode Silencieux (Pas de panneaux, juste mise à jour des portes)
                ConnexionAuProfil(id, nom, true); 
                return true; 
            }
        }
        return false; 
    }

    // --- CŒUR DU SYSTEME : LA CONNEXION ---
    void ConnexionAuProfil(int id, string nom, bool modeSilencieux)
    {
        // 1. Sauvegarde l'état
        bdd.userIdConnecte = id;
        PlayerPrefs.SetInt("ID_Joueur_Actif", id);
        PlayerPrefs.Save();

        // 2. Met à jour l'interface visuelle
        textJoueurActuel.text = "Joueur : " + nom;
        
        // 3. Met à jour les scores sur les portes 3D
        foreach (PorteInfo porte in lesPortes)
        {
            if(porte != null) porte.MettreAJourAffichage(bdd);
        }

        Debug.Log("✅ Connecté en tant que : " + nom);

        // 4. Gestion de l'affichage selon le mode
        if (modeSilencieux)
        {
            // HUB : On cache tout, le joueur est libre
            panelProfils.SetActive(false);
            panelMenuPrincipal.SetActive(false);
            panelCreation.SetActive(false);
            Time.timeScale = 1f; 
        }
        else
        {
            // MANUEL : On affiche le menu principal pour lancer le jeu
            panelProfils.SetActive(false);
            panelCreation.SetActive(false);
            panelMenuPrincipal.SetActive(true);
            
            // On repositionne le menu devant les yeux pour être sûr
            if (scriptPosition != null) scriptPosition.SePlacerMaintenant();
            
            Time.timeScale = 0f; // Pause
        }
    }

    // --- ACTIONS DU JOUEUR ---

    // Appelé par le Totem 3D ou Déconnexion
    public void OuvrirMenuProfils3D()
    {
        panelProfils.SetActive(true);
        panelMenuPrincipal.SetActive(false); 
        panelCreation.SetActive(false);

        // On force le menu à venir devant les yeux
        if (scriptPosition != null) scriptPosition.SePlacerMaintenant();

        RafraichirAffichageSlots();
        Time.timeScale = 0f; // Pause le jeu
    }

    public void ClickSurSlot(int idSlot)
    {
        string nom = bdd.GetNomProfil(idSlot);

        if (nom == "Vide")
        {
            // NOUVEAU PROFIL
            slotEnCoursDeTraitement = idSlot;
            
            // On prépare la liste des noms (on cache ceux déjà pris)
            MettreAJourListeNomsDisponibles();

            panelCreation.SetActive(true);
            panelProfils.SetActive(false); 

            // Repositionnement de sécurité
            if (scriptPosition != null) scriptPosition.SePlacerMaintenant();
        }
        else
        {
            // CHARGEMENT PROFIL EXISTANT -> Mode Manuel (Affiche menu Jouer)
            ConnexionAuProfil(idSlot, nom, false);
        }
    }

    // --- NOUVEAU SYSTÈME DE CRÉATION (SANS CLAVIER) ---
    
    // Cette fonction cache les boutons "Alpha", "Bravo" s'ils sont déjà utilisés
    void MettreAJourListeNomsDisponibles()
    {
        string nom1 = bdd.GetNomProfil(1);
        string nom2 = bdd.GetNomProfil(2);
        string nom3 = bdd.GetNomProfil(3);

        foreach (GameObject bouton in boutonsNomsPredefinis)
        {
            // On lit le texte sur le bouton
            string nomDuBouton = bouton.GetComponentInChildren<TMP_Text>().text;

            // Si ce nom existe déjà ailleurs, on cache le bouton
            if (nomDuBouton == nom1 || nomDuBouton == nom2 || nomDuBouton == nom3)
            {
                bouton.SetActive(false);
            }
            else
            {
                bouton.SetActive(true);
            }
        }
    }

    // Appelé par les boutons de choix (Alpha, Bravo, Sniper...)
    public void ChoisirNomDirect(string nomChoisi)
    {
        // Création en BDD
        bdd.CreerOuSauverProfil(slotEnCoursDeTraitement, nomChoisi, 1, 0);
        
        // Connexion immédiate + Affiche Menu Jouer
        ConnexionAuProfil(slotEnCoursDeTraitement, nomChoisi, false);
    }

    public void AnnulerCreation()
    {
        panelCreation.SetActive(false);
        panelProfils.SetActive(true); // Retour en arrière
    }

    // --- MENU PRINCIPAL ---
    public void LancerLeJeu()
    {
        panelMenuPrincipal.SetActive(false);
        Time.timeScale = 1f; // IMPORTANT : On relance le temps
        SceneManager.LoadScene("MainMenu"); 
    }

    public void Deconnexion()
    {
        OuvrirMenuProfils3D();
    }

    // --- SUPPRESSION ---
    public void DemanderSuppression(int idSlot)
    {
        slotEnCoursDeTraitement = idSlot;
        panelConfirmDelete.SetActive(true);
        panelProfils.SetActive(false);
    }

    public void ConfirmerSuppression()
    {
        bdd.SupprimerProfil(slotEnCoursDeTraitement);
        
        if (PlayerPrefs.GetInt("ID_Joueur_Actif") == slotEnCoursDeTraitement)
        {
            PlayerPrefs.DeleteKey("ID_Joueur_Actif");
        }

        panelConfirmDelete.SetActive(false);
        panelProfils.SetActive(true);
        RafraichirAffichageSlots();
    }

    public void AnnulerSuppression()
    {
        panelConfirmDelete.SetActive(false);
        panelProfils.SetActive(true);
    }

    // --- OUTILS ---
    void RafraichirAffichageSlots()
    {
        for (int i = 0; i < 3; i++)
        {
            int id = i + 1;
            string nom = bdd.GetNomProfil(id);

            if (nom == "Vide")
            {
                textesSlots[i].text = "Nouvelle Partie";
                boutonsDelete[i].gameObject.SetActive(false);
            }
            else
            {
                textesSlots[i].text = nom;
                boutonsDelete[i].gameObject.SetActive(true);
            }
        }
    }
}