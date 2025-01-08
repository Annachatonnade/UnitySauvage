using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePauseMenu : MonoBehaviour
{
    public GameObject pauseMenu; // Panel du menu de pause
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Appuie sur Échap
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenu.SetActive(true); // Affiche le menu pause
        Time.timeScale = 0f;      // Met le jeu en pause
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false); // Cache le menu pause
        Time.timeScale = 1f;        // Reprend le jeu
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Reprendre le temps avant de retourner au menu principal
        SceneManager.LoadScene("MainMenu");
    }

}
