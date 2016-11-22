// Dieses Script steuert den Button für die Drinkphase und wird diesem angehangen.

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NextRound : MonoBehaviour {

	// Rundenvariable
	RoundSystem nextRound;
	// Button für die Drinkphase (Taverne oeffnen)
	public Button roundButton;
	// Text für die Drinkphase, der auf dem Button steht
	public Text roundButtonText;

	public Shop shop;



	// Initialisieren
	void Start (){
		// Auf RoundSystem verweisen, um daraus den Rundenstatus abzufragen
		nextRound = GameObject.FindGameObjectWithTag("GameController").GetComponent<RoundSystem>();

	}

	void Update (){

		// WENN die Drinkphase an ist
		// DANN ist Button nicht anklickbar
		// ANSONSTEN schon
		if (nextRound.DrinkingPhase == true) {
			roundButton.interactable = false;
			roundButtonText.text = "Taverne hat geöffnet";
		} else {
			roundButton.interactable = true;
			roundButtonText.text = "Öffne die Taverne";
		}

	}

	// Funktion für den Button. Wenn man draufklickt, beginnt die Drinkingphase
	// Muss im OnClick vom Button angegeben werden
	public void nextRoundOnClick(){

        // WENN Spiel NICHT pausiert ist, dann passiert folgender Code, wenn man auf den NextRound Button klickt:
        if (!PauseMenu.Instance.Paused) {
            nextRound.DrinkingPhase = true;


            // Schließe den Shop, wenn er offen ist
            if (shop.IsOpen == true) {
                shop.OpenInventoryButton();
            }

            // Starte die Funktion StartSpawn, welche die Coroutine startet
            SpawnManager.Instance.StartSpawn();
        }

		

	}

}
