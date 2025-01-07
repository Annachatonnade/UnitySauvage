using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartRushMode()
    {
        SceneManager.LoadScene("../Scenes/RushMode");     // Chargez la scène associée au mode Rush
    }

    public void StartChronoMode()
    {
        SceneManager.LoadScene("../Scenes/ChronoMode");   // Chargez la scène associée au mode Contre la montre
    }

    public void QuitGame()
    {
        Application.Quit();                     // Ferme l'application
        Debug.Log("Game Quit");                 // Fonctionne uniquement dans la build
    }
}
