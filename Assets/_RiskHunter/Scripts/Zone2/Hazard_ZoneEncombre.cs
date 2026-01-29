using UnityEngine;

public class Hazard_ZoneEncombre : MonoBehaviour
{
    [Header("--- Configuration ---")]
    [Tooltip("Le Tag des objets qui bloquent le passage (ex: 'Obstacle' ou 'Carton')")]
    public string tagObstacle = "Obstacle"; 

    private int objetsBloquants = 0;
    private bool estSecurise = false;
    private ManagerNiveauDanger manager;

    void Start()
    {
        manager = FindFirstObjectByType<ManagerNiveauDanger>();
    }

    // Au début, on compte combien d'obstacles sont DÉJÀ dans la zone
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagObstacle))
        {
            objetsBloquants++;
        }
    }

    // Quand on sort un objet de la zone
    void OnTriggerExit(Collider other)
    {
        if (estSecurise) return;

        if (other.CompareTag(tagObstacle))
        {
            objetsBloquants--;
            
            // Si le compteur tombe à 0, c'est gagné !
            if (objetsBloquants <= 0)
            {
                objetsBloquants = 0; // Sécurité
                ValiderZone();
            }
        }
    }

    void ValiderZone()
    {
        estSecurise = true;
        Debug.Log("Zone dégagée !");
        
        // Feedback Visuel (ex: lumière verte au dessus de l'extincteur)
        // Light lumiere = GetComponentInChildren<Light>();
        // if(lumiere) lumiere.color = Color.green;

        if(manager != null) manager.ValiderUnDanger();
    }
}