// Basierend auf:
// https://youtu.be/VQe-Wd5JTEo
// Da wird auch erklärt wie man das Pausmenu mit dem Timer kombiniert
// Und basierend auf: http://answers.unity3d.com/questions/692905/c-timer-that-displays-in-mins-and-seconds-1.html

// Dieses Script wird dem GameController (dem Player) angeheftet und kontrolliert die Kaufzeit und die Runden.

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoundSystem : MonoBehaviour {

	// Singleton, um von anderen Klassen auf den RoundSystem zugreifen zu können
	private static RoundSystem instance;
	public static RoundSystem Instance{
		get { 
			if (instance == null){				// Wenn Instanz nicht gesetzt ist, dann setze sie
				instance = GameObject.FindObjectOfType<RoundSystem>();
			}
			return RoundSystem.instance;
		}
	}

	// starting Time ist die deltaTime Zeit (also 1 Sekunde) bzw. die Zeit die für das Rendern des letztgerenderten Frames benoetigt wurde
	private float startingTime;
	// Die Ingame Stunden. Starten bei 18, da um 18 Uhr die Kaufphase startet
	public float ingameHours = 18;
	// Die Ingame Minuten
	public float ingameMinutes = 0;
	public int speed = 4;
	// Die aktuelle Runde
	public int currentRound = 1;
	// Ein Switch, der die DrinkingPhase ein und ausschaltet
	public bool DrinkingPhase;
	//public bool TransitionPhase;
	public bool EndPhase = false;

	// Die Beiden Variablen, die die Textobjekte beinhaltet, auf denen Zeit und Runden dargestellt werden soll
	public Text timeText;
	public Text roundText;

	// Das Endphase Panel, wo die Statistiken angezeigt werden
	public GameObject Endpanel;
	// closeInventories ist eine Steuervariable, die beim RoundEnd die Inventare schließt
	public bool closeInventories;
	// Variable, die die Gäste rausschmeisst
	public bool exitGuests = false;

	// Use this for initialization
	void Start () {


	
	}
	
	// Update is called once per frame
	void Update () {

        // Hauptablauf der Runden
        // WENN die Trinkphase aus ist, dann befinden wir uns in der Kaufphase. Es wird "Kaufphase" statt Uhrzeit angezeigt
        // ANSONSTEN WENN Befinden wir uns in der Drinkphase und es wird die Funktion zum berechnen der IngameZeit aufgerufen
        // ANSONSTEN WENN wir uns in dern endphase befinden, wird RoundEnd aufgerufen
        // 


		if (DrinkingPhase == false && EndPhase == false) {
			timeText.text = "Kaufphase";
		} else if (DrinkingPhase == true && EndPhase == false) {
            // Nur Ingame Time kalkulieren, wenn es nicht pausiert ist
            if (!PauseMenu.Instance.Paused) {
                calculateIngameTime();
            }
            else {
                timeText.text = "Pausiert";
            }

            
		} else if (DrinkingPhase == false && EndPhase == true) {
			RoundEnd();
		}


	}

	// Die Rundenzeit wird dem roundText Objekt uebergeben, damit sie im Interface angezeigt wird
	// Evneutell optimieren! Muss nicht immer OnGui sein, könnte theoretisch auch beim wegklicken der Statistik sein
	void OnGUI(){

		roundText.text = "" + currentRound;
	}

	// Funktion zur Berechnung der Ingame Zeit.
	// Die Zeitberechnung funktioniert, weil die If-Abfragen bei jedem Frame neu aufgerufen werden und neu kalkuliert werden
	void calculateIngameTime(){

		// startingTime soll sich jedes mal um die deltaTime erhoehen. Also jede Sekunde um 1 hoch.
		startingTime += Time.deltaTime;
		

		// ===Minuten-Zaehler===
		// WENN Die Startingtime groesser/gleich 4 ist
		// DANN erhoehe die ingame Minuten um 10 und setze die Startingtime wieder auf 0
		// Damit dauern 10 ingame Minuten 4 echte Sekunden
		if (startingTime >= speed) {
			ingameMinutes += 10;
			startingTime = 0;
		}
		
		// ===Stunden-Zaehler===
		// WENN die ingameMinutes groesser/gleich 60 UND die ingameHours groesser/gleich 23 sind
		// DANN setze die ingame Stunden auf - 1 und erhoehe sie anschließend um 1 und setze die ingame Minuten auf 0
		// ANSONSTEN WENN die ingameMinutes nur groesser/gleich 23 60 sind
		// DANN erhohe die Stunden einfach um 1 und setze die ingame Minuten wieder auf 0
		// Immer wenn 60 Minuten erreicht sind, wird die Stunde um 1 hochgezaehlt, außer bei 23 Uhr. Da wird die Stunde auf 0 gesetzt 
		// und weitergezahelt
		if (ingameMinutes >= 60 && ingameHours >= 23) {
			ingameHours = -1;
			ingameHours += 1;
			ingameMinutes = 0;
		} else if(ingameMinutes >=60) {
			ingameHours += 1;
			ingameMinutes = 0;
		}

		// === Runden Ende ===
		// WENN ingame 3 Uhr morgens erreicht ist
		// DANN beende die Trinkphase, schließe die Inventare, schmeiß die Gäste raus und gehe in die Endphase
		if (ingameHours == 3) {
			DrinkingPhase = false;
			closeInventories = true;
			exitGuests = true;
			EndPhase = true;

		}

		// === Darstellungs Kontrollstruktur ==
		// WENN die ingame Minuten unter 10 sind (also 0, da sie in 10er Schritten berechnet werden)
		// DANN zeige 18:00 Uhr an (statt 18:0 Uhr)
		// ANSONSTEN lasse die extra 0 bei der Darstellung weg (da die Minutenzahlen ab dann eh zweistellig sind
		if (ingameMinutes < 10) {
			timeText.text = "" + Mathf.Round (ingameHours) + " : 0" + Mathf.Round (ingameMinutes) + " Uhr";
		} else {
			timeText.text = "" + Mathf.Round (ingameHours) + " : " + Mathf.Round (ingameMinutes) + " Uhr";
		}
	}

	// Funktion für das RoundEnd
	void RoundEnd(){
		// Resetten der Zeit
		startingTime = 0;
		ingameHours = 18;
		ingameMinutes = 0;

		// Für jeden Gasttyp, der in dem Array aus Gasttypen ist
		foreach (SpawnManager.GuestType Guest in SpawnManager.Instance.guestType) {
			Guest.guestCount = 0;

		}


		// Der Text soll statt der Zeit angezeigt werden
		timeText.text = "Runde beenden...";
		// Das Endpanel wird aktiviert
		Endpanel.SetActive(true);

		// Schliesse das Crafting Menü und das Inventar, wenn es offen ist
		if (closeInventories == true && Inventory.Instance.IsOpen == true) {
			Inventory.Instance.Open ();

		}
		if (closeInventories == true && CraftingBench.Instance.IsOpen == true) {
			CraftingBench.Instance.Open ();

		}
		// Schmeiß alle Objekte raus die "Guest" als Tag haben
		if (exitGuests == true) {
			GameObject[] guests = GameObject.FindGameObjectsWithTag("Guest");
			foreach (GameObject target in guests) {
				GameObject.Destroy(target);
			}
		}
		// Resette Variablen wieder in Ursprungszustand für die nächste Runde
		closeInventories = false;
		exitGuests = false;
	}
}

