using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections; 

public class GestionEPI : MonoBehaviour
{
    private static bool estPremierLancement = true;

    [Header("--- AUDIO ---")]
    public AudioSource sourceAudio; // <--- Glisse ton AudioSource ici
    public AudioClip sonPorteOuverture; // <--- Glisse le son "Porte qui s'ouvre" ou "Succès"
    public AudioClip sonAccesRefuse;    // <--- Glisse le son "Erreur" ou "Bip rouge"

    [Header("--- Les Objets EPI ---")]
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable objetCasque;
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable objetGilet;

    [Header("--- Les Sockets du Joueur ---")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socketTete;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socketTorse;

    [Header("--- Les Positions sur la Table ---")]
    public Transform tableSpawnCasque;
    public Transform tableSpawnGilet;

    [Header("--- FEEDBACK : Le Message d'Erreur ---")]
    public GameObject panneauErreur; 
    public float dureeMessage = 3.0f; 

    void Start()
    {
        if (estPremierLancement)
        {
            ForcerSurTable();
            estPremierLancement = false; 
        }
        else
        {
            RandomiserEquipement();
        }
    }

    // --- CETTE FONCTION EST APPELÉE PAR LA PORTE ---
    public void TenterAcces(string nomDeLaScene)
    {
        // On vérifie les EPI à l'instant T
        bool estSecurise = socketTete.hasSelection && socketTorse.hasSelection;

        if (estSecurise)
        {
            // ✅ C'est tout bon : On lance la transition douce
            Debug.Log("Accès autorisé vers " + nomDeLaScene);
            
            // --- JOUER SON OUVERTURE ---
            if (sourceAudio != null && sonPorteOuverture != null)
            {
                sourceAudio.PlayOneShot(sonPorteOuverture);
            }

            if (ChargeurScene.Instance != null)
            {
                ChargeurScene.Instance.ChargerScene(nomDeLaScene);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(nomDeLaScene);
            }
        }
        else
        {
            // ⛔ Pas bon : Message d'erreur
            Debug.Log("Accès refusé ! Mettez vos EPI.");

            // --- JOUER SON ERREUR ---
            if (sourceAudio != null && sonAccesRefuse != null)
            {
                sourceAudio.PlayOneShot(sonAccesRefuse);
            }

            StartCoroutine(AfficherErreurTemporaire());
        }
    }

    IEnumerator AfficherErreurTemporaire()
    {
        if (panneauErreur != null)
        {
            panneauErreur.SetActive(true); 
            yield return new WaitForSeconds(dureeMessage); 
            panneauErreur.SetActive(false); 
        }
    }

    // --- Logique de placement (Inchangée) ---
    void ForcerSurTable()
    {
        if(tableSpawnCasque) { objetCasque.transform.position = tableSpawnCasque.position; objetCasque.transform.rotation = tableSpawnCasque.rotation; }
        if(tableSpawnGilet) { objetGilet.transform.position = tableSpawnGilet.position; objetGilet.transform.rotation = tableSpawnGilet.rotation; }
    }

    void RandomiserEquipement()
    {
        if (Random.value > 0.5f) { objetCasque.transform.position = socketTete.transform.position; objetCasque.transform.rotation = socketTete.transform.rotation; }
        else { objetCasque.transform.position = tableSpawnCasque.position; objetCasque.transform.rotation = tableSpawnCasque.rotation; }
        
        if (Random.value > 0.5f) { objetGilet.transform.position = socketTorse.transform.position; objetGilet.transform.rotation = socketTorse.transform.rotation; }
        else { objetGilet.transform.position = tableSpawnGilet.position; objetGilet.transform.rotation = tableSpawnGilet.rotation; }
    }
}