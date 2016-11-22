// Dieses Script verwaltet die items und macht sie benutzbar
// Es wird dem jweiligen Item angehangen

using UnityEngine;
using System.Collections;

public enum ItemType {MANA, HEALTH, WATER, BARLEY, BUTTERBREAD, BEER};
public enum Quality {INGRED, MEAL, DRINK, CANBEUSED, CANNOTBEUSED};
public enum DrinkQuality {NONALC, ALC, BOOZE};


public class Item : MonoBehaviour {

	/// The current item type
	public ItemType type;

	public Quality quality;

	public DrinkQuality drinkQUality;

	/// The item's neutral sprite
	public Sprite spriteNeutral;

	/// The item's highlighted sprite
	public Sprite spriteHighlighted;

	/// The max amount of times the item can stack
	public int maxSize;

	// Item Kaufpreis
	public int price;

	public int sellPrice;

	public int guestPrice;

	//Geld Ressource
	private PlayerResources availableMoney;

	//Objektrefernz auf das Inventar
	public Inventory inventory;

	// Unendlich viele Objekte im Shop?
	public bool infiniteItems;	

	// Kann das Item vom Spieler verkauft werden?
	public bool isSellable;
	// Kann das Item von Gästen konsumiert werden?
	public bool isConsumable;
	// Name des Items
	public string itemName;
	// Beschreibung
	public string description;
	// Auf welchem Level ist das Item
	public int consumLvl;

	void Start(){

		// Referenz zur Geldressource
		availableMoney = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerResources>();

	}

	/// Kauft Items aus dem Shop
	public bool Use(){

		// Steuervariable, die überprüft, ob der ShopSlot das Item aus dem Shop entfernen muss oder nicht
		bool checkIfAddAble = false;
		switch (type) {  //Checks which kind of item this is
		case ItemType.MANA:
			Debug.Log("I just used a mana potion");
			// WENN Im Inventar mehr als 0 Slots frei sind (es nicht voll ist)
			// DANN ziehe Preis ab und fuege es dem Inventar zu
			if(inventory.AddItem(gameObject.GetComponent<Item>())){
				availableMoney.money -= price;
				//inventory.AddItem(gameObject.GetComponent<Item>());
				checkIfAddAble = true;
			}
			break;
		case ItemType.HEALTH:
			Debug.Log("I just used a health potion");
			if(inventory.AddItem(gameObject.GetComponent<Item>())){
				availableMoney.money -= price;
				//inventory.AddItem(gameObject.GetComponent<Item>());
				checkIfAddAble = true;
			}
			break;
		case ItemType.WATER:
			Debug.Log("I just used a health potion");
			if(inventory.AddItem(gameObject.GetComponent<Item>())){
				availableMoney.money -= price;
				//inventory.AddItem(gameObject.GetComponent<Item>());
				checkIfAddAble = true;
			}
			break;
		case ItemType.BARLEY:
			Debug.Log("I just used a health potion");
			if(inventory.AddItem(gameObject.GetComponent<Item>())){
				availableMoney.money -= price;
				//inventory.AddItem(gameObject.GetComponent<Item>());
				checkIfAddAble = true;
			}
			break;
		case ItemType.BUTTERBREAD:
			Debug.Log("I just used a health potion");
			if(inventory.AddItem(gameObject.GetComponent<Item>())){
				availableMoney.money -= price;
				//inventory.AddItem(gameObject.GetComponent<Item>());
				checkIfAddAble = true;
			}
			break;
		}

		// WENN es oben auf true gesetzt wird (wenn es geadded werden konnte), dann returne truhe
		// ANSONSTEN nicht
		if (checkIfAddAble == true) {
			return(true);
		} else {
			return(false);
		}

	}

	// Verkauft Items aus dem Inventar
	public void Sell(){
		switch (type) {  //Checks which kind of item this is
		case ItemType.MANA:
			Debug.Log("I just used a mana potion");
			availableMoney.money += price/2; // Erhoehe das Geld um die Haelfte des Preises
			break;
		case ItemType.HEALTH:
			Debug.Log("I just used a health potion");
			availableMoney.money += price/2;
			break;
		}
	}

	// Funktion den Tooltip des Items generiert
	public string GetTooltip(){

		// Eigenschafen String
		string stats = string.Empty;
		// Farbe des Items
		string color = string.Empty;
		// String der Zeilenumbruch speichert
		string newLine = string.Empty;

		// WENN die beschreibung nicht leer ist, speichert newLine einen Zeilenumbruch
		if (description != string.Empty) {
			newLine = "\n";
		}

		// SwitchCase Funktion fuer die Qualitaet
		switch (quality) {
		case Quality.INGRED:  // Bei Qualitaet Ingred = white
			color = "white";
			break;
		case Quality.MEAL:   // Bei Qualitaet Meal = lime
			color = "lime";
			break;
		case Quality.DRINK:   // Bei Qualitaet Drink = lime
			color = "lime";
			break;
		case Quality.CANBEUSED:  // Bei Qualitaet Canbeused = navy
			color = "navy";
			break;
		case Quality.CANNOTBEUSED: // Bei Qualitaet Cannotbeused = orange
			color = "orange";
			break;
		}

		// WENN der Preis groesser als 0 ist, dann zeige ihn an
		if (price > 0) {
			// Zeilenumbruch + Kaufpreis: x Gold
			stats += "\n" + "Kaufpreis: " + price.ToString() + " Gold";
		}
		// WENN der Verkaufspreis groesser als 0 ist, dann zeige ihn an
		if (sellPrice > 0) {
			// Zeilenumbruch + Verkaufspreis: x Gold
			stats += "\n" + "Verkaufspreis: " + sellPrice.ToString() + " Gold";
		}
		// WENN der Gastpreis groesser als 0 ist, dann zeige ihn an
		if (guestPrice > 0) {
			// Zeilenumbruch + ...
			stats += "\n" + "Preis für Gäste: " + guestPrice.ToString() + " Gold";
		}
		// ... (wie oben)
		if (isSellable == false) {
			stats += "\n" + "Item kann nicht verkauft werden";
		}
		if (isConsumable == false) {
			stats += "\n" + "Item kann nicht konsumiert werden";
		}

		// Gibt an in welchem format und mit welcher Struktur der string zurueckgegeben wird
		return(string.Format("<color=" + color + "><size=16>{0}</size></color><size=14><i><color=lime>"+newLine+"{1}</color></i>{2}</size>",itemName,description,stats));
	}
}
