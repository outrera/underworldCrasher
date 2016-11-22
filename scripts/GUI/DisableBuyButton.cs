// Das Script deaktiviert einen Kaufbutton, wenn das zu kaufende Objekt zu teuer ist, oder schon gekauft wurde
// Wird dem Button angeheftet, welcher deaktiviert werden soll
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisableBuyButton : MonoBehaviour {

	// Definiere Button, welcher deaktiviert werden soll
	public Button BuyButton;
	// Definiere welches Objekt mit dem Button gekauft werden kann
	public GameObject ToBuyObject;
	// Definiere die Objektkosten aus BuyObject
	private BuyObject TotalCost;
	// Definiere das zur Verfuegung stehende Geld aus PlayerResources
	private PlayerResources availableMoney;
	// Definiere das Haeckchen, welches angezeigt werden soll, wenn Objekt gekauft wurde
	public GameObject checkArrow;

	void Start(){
		// Variable availableMoney enthaelt die Klasse PlayerResources
		availableMoney = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerResources>();

		// Verweise der Variable TotalCost auf die Komponente BuyObject der Variable ToBuyObject
		TotalCost = ToBuyObject.GetComponent<BuyObject> ();
	}

	// Funktion
	void Update () 
	{


		// Verweise der Variable TotalCost auf die Komponente BuyObject der Variable ToBuyObject
		TotalCost = ToBuyObject.GetComponent<BuyObject> ();

		// WENN Objekt gekauft wurde, bzw. aktiv ist
		// DANN deaktiviere den Button und lasse den checkArrow anzeigen
		// ANSONSTEN WENN die Kosten groesser als das verfuegbare Geld sind
		// DANN setze den Kaufbutton auf Inaktiv und setze zusaetzlich den CheckArrow nochmal auf inaktiv
		// ANSONSTEN WENN die Kosten kleiner oder gleich dem verfuegbaren Geld sind
		// DANN setze den Kaufbutton auf Aktiv, aber lasse den checkArrow noch inaktiv
		if (ToBuyObject.activeSelf == true) {
			//
			BuyButton.interactable = false;
			checkArrow.SetActive (true);
		} else if (TotalCost.totalcost > availableMoney.money) {
			BuyButton.interactable = false;
			checkArrow.SetActive (false);
		} else if (TotalCost.totalcost <= availableMoney.money) {
			BuyButton.interactable = true;
			checkArrow.SetActive (false);
		}


	}
}
