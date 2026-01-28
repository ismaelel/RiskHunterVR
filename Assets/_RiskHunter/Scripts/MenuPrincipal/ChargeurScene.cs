using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ChargeurScene : MonoBehaviour
{
    public static ChargeurScene Instance;

    [Header("Réglages UI")]
    public CanvasGroup groupeCanvasNoir; 
    public Slider barreDeProgression;    
    
    [Header("Réglages Temps")]
    public float dureeFondu = 1.0f;          // Vitesse de l'animation (Fade In/Out)
    public float tempsAttenteEcranNoir = 2.0f; // COMBIEN DE TEMPS ON RESTE AU NOIR (Pause)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    void Start()
    {
        // Au lancement, on force le noir direct
        groupeCanvasNoir.alpha = 1f; 
        groupeCanvasNoir.blocksRaycasts = true;

        StartCoroutine(FonduOuverture());
    }

    public void ChargerScene(string nomScene)
    {
        StartCoroutine(SequenceChargement(nomScene));
    }

    // --- LES ANIMATIONS ---

    IEnumerator FonduOuverture()
    {
        // 1. PAUSE AU DÉMARRAGE (Ecran noir fixe)
        // On utilise Realtime pour ignorer la pause du menu
        yield return new WaitForSecondsRealtime(tempsAttenteEcranNoir);

        // 2. DISPARITION DU NOIR
        float timer = 0f;
        while (timer < dureeFondu)
        {
            timer += Time.unscaledDeltaTime; // Important : unscaled pour ignorer la pause
            groupeCanvasNoir.alpha = 1f - (timer / dureeFondu); 
            yield return null;
        }
        groupeCanvasNoir.alpha = 0f;            
        groupeCanvasNoir.blocksRaycasts = false; 
    }

    IEnumerator SequenceChargement(string scene)
    {
        // 1. APPARITION DU NOIR
        groupeCanvasNoir.blocksRaycasts = true;
        float timer = 0f;
        while (timer < dureeFondu)
        {
            timer += Time.unscaledDeltaTime;
            groupeCanvasNoir.alpha = timer / dureeFondu;
            yield return null;
        }
        groupeCanvasNoir.alpha = 1f;

        // 2. CHARGEMENT
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        operation.allowSceneActivation = false; // On empêche la scène de se lancer tout de suite

        while (!operation.isDone)
        {
            float progression = Mathf.Clamp01(operation.progress / 0.9f);
            if(barreDeProgression != null) barreDeProgression.value = progression;

            // Quand le chargement est fini (à 90%)
            if (operation.progress >= 0.9f)
            {
                // 3. PAUSE DE TRANSITION (On force l'attente ici)
                yield return new WaitForSecondsRealtime(tempsAttenteEcranNoir);
                
                operation.allowSceneActivation = true; // C'est bon, on y va !
            }
            yield return null;
        }

        // 4. OUVERTURE DANS LA NOUVELLE SCÈNE
        Canvas monCanvas = GetComponent<Canvas>();
        if (monCanvas.worldCamera == null) monCanvas.worldCamera = Camera.main;

        timer = 0f;
        while (timer < dureeFondu)
        {
            timer += Time.unscaledDeltaTime;
            groupeCanvasNoir.alpha = 1f - (timer / dureeFondu);
            yield return null;
        }
        groupeCanvasNoir.alpha = 0f;
        groupeCanvasNoir.blocksRaycasts = false;
    }
}