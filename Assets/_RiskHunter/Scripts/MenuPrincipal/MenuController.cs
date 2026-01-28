using UnityEngine;

public class MenuController : MonoBehaviour
{
    // Appelle cette fonction sur le bouton "Lancer Tri"
    public void LancerNiveauTri()
    {
        ChargeurScene.Instance.ChargerScene("Zone1"); 
        // ^ Remplace par le VRAI nom de ta scène (ex: "Entrepo_Tri")
    }

    // Appelle cette fonction sur le bouton "Lancer Danger"
    public void LancerNiveauDanger()
    {
        ChargeurScene.Instance.ChargerScene("Zone2");
    }

    public void QuitterJeu()
    {
        Application.Quit();
    }
}