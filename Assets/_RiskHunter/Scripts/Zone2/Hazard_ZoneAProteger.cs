using UnityEngine;
using System.Collections;
using UnityEngine.UI; 
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; 

public class Hazard_ZoneAProteger : MonoBehaviour
{
    [Header("--- Configuration ---")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socketDeProtection; 
    public ManagerNiveauDanger manager; // Lien vers le manager (optionnel si auto)

    [Header("--- LIEN HUD ---")]
    public FeedbackManager leHudManager; 

    [Header("--- Messages & Sons ACCIDENT ---")]
    public string messageBlessure = "ATTENTION DANGER !"; 
    public AudioSource sonAccident; // Le cri ou la chute (One Shot)
    public bool blesserSiOnMarcheDedans = true; 

    [Header("--- AMBIANCE (Bruit de fond) ---")]
    public AudioSource sonAmbianceLoop; 

    [Header("--- Feedback Visuel (Flash) ---")]
    public Canvas canvasFlashRouge; 
    public Image imageSang;   

    private bool peutEtreBlesse = true; 
    private bool estSecurise = false;

    void Start()
    {
        // Recherche automatique du manager si la case est vide
        if (manager == null) manager = FindFirstObjectByType<ManagerNiveauDanger>();
        if (leHudManager == null) leHudManager = FindFirstObjectByType<FeedbackManager>();

        if(socketDeProtection != null)
        {
            socketDeProtection.selectEntered.AddListener(OnProtectionPlacee);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !estSecurise && blesserSiOnMarcheDedans)
        {
            if (socketDeProtection && socketDeProtection.hasSelection) {
                estSecurise = true; return;
            }
            if (peutEtreBlesse) DeclencherAccident();
        }
    }

    void DeclencherAccident()
    {
        if(sonAccident) sonAccident.Play();
        
        if (leHudManager != null) leHudManager.Afficher(messageBlessure, Color.red);

        StartCoroutine(FlashRouge());
        peutEtreBlesse = false;
        Invoke("ResetBlessure", 2.0f);
    }

    public void OnProtectionPlacee(SelectEnterEventArgs args)
    {
        if (!estSecurise)
        {
            estSecurise = true;
            
         
            if (sonAmbianceLoop != null)
            {
                sonAmbianceLoop.Stop(); // Silence !
            }
            // -------------------------------------------

            if(manager != null) manager.ValiderUnDanger();
        }
    }

    // ... (Le reste : ResetBlessure et FlashRouge ne change pas)
    void ResetBlessure() { peutEtreBlesse = true; }
    IEnumerator FlashRouge()
    {
        if(canvasFlashRouge) canvasFlashRouge.gameObject.SetActive(true);
        if (imageSang) {
            Color c = imageSang.color; c.a = 0.7f; imageSang.color = c;
            float timer = 0f;
            while (timer < 1f) {
                timer += Time.deltaTime; c.a = Mathf.Lerp(0.7f, 0f, timer); imageSang.color = c; yield return null;
            }
        }
        if(canvasFlashRouge) canvasFlashRouge.gameObject.SetActive(false);
    }
}