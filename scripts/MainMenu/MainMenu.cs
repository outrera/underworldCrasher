using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public GameObject quitPrompt;
    public GameObject optionPrompt;
    public GameObject menuPanel;
    public GameObject creditPanel;
    

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void StartGame() {
        SceneManager.LoadScene(1);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void OpenPrompt() {
        quitPrompt.SetActive(true);
        CloseCredtis();
    }

    public void ClosePrompt() {
        quitPrompt.SetActive(false);
    }

    public void VolumeControl(float volumeControl) {
        AudioListener.volume = volumeControl;
    }

    public void OpenOptions() {
        optionPrompt.SetActive(true);
        CloseMenuPanel();
        CloseCredtis();
        ClosePrompt();
    }

    public void CloseOptions() {
        optionPrompt.SetActive(false);
        OpenMenuPanel();
    }

    public void OpenMenuPanel() {
        menuPanel.SetActive(true);
    }

    public void CloseMenuPanel() {
        menuPanel.SetActive(false);
    }
    public void OpenCredits() {
        creditPanel.SetActive(true);
        ClosePrompt();
    }
    public void CloseCredtis() {
        creditPanel.SetActive(false);
    }
}
