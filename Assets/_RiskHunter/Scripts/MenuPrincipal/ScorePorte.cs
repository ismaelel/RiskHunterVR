using UnityEngine;
using TMPro; // N'oublie pas ça pour le texte

public class ScorePorte : MonoBehaviour
{
    [Header("Réglages")]
    public int idDuNiveau = 1; // Mets 1 pour le Tri, 2 pour l'Incendie...
    
    [Header("UI")]
    public TextMeshProUGUI texteAffiche; // Glisse le texte ici
    public string texteSiJamaisJoue = "Non joué";

    void Start()
    {
        // On cherche le manager
        DatabaseManager db = FindFirstObjectByType<DatabaseManager>();

        if (db != null && texteAffiche != null)
        {
            // On utilise TA fonction existante
            int meilleurScore = db.GetMeilleurScore(idDuNiveau);

            if (meilleurScore > 0)
            {
                texteAffiche.text = "MON RECORD : <color=yellow>" + meilleurScore + " pts</color>";
            }
            else
            {
                texteAffiche.text = "<color=grey>" + texteSiJamaisJoue + "</color>";
            }
        }
    }
}