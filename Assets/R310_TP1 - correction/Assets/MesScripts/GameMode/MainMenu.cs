using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartRushMode()
    {
        SceneManager.LoadScene("RushMode");     // Chargez la sc�ne associ�e au mode Rush
    }

    public void StartChronoMode()
    {
        SceneManager.LoadScene("ChronoMode");   // Chargez la sc�ne associ�e au mode Contre la montre
    }

    public void QuitGame()
    {
        Application.Quit();                     // Ferme l'application
    }
}
