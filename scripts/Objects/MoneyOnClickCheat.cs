// Cheat Script

using UnityEngine;
using System.Collections;

public class MoneyOnClickCheat : MonoBehaviour {

	// Definiere die Variable available Money vom Typ PlayerResources (public class)
	PlayerResources availableMoney;

	// Definieren der Funktion OnMouseOver, welche die Funktion open() aufruft, wenn mit links(0) draufgeklickt wird
	void OnMouseOver(){
		availableMoney = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerResources>();
		if (Input.GetMouseButtonDown (0)) {
			availableMoney.money = availableMoney.money + 50;
		} else if (Input.GetMouseButtonDown (1)) {
			availableMoney.money = availableMoney.money - 50;
		}
		
	}

}
