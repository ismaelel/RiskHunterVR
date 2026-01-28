using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // Pour détecter si on le tient

public class ObjetFragile : MonoBehaviour
{
    [Header("Réglages Sensibilité")]
    [Tooltip("Vitesse d'impact pour casser (Chute). Plus c'est haut, plus c'est résistant.")]
    public float seuilImpact = 5.0f; 
    
    [Tooltip("Vitesse de mouvement pour casser (Secousse).")]
    public float seuilSecousse = 3.0f;

    [Header("Punition")]
    public int pointsPerdus = 10;
    public AudioSource sonCasse; // Son de verre brisé ou choc

    private ManagerNiveauTri manager;
    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    
    private float cooldownDegats = 0f; // Pour ne pas perdre 50 fois les points en 1 seconde

    void Start()
    {
        manager = FindFirstObjectByType<ManagerNiveauTri>();
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    void Update()
    {
        if (cooldownDegats > 0) cooldownDegats -= Time.deltaTime;

        // DÉTECTION SECOUSSE (Seulement si on tient l'objet)
        if (grabInteractable != null && grabInteractable.isSelected)
        {
            // Si on agite l'objet trop vite
            if (rb.linearVelocity.magnitude > seuilSecousse && cooldownDegats <= 0)
            {
                TriggerDegats("Objet secoué !");
            }
            // Note: Sur Unity plus vieux, utilise rb.velocity au lieu de rb.linearVelocity
        }
    }

    // DÉTECTION CHUTE / CHOC
    void OnCollisionEnter(Collision collision)
    {
        // relativeVelocity = la force de l'impact
        if (collision.relativeVelocity.magnitude > seuilImpact && cooldownDegats <= 0)
        {
            // On vérifie qu'on ne tape pas juste la main du joueur
            if (!collision.gameObject.CompareTag("Player")) 
            {
                TriggerDegats("Choc violent !");
            }
        }
    }

    void TriggerDegats(string raison)
    {
        cooldownDegats = 1.0f; // On est tranquille pendant 1 seconde après un choc

        // Son
        if (sonCasse != null) sonCasse.Play();

        // Effet visuel (Flash Rouge)
        StartCoroutine(FlashRouge());

        // Pénalité Manager
        if (manager != null)
        {
            manager.AppliquerPenalite(pointsPerdus, raison);
        }
    }

    System.Collections.IEnumerator FlashRouge()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            Color oldColor = rend.material.color;
            rend.material.color = Color.red;
            yield return new WaitForSeconds(0.3f);
            rend.material.color = oldColor;
        }
    }
}