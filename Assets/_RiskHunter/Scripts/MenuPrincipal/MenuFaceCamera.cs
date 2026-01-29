using UnityEngine;
using System.Collections; // Nécessaire pour le délai

public class MenuFaceCamera : MonoBehaviour
{
    [Header("Référence Obligatoire")]
    public Transform cameraTete; 

    [Header("Réglages")]
    public float distance = 1.0f; // 1 mètre devant
    public float hauteurOffset = -0.1f; // Un poil plus bas que les yeux

    void OnEnable()
    {
        // On lance le repositionnement avec un tout petit délai
        // pour être sûr que le casque est bien calé
        StartCoroutine(PlacerAvecDelai());
    }

    // Cette fonction permet d'attendre une fraction de seconde
    IEnumerator PlacerAvecDelai()
    {
        yield return new WaitForEndOfFrame(); // Attend la fin de l'image actuelle
        
        // Si on n'a pas assigné la caméra, on essaye de la trouver
        if (cameraTete == null && Camera.main != null) 
            cameraTete = Camera.main.transform;

        if (cameraTete != null)
        {
            SePlacerMaintenant();
        }
    }

    public void SePlacerMaintenant()
    {
        // 1. On prend la position de la tête
        Vector3 posTete = cameraTete.position;

        // 2. On prend la direction "devant", mais on l'aplatit totalement sur l'horizontale
        Vector3 directionAvant = cameraTete.forward;
        directionAvant.y = 0; // On interdit d'aller vers le haut ou le bas
        directionAvant.Normalize(); // On garde une longueur de 1

        // 3. Calcul de la position finale
        Vector3 posFinale = posTete + (directionAvant * distance);

        // 4. On ajuste la hauteur par rapport à la tête (pas par rapport au sol absolu)
        posFinale.y = posTete.y + hauteurOffset;

        // 5. On applique
        transform.position = posFinale;

        // 6. Rotation : On regarde la tête (en bloquant l'axe vertical pour que le menu reste droit)
        transform.LookAt(new Vector3(posTete.x, transform.position.y, posTete.z));
        transform.Rotate(0, 180, 0); // Demi-tour pour être face au joueur
    }
}