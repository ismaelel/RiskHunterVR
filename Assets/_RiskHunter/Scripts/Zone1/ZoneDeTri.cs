using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; 

public class ZoneDeTri : MonoBehaviour
{
    [Header("Réglages")]
    public string tagAttendu; 
    
    [Header("Audio")]
    public AudioSource sourceAudio;
    
    [Header("Précision")]
    [Tooltip("Marge d'erreur en mètres (0.01 = 1cm). Si un coin dépasse de plus que ça, c'est refusé.")]
    public float margeErreur = 0.01f;

    private ManagerNiveauTri manager;
    private BoxCollider zoneCollider; 

    void Start()
    {
        manager = FindFirstObjectByType<ManagerNiveauTri>();
        zoneCollider = GetComponent<BoxCollider>();

        if (zoneCollider == null)
            Debug.LogError("ERREUR : L'objet '" + gameObject.name + "' doit avoir un Box Collider !");
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(tagAttendu))
        {
            XRGrabInteractable interactable = other.GetComponent<XRGrabInteractable>();
            BoxCollider cartonCollider = other.GetComponent<BoxCollider>();

            // On vérifie que l'objet est encore actif (pas déjà validé)
            if (interactable != null && cartonCollider != null && interactable.enabled)
            {
                // Le joueur a lâché l'objet
                if (!interactable.isSelected)
                {
                    if (EstEntierementDansLaZone(cartonCollider))
                    {
                        ValiderCarton(interactable);
                    }
                }
            }
        }
    }

    bool EstEntierementDansLaZone(BoxCollider boiteAtester)
    {
        Vector3[] coins = GetCoinsDuCollider(boiteAtester);

        foreach (Vector3 coin in coins)
        {
            Vector3 pointLePlusProche = zoneCollider.ClosestPoint(coin);
            float distance = Vector3.Distance(coin, pointLePlusProche);

            if (distance > margeErreur) return false; 
        }
        return true; 
    }

    Vector3[] GetCoinsDuCollider(BoxCollider b)
    {
        Vector3[] points = new Vector3[8];
        Transform t = b.transform;
        Vector3 min = b.center - b.size * 0.5f;
        Vector3 max = b.center + b.size * 0.5f;

        points[0] = t.TransformPoint(new Vector3(min.x, min.y, min.z));
        points[1] = t.TransformPoint(new Vector3(min.x, min.y, max.z));
        points[2] = t.TransformPoint(new Vector3(min.x, max.y, min.z));
        points[3] = t.TransformPoint(new Vector3(min.x, max.y, max.z));
        points[4] = t.TransformPoint(new Vector3(max.x, min.y, min.z));
        points[5] = t.TransformPoint(new Vector3(max.x, min.y, max.z));
        points[6] = t.TransformPoint(new Vector3(max.x, max.y, min.z));
        points[7] = t.TransformPoint(new Vector3(max.x, max.y, max.z));

        return points;
    }

    void ValiderCarton(XRGrabInteractable interactable)
    {
        Debug.Log("Carton parfaitement rangé !");
        
        // --- SON DE VALIDATION ---
        if (sourceAudio != null) 
        {
            sourceAudio.Play(); // <--- AJOUT ICI : On joue le son
        }
        
        interactable.enabled = false; 
        if(manager != null) manager.AjouterCarton();
        if (FeedbackManager.Instance != null) 
            FeedbackManager.Instance.Afficher("Stockage effectué !", Color.green);
    }
    
    void OnDrawGizmos()
    {
        if(GetComponent<BoxCollider>() != null)
        {
            Gizmos.color = new Color(1, 0.92f, 0.016f, 0.4f); 
            Gizmos.DrawCube(transform.position, transform.lossyScale);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, transform.lossyScale);
        }
    }
}