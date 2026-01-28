using UnityEngine;

public class TutoProximite : MonoBehaviour
{
    [Header("L'objet à afficher (Canvas ou Panneau)")]
    public GameObject panneauTuto;

    void Start()
    {
        // On cache le tuto au démarrage
        if(panneauTuto) panneauTuto.SetActive(false);
    }

    // Quand le joueur ENTRE dans la zone
   void OnTriggerEnter(Collider other)
       {
           // L'espion nous dit ce qui entre dans la zone
           Debug.Log("Quelque chose est entré : " + other.name + " | Tag : " + other.tag);
   
           if (other.CompareTag("Player"))
           {
               Debug.Log("C'est le joueur ! J'affiche le panneau.");
               panneauTuto.SetActive(true);
           }
       }

    // Quand le joueur SORT de la zone
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            panneauTuto.SetActive(false);
        }
    }
}