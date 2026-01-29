using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Hazard : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource sourceAudio; 

    private bool dejaTrouve = false;
    private ManagerNiveauTri manager;

    void Start()
    {
        manager = FindFirstObjectByType<ManagerNiveauTri>();
    }

    public void ValiderDanger()
    {
        // Si on l'a déjà trouvé, on arrête tout (on ne rejoue pas le son)
        if (dejaTrouve) return;

        dejaTrouve = true;
        
        // --- 1. JOUER LE SON ---
        if (sourceAudio != null)
        {
            sourceAudio.Play();
        }

        // --- 2. CHANGEMENT VISUEL ---
        GetComponent<Renderer>().material.color = Color.green;

        // --- 3. LOGIQUE JEU ---
        if(manager != null)
        {
            manager.AjouterRisque();
        }

        if (FeedbackManager.Instance != null)
        {
            FeedbackManager.Instance.Afficher("Risque identifié ! (+1)", Color.yellow);
        }
    }
}