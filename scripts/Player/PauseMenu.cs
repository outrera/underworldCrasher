using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    // pasued status. is game paused or not?
    private bool paused = false;

    // Singleton of PauseMenu
    private static PauseMenu instance;

    public static PauseMenu Instance
    {
        get
        {
            if (instance == null) {
                instance = GameObject.FindObjectOfType<PauseMenu>();
            }
            return PauseMenu.instance;
        }

    }

    // property of paused
    public bool Paused
    {
        get
        {
            return paused;
        }
    }

    // Is pauseMenu Panel open?
    public bool pauseMenuStatus = false;
    public GameObject pauseMenu;

    // Function to change the paused status
    public void PauseGame() {
        // Opposit auf current status
        paused = !paused;
        if (paused == true) {
            RoundSystem.Instance.timeText.text = "Pausiert";
            Time.timeScale = 0;
           
        }
        else {
            Time.timeScale = 1;
        }

    }

    // Function to return to MainMenu
    public void BackToMainMenu() {
        SceneManager.LoadScene(0);
    }

    // Function to change the pauseMenuStatus to true when it is active (it is active if paused is true, that is handlet in Player.cs)
    public void PauseMenuStatus() {
        if (pauseMenu.activeSelf == true) {
            
            pauseMenuStatus = true;
            
        }
        else {
            pauseMenuStatus = false;
        }
    }
}
