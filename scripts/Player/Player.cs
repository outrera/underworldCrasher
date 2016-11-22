using UnityEngine;
using System.Collections;

// Klasse, die den Spieler repraesentiert
public class Player : MonoBehaviour {

	// public Variable, die den Usernamen speichert
	public string username;
	// public Variable, die speichert, ob Spieler ein Mensch ist
	// Eventuell spaeter loeschen, da es nur Menschen gibt?
	public bool human;


	// A reference to the inventory

	public Inventory inventory;
	
	public CraftingBench craftingBench;

    public GameObject pausemenu;
    

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.I))
		{
			inventory.Open();
		}
		if (Input.GetKeyDown(KeyCode.C))
		{

			craftingBench.Open();

		}
        if (Input.GetKeyDown(KeyCode.Escape) && RoundSystem.Instance.EndPhase == false) {

            if (pausemenu.activeSelf==false) {
                pausemenu.SetActive(true);
                PauseMenu.Instance.PauseGame();
                PauseMenu.Instance.PauseMenuStatus();
            }
            else {
                pausemenu.SetActive(false);
                PauseMenu.Instance.PauseGame();
                PauseMenu.Instance.PauseMenuStatus();
            }
            
            
        }
        

    }
}
