// Dieses Script oeffnet ein Panel oder ein Menue (bzw. Macht ein Objekt aktiv) wenn darauf geklickt wird
// Es muss dem GameObjekt angeheftet werden, auf das man draufklicken soll

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ControlPanel : MonoBehaviour {
	
	// Definieren des Gameobjekts, welches aktiviert werden soll
	public GameObject thePanel;
	RoundSystem roundDetect;
	
	// Definieren der Funktion OnMouseOver, welche die Funktion open() aufruft, wenn mit links(0) draufgeklickt wird
	// Solange nicht die Drinkphase oder Endphase aktiv ist
	void OnMouseOver(){
		
		// Es darf nicht in das Eventsystem geklickt werden, damit die Clicks nicht durch die UI gehen
		if(!EventSystem.current.IsPointerOverGameObject()){
			
			roundDetect = GameObject.FindGameObjectWithTag("GameController").GetComponent<RoundSystem>();
			
			if (Input.GetMouseButtonUp (0) && roundDetect.DrinkingPhase == false && roundDetect.EndPhase == false) {
				open();
			}
			
		}
	}
	
	// Funktion open() setzt das oben definierte GameObject Aktiv.
	public void open(){
		thePanel.SetActive (true);
	}

	// Funktion, die oben definiertes Panel schliesst bzw. inaktiv setzt
	public void close()
	{
		thePanel.SetActive(false);
	}


}
