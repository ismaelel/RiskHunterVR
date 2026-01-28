using UnityEngine;
using UnityEngine.Events;

public class HandleTrigger : MonoBehaviour
{
    public HingeJoint hinge; // La charnière
    public float angleDeclenchement = 40f; // A quel angle ça s'ouvre ?
    
    // On crée un événement Unity pour pouvoir glisser notre SceneChanger dedans
    public UnityEvent onDoorOpen; 

    private bool estOuvert = false;

    void Update()
    {
        
        if (estOuvert) return; // Si déjà ouvert, on ne fait rien

        // On vérifie l'angle actuel de la charnière
        // Note: L'angle peut être négatif selon le sens, donc on prend la valeur absolue (Mathf.Abs)
        if (Mathf.Abs(hinge.angle) >= angleDeclenchement)
        {
            Debug.Log("Poignée enclenchée ! Ouverture...");
            estOuvert = true;
            onDoorOpen.Invoke(); // Déclenche l'action
        }
    }
}