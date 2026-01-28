using UnityEngine;
// On n'a même plus besoin de SceneManager ici car le Chargeur s'en occupe !

public class SceneChanger : MonoBehaviour
{
    [Header("Réglages")]
    [Tooltip("Écris ici le nom EXACT de la scène du menu principal")]
    public string nomSceneMenu = "SampleScene";

    public void RetourAuMenu()
    {
        Debug.Log("Clic sur la porte : Retour au menu en cours...");
        
        if (!string.IsNullOrEmpty(nomSceneMenu))
        {
            // --- MODIFICATION ICI ---
            // Au lieu de couper direct, on appelle le fondu au noir
            if (ChargeurScene.Instance != null)
            {
                ChargeurScene.Instance.ChargerScene(nomSceneMenu);
            }
            else
            {
                // Sécurité : Si tu as oublié de mettre le Canvas Chargement dans la scène
                Debug.LogWarning("⚠️ Pas de ChargeurScene trouvé ! Chargement brutal.");
                UnityEngine.SceneManagement.SceneManager.LoadScene(nomSceneMenu);
            }
        }
        else
        {
            Debug.LogError("Erreur : Le nom de la scène du menu n'est pas renseigné !");
        }
    }
}