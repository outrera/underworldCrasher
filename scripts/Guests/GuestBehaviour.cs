// Steuert Gast verhalten
// Wird dem Gast angehangen
// TO DO: Ruf verringern, wenn es keine Mahlzeit gibt

using UnityEngine;
using System.Collections;

public class GuestBehaviour : MonoBehaviour {

	// Wie viel Gold hat der Gast
	public int guestGold;
	// 
	private int itemCounter = 0;
	// Auf welchem Niveau konsumiert der Gast (wie exklusiv muss das Itemlevel sein, dass er konsumieren will)
	public int consumLevel;
	// Wie hoch muss das Konsumniveau mindestens sein, damit er die Speise noch anrührt
	public int consumLevelMin;
	// Wie viel kann der Gast trinken
	public int drinkLimit;
	// Wie viel hat der Gast schon getrunken
	public int drinkCounter = 0;
	// Wie wahrscheinlich ist es, dass der Gast eine Mahlzeit möchte (Wert zwischen 1 und 10)
	public int eatPropability;
	// Hat er seine Mahlzeit schon gegessen?
	private bool didConsumMeal = false;
	// Ist er schon voll?
	private bool didConsumDrink = false;


	// Physics
	// Der NavMesh Agent, damit der Gast über das NavMesh laufen kann
	NavMeshAgent agent;
	// Das Zielobjekt bzw. der Platz wo der Gast hinlaufen soll
	public GameObject target;
	// Hat er einen freien Platz erspäht?
    [SerializeField]
	private bool findSpot = false;
	// Verzögerungszeit, die er braucht bevor er einen Platz sucht (damit nicht alle gleichzeitig suchen und es nicht zu konflikten kommt)
	public float delaySpot;
	// Habe ich den Platz erreicht?
	public bool reachedSpot = false;
	// 
	public bool idle = false;

	public bool sit = false;



	// Use this for initialization
	void Start () {

		// Referenz zum Agent
		agent = GetComponent<NavMeshAgent> ();
		// Aktiviere "FindSpot", mit einer Verzögerungszeit
		Invoke ("FindSpot", delaySpot);
		// Aktiviere das Gastverhalten mit einer Verzögerungszeit
		Invoke("InitGuestbehaviour", 3f);

	}
	
	// Update is called once per frame
	void Update () {
	 
		// WENN endphase ist, dann stoppe coroutine
		if (RoundSystem.Instance.EndPhase == true) {
			StopCoroutine("ConsumItem");
		}
		// WENN du einen Spot gefunden hast und ihn noch NICHT erreicht hast, dann gehe da hin
		if (findSpot == true && reachedSpot == false) {
            
           Walk();
               
		}
    }

