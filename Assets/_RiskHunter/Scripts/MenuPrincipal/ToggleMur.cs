using UnityEngine;
using TMPro; // Indispensable pour modifier le texte du bouton

public class ToggleMur : MonoBehaviour
{
    [Header("Ce qu'on veut cacher/montrer")]
    public GameObject zoneDeTexte; // L'objet qui contient tes paragraphes

    [Header("Le bouton lui-même")]
    public TextMeshProUGUI texteDuBouton; // Pour changer "Masquer" en "Voir"

    private bool estVisible = true; // On commence ouvert

    public void Basculer()
    {
        // 1. On inverse l'état (Vrai devient Faux, Faux devient Vrai)
        estVisible = !estVisible;

        // 2. On applique le changement à l'objet
        zoneDeTexte.SetActive(estVisible);

        // 3. On met à jour le texte du bouton pour guider le joueur
        if (estVisible)
        {
            if(texteDuBouton) texteDuBouton.text = "Masquer [-]";
        }
        else
        {
            if(texteDuBouton) texteDuBouton.text = "Afficher [+]";
        }
    }
}