using UnityEngine;
using TMPro;

public class PorteInfo : MonoBehaviour
{
    public int numeroNiveau = 1; // Niveau 1, 2, 3...
    public TextMeshPro texteScore; // Le texte 3D au dessus de la porte

    // Cette fonction sera appelée par le Menu quand le joueur change
    public void MettreAJourAffichage(DatabaseManager bdd)
    {
        // On demande à la BDD le score du joueur actuel pour CE niveau
        int bestScore = bdd.GetMeilleurScore(numeroNiveau);

        if (bestScore == 0)
        {
            texteScore.text = "Niveau " + numeroNiveau + "\nPas encore joué";
        }
        else
        {
            texteScore.text = "Niveau " + numeroNiveau + "\nMeilleur Score : " + bestScore;
        }
    }
}