using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Test
{
    public class UIManager : MonoBehaviour
    {
        public GameObject pauseMenu; 
        private bool isPaused = false;

        void Update()
        {
            // Si el jugador presiona ESC o la tecla P, se pausa o reanuda el juego
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                if (isPaused)
                    ResumeGame();
                else
                    PauseGame();
            }
        }

        public void PauseGame()
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f; 
            isPaused = true;
        }

        public void ResumeGame()
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f; 
            isPaused = false;
        }

        public void LoadMainMenu()
        {
            Time.timeScale = 1f; 
            SceneManager.LoadScene("MainMenu");
        }

        public void PlayGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

    }
}