 /*   // Update is called once per frame
    void Update() {


        // IF a free spot was found and not yet reached
        if (findSpot == true && reachedSpot == false) {
            // IF the game is NOT paused
            if (!PauseMenu.Instance.Paused) {
                // Walk to the spot!
                Walk();
                // Or resume if you stopped
                agent.Resume();
            }
            else {
                // If the game is paused: STOP!
                agent.Stop();
            }
        }
    }
*/
    // Coroutine, die den Konsum des Gastes steuert
    // Sie wird beim starten des Gastes aufgerufen
    // Der Gast nimmt sich mit der Zeit immer essen und drinken aus dem Lager
    public IEnumerator ConsumItem(){
		// SOLANGE die Coroutine aktiv ist
		while (true) {
			// Esse eine Mahlzeit
			// Hast du schon gegessen? WENN nein, dann:
			if (didConsumMeal == false) {
				
				// Wahrscheinlichkeit zu essen
				// Wie wahrscheinlich ist es, dass du isst?
				if (UnityEngine.Random.Range(1,10) <= eatPropability){
					// Suche erst nach einer Mahlzeit deines Consumlevels
					// Gehe jeden Slot des Inventars durch
					foreach (GameObject slot in Inventory.Instance.AllSlots) {
						// Referenz zum Slot-Component des Slots
						Slot tmp = slot.GetComponent<Slot>();
						// WENN der Slot NICHT leer ist, dann:
						if(!tmp.IsEmpty){
							// WENN das Item, dass du findest:
							// - Konsumierbar ist
							// - Deinem consumLvl entspricht
							// - Eine Mahlzeit ist
							// - Es weniger oder genauso viel kostet, wie du zur Verfügung hast
							// DANN
							if (tmp.CurrentItem.isConsumable == true && tmp.CurrentItem.consumLvl == consumLevel && tmp.CurrentItem.quality == Quality.MEAL && tmp.CurrentItem.guestPrice <= guestGold) {
								// Ziehe die Mahlzeitkosten aus deinem Geldbeutel ab
								guestGold -= tmp.CurrentItem.guestPrice;
								// Rechne die Mahlzeitkosten auf das Gold des Spielers drauf
								PlayerResources.Instance.money += tmp.CurrentItem.guestPrice;
								// Entferne das Item aus dem Lager
								tmp.RemoveItem ();
								// Du bist satt
								didConsumMeal = true;
								// Verlasse die Schleife und fahre mit ConsumItem fort
								break;
							} 
						}
					}
					
					// Esse Mahlzeit deines Consumbereiches
					// Du hast noch nichts gegessen, weil du keine Mahlzeit deines Konsumlevels gefunden hast?
					// Dann nimm eine schlechtere Mahlzeit, die aber deinem Konsumbereich entspricht:
					if (didConsumMeal == false) {
						// Durchsuche das Lager nach einer Mahlzeit:
						foreach (GameObject slot in Inventory.Instance.AllSlots) {
							// Referenz zur Slot-Component des Slots
							Slot tmp = slot.GetComponent<Slot>();
							// WENN der Slot NICHT leer ist, DANN:
							if(!tmp.IsEmpty){
								// WENN das Item, dass du findest:
								// - Konsumierbar ist
								// - Deinem consumBereich entspricht (Kleiner-Gleich Consumlvl aber Größer-Gleich ConsumMinimum ist)
								// - Eine Mahlzeit ist
								// - Es weniger oder genauso viel kostet, wie du zur Verfügung hast
								// DANN
								if(tmp.CurrentItem.isConsumable == true && tmp.CurrentItem.consumLvl <= consumLevel && tmp.CurrentItem.consumLvl >= consumLevelMin && tmp.CurrentItem.quality == Quality.MEAL && tmp.CurrentItem.guestPrice <= guestGold){
									// Ziehe die Mahlzeitkosten aus deinem Geldbeutel ab
									guestGold -= tmp.CurrentItem.guestPrice;
									// Rechne die Mahlzeitkosten auf das Gold des Spielers drauf
									PlayerResources.Instance.money += tmp.CurrentItem.guestPrice;
									// Entferne das Item aus dem Lager
									tmp.RemoveItem ();
									// Du bist satt
									didConsumMeal = true;
									// Verlasse die Schleife und fahre mit ConsumItem fort
									break;
								}
							}
						}
					}
					
				}
				
			}
			
			// Trinke, wenn dein Limit es noch hergibt
			if (drinkCounter < drinkLimit) {
				// Trinke etwas deines Consumbereichs
				// Durchsuche jeden Slot des Lagers
				foreach (GameObject slot in Inventory.Instance.AllSlots) {
					// Referenz zur Slot-Component des Slots
					Slot tmp = slot.GetComponent<Slot>();
					// WENN der Slot NICHT leer ist, DANN:
					if(!tmp.IsEmpty){
						// WENN das Item, dass du findest:
						// - Konsumierbar ist
						// - Deinem consumBereich entspricht (Kleiner-Gleich Consumlvl aber Größer-Gleich ConsumMinimum ist)
						// - Ein Getränk ist
						// - Es weniger oder genauso viel kostet, wie du zur Verfügung hast
						// DANN
						if (tmp.CurrentItem.isConsumable == true && tmp.CurrentItem.consumLvl <= consumLevel && tmp.CurrentItem.consumLvl >= consumLevelMin && tmp.CurrentItem.quality == Quality.DRINK && tmp.CurrentItem.guestPrice <= guestGold) {
							// Ziehe die Getränkekosten aus deinem Geldbeutel ab
							guestGold -= tmp.CurrentItem.guestPrice;
							// Rechne die Getränkekosten auf das Gold des Spielers drauf
							PlayerResources.Instance.money += tmp.CurrentItem.guestPrice;
							// Du wirst betrunken:
							// WENN du Alkohol getrunken hast
							if(tmp.CurrentItem.drinkQUality == DrinkQuality.ALC){
								// DANN erhöhe den drinkCounter um 1
								drinkCounter++;
							  // WENN du Schnaps getrunken hast
							} else if(tmp.CurrentItem.drinkQUality == DrinkQuality.BOOZE){
								// DANN erhöhe den drinkCounter um 2
								drinkCounter += 2;
							}
							// Entferne das Item aus dem Lager
							tmp.RemoveItem ();
							// Unterbreche die Schleife und fahre mit ConsumItem fort
							break;
						} 
					}
					
				}
			  // WENN du dein drinkLimit erreicht hast, dann hast du fertig konsumiert
			} else if(drinkCounter >= drinkLimit){
				didConsumDrink = true;
			}
			// Sekunden Wartezeit, bevor du nochmal konsumierst
			yield return new WaitForSeconds(7f);
		}






	}

