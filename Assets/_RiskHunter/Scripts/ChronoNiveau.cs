using UnityEngine;
using TMPro; // Important pour le texte

public class ChronoNiveau : MonoBehaviour
{
    [Header("Réglages")]
    public float tempsDepart = 120f; // 2 minutes par exemple
    public bool chronometreActif = false;

    [Header("Interface")]
    public TextMeshProUGUI texteChrono; 

    private float tempsRestant;
    
    // Instance statique pour y accéder facilement
    public static ChronoNiveau Instance;

    void Awake()
    {
        Instance = this;
        tempsRestant = tempsDepart;
    }

    void Start()
    {
        chronometreActif = true; // On lance le chrono au début
    }

    void Update()
    {
        if (chronometreActif && tempsRestant > 0)
        {
            tempsRestant -= Time.deltaTime;
            
            // Si on arrive à 0, on arrête
            if (tempsRestant <= 0)
            {
                tempsRestant = 0;
                chronometreActif = false;
                Debug.Log("⏰ Temps écoulé !");
            }

            MettreAJourUI();
        }
    }

    void MettreAJourUI()
    {
        if (texteChrono != null)
        {
            // Convertit en minutes:secondes (ex: 01:30)
            float minutes = Mathf.FloorToInt(tempsRestant / 60);
            float secondes = Mathf.FloorToInt(tempsRestant % 60);
            texteChrono.text = string.Format("{0:00}:{1:00}", minutes, secondes);
            
            // Petit effet rouge si moins de 10 secondes
            if(tempsRestant < 10) texteChrono.color = Color.red;
            else texteChrono.color = Color.white;
        }
    }

    // Fonction à appeler quand le niveau est fini
    public int ArreterEtCalculerBonus()
    {
        chronometreActif = false;
        // Exemple : 10 points par seconde restante
        int bonus = Mathf.RoundToInt(tempsRestant * 10);
        return bonus;
    }
}