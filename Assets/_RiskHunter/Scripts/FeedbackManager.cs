using UnityEngine;
using TMPro;
using System.Collections;

public class FeedbackManager : MonoBehaviour
{
    // --- LE SINGLETON (L'Astuce magique) ---
    // Cela permet d'écrire FeedbackManager.Instance depuis n'importe où
    public static FeedbackManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    // ---------------------------------------

    [Header("Réglages")]
    public TextMeshProUGUI texteHUD;
    public float dureeAffichage = 3f; // Temps avant disparition

    private Coroutine currentCoroutine;

    void Start()
    {
        // Au démarrage, on cache le texte
        if(texteHUD) texteHUD.text = "";
    }

    // Fonction à appeler pour afficher un message
    // Exemple : FeedbackManager.Instance.Afficher("Bravo !", Color.green);
    public void Afficher(string message, Color couleur)
    {
        if (texteHUD == null) return;

        texteHUD.text = message;
        texteHUD.color = couleur;

        // Si un message était déjà en train de s'effacer, on annule pour relancer le timer
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(CacherMessageApresDelai());
    }

    IEnumerator CacherMessageApresDelai()
    {
        yield return new WaitForSeconds(dureeAffichage);
        texteHUD.text = ""; // On efface le texte
    }
}