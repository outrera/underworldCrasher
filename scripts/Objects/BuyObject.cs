// Dieses Script spawnt ein gekauftes Objek
// Es wird dem Objekt angeheftet, welches gekauft werden soll
// Variablen aus anderen Scripts ansprechen
// http://unitylore.com/articles/script-communication/

using UnityEngine;
using System.Collections;

public class BuyObject : MonoBehaviour {
	
	//public GameObject SpielObjekt;
	public int totalcost;

    // Ingame Name des Objekts
    public string objectName;

	// Name des Gastes der aktiviert werden soll durch das Item
	public string activateGuest;

	// Definiere die Variable available Money vom Typ PlayerResources (public class)
	PlayerResources availableMoney;

    // Wurde Objekt gekauft? (für Speicherfunktion)

    // Bei Aktivierung
    private void Start() {
        // Schleife, die GuestType durchläuft und nach dem Namen sucht, der oben eingegeben wurde
        // Dieser Gasttype wird dann auf freigeschaltet
        foreach (SpawnManager.GuestType guest in SpawnManager.Instance.guestType)
        {
            if (activateGuest == guest.name)
            {
                guest.isGuestAvailable = true;
            }
        }
    }

	// Funktion die checkt, ob das Objekt, welches gekauft werden soll schon aktiv ist. Falls nicht, wird geprueft, ob man es sich leisten kann.
	public void checkIfBuilt(){
		if (gameObject.activeSelf != true) {
			checkcost ();
		} 
	}


	// Definiere Funktion, die uebeprueft, ob Objekt gekauft werden kann
	public void checkcost(){
		// Variable availableMoney enthaelt die Klasse PlayerResources
		availableMoney = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerResources>();
		// WENN availableMoney.money (Wert von PlayerResources) groesser oder Gleich als die ObjektKosten sind
		// DANN spawne es und reduziere availableMoney.money (Variable von PlayerResources) um die Objektkosten.
		// ANSONSTEN kann Objekt nicht gekauft werden (bleibt deaktiviert)
		if (availableMoney.money >= totalcost) {
			spawn ();
			availableMoney.money = availableMoney.money - totalcost;

		} else {
			gameObject.SetActive(false);
		}
	}

	// Funktion um das Objekt zu spawnen bzw. aktiv zu setzen
	public void spawn()
	{
		// Macht Objekt aktiv
		gameObject.SetActive(true);
        		
	}
}