	// Gehe zum Spot
	public void Walk(){
		// Ziel des Agenten ist Position des Targets
		agent.SetDestination (target.transform.position);
		// Check if we've reached the destination
		if (!agent.pathPending)
		{
			if (agent.remainingDistance <= agent.stoppingDistance || agent.stoppingDistance <= 1f)
			{
				if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
				{
					reachedSpot = true;
				}
			}
		}
	}


	// Suche freien Spot
	public bool LookForSpot(){

		// Gehe jeden Spot der Spotliste des Spotmanagers durch
		foreach (GameObject spot in SpotManager.Instance.spotList) {
			// Temporärer Spot (Referenz auf die SpotKomponente
			Spot tmp = spot.GetComponent<Spot>();
			// WENN der Platz NICHT besetzt ist
			if (tmp.isTaken==false){
				// DANN ist er dein Ziel
				target = spot;
				// Und er ist jetzt besetzt
				tmp.isTaken = true;
				// LookForSpot erfolgreich
				return true;
			}

		}
		// Keinen Platz gefunden
		return false;
	}

	// Habe ich einen Platz gefunden?
	public void FindSpot(){
		// WENN ich einen freien Platz gesehen habe, habe ich ihn gefunden
		if (LookForSpot () == true) {
			findSpot = true;

		} else {
			// Sonst nicht
			findSpot = false;
		}
	}

	// Funktion die die Itemms eines bestimtmen Typs zaehlt
	// Die Eingabe muss das Item, dessen Typ man zaehlen will rein
	// Die FUnktion gibt die Anzahl des Itemtyps zurueck
	public int ItemsCount(Item itemToCount){
		// Temporaere Variable x = 0
		int x = 0;
		// Eine Schleife die jeden Slot durchsucht, der sich in der Liste AllSlots des Inventorys befindent
		foreach (GameObject slot in Inventory.Instance.AllSlots){
			// Bei jedem durchlauf wird eine Referenz zum aktuellen Slo erstellt
			Slot tmp = slot.GetComponent<Slot>(); // Referenz auf das Slotscript
			
			// WENN der Slot NICHT leer ist
			if (!tmp.IsEmpty == true){
				// DANN ueberpruefe ob der Typ des Items dem Typ des Items des Slots entspricht
				if (itemToCount.type == tmp.CurrentItem.type){
					// Wenn ja, dann zaehle x hoch
					x += tmp.Items.Count;
				} else {
					
				}
			} else {
				
			}
		}
		// Speichere x in itemCounter und setze es anschliessend wieder auf 0
		itemCounter = x;
		x = 0;
		
		// Gebe den itemCounter zurueck
		return (itemCounter);
		
	}

	// Wrapper für die Coroutine
	public void InitGuestbehaviour(){
		StartCoroutine ("ConsumItem");
	}
}
