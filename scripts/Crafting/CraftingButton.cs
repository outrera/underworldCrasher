/*
// Dieses Script verwaltet das Crafting Menü
// Es wird den Buttons im Crafting menü angehangen und zeigt dann immer die Crafting Formel an

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CraftingButton : MonoBehaviour {


	//public Sprite[] arrayOfSprites;
	public Item[] arrayOfItems;
	public GameObject craftingPanel;
	// Slot Prefab (vom Slot Image)
	public GameObject slotPrefab;
	// Inventorygroese (vom Layout her)
	private RectTransform panelRect;
	// Anzahl der groesse der Slots (vom Layout her)
	public float slotSize = 30;
	// Referenz zum Canvas in dem das Inventar ist
	public Canvas inventoryCanvas;

	// Anzahl der Abstände zwischen der Slots, links und oben zu einander
	public float slotPaddingLeft = 30, slotPaddingTop = 30;

	/// Indicates the number of items stacked on the slot as Text
	public Text formularTxt;

	public Text stackTxt;



	public Stack<Item> Items;

	
	private int itemCounter;

	public string slotName;

	public bool isActive;




	/// The slot's empty sprite
	//public Sprite craftSprite;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void killChildren(){
		// Bevor das craftingPanel gezeichnet wird, sollen zuerst alle Children zerstoert werden
		// Damit es nicht zu ueberlagerungen des Layouts kommt
		// Es duerfen immer nur die Children angezeigt werden, dessen Button man geklickt hat
		// http://answers.unity3d.com/questions/611850/destroy-all-children-of-object.html
		foreach (Transform child in craftingPanel.transform) {
			GameObject.Destroy(child.gameObject);
		}
	}

	public void CreateLayout(){
		// WENN mehr Zutaten als 1 drinn sind 
		// DANN erstelle das Layout
		if (arrayOfItems.Length > 1) { 


			
			// panelRect wird auf die RectTransform Komponente des Objektes (im Inspektor zu sehen) verwiesen
			panelRect = craftingPanel.GetComponent<RectTransform> ();


			for (int c = 0; c < arrayOfItems.Length; c++) {
				// Instantiates the slot and creates a reference to it
				GameObject newSlot = (GameObject)Instantiate (slotPrefab);

				// Makes a reference to the rect transform of the Slot
				RectTransform slotRect = newSlot.GetComponent<RectTransform> ();

				// Refernz zum Bild des Slots
				Image slotImage = newSlot.GetComponent<Image> ();

				Item itemTmp = arrayOfItems[c].GetComponent<Item>();
				// Bild wird aus dem arrayOfSprites hinzugefuegt
				slotImage.sprite = itemTmp.spriteNeutral; 

				// Sets the Objektnamens
				newSlot.name = slotName;

				// Sets the craftingPanel as the parent (mit this bekommt es den gleichen Parent wie "this" ( Das Inventarscript) of the slots, so that it will be visible on the screen
				newSlot.transform.SetParent (craftingPanel.transform);

				// Sets the slots position
				//slotRect.localPosition = panelRect.localPosition;
				slotRect.transform.localPosition = new Vector3 (0 + (c*slotSize*2), -30);
		
				// Sets the size of the slot
				slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotSize * inventoryCanvas.scaleFactor);
				slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize * inventoryCanvas.scaleFactor);



				// Text --------------------------------
				//Creates a reference to the stackTxt's recttransform
				RectTransform txtRect = formularTxt.GetComponent<RectTransform> ();
				
				// Calculates the scalefactor of the text by taking 60% of the slots width
				int txtScaleFactor = (int)(slotRect.sizeDelta.x * 0.60);
				
				// Sets the min and max textSize of the stackTxt
				formularTxt.resizeTextMaxSize = txtScaleFactor;
				formularTxt.resizeTextMinSize = txtScaleFactor;

				//Sets the actual size of the txtRect
				txtRect.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, slotRect.sizeDelta.x);
				txtRect.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, slotRect.sizeDelta.y);


				// Referenz auf die Children (Texte) von new Slot
				Text slotTxt = newSlot.GetComponentInChildren<Text>();

				int controlVariable = arrayOfItems.Length - c;

				if (controlVariable > 2){
					slotTxt.text = "+";
				} else if (controlVariable == 2) {
					slotTxt.text = "=";
				} else if (controlVariable < 2){
					slotTxt.text = "";
				}


				checkItemCount(newSlot, c);
				// Referenz auf die Children (Texte) von new Slot
				//Text[] tempStackTxt = newSlot.GetComponentsInChildren<Text>();

				//int countTmp = ItemsCount(arrayOfItems[c]);
			
				//tempStackTxt[1].text = countTmp.ToString();
			}
		}
	}


	// Funktion die die Itemms eines bestimtmen Typs zaehlt
	// Die Eingabe muss das Item, dessen Typ man zaehlen will rein
	// Die FUnktion gibt die Anzahl des Itemtyps zurueck
	public int ItemsCount(Item itemToCount){
		// Temporaere Variable x = 0
		int x = 0;
		// Eine Schleife die jeden Slot durchsucht, der sich in der Liste AllSlots des Inventorys befindent
		foreach (GameObject slot in Inventory.AllSlots){
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

	// Funktion die die Anzahl der Items als Text anzeigt anzeigt
	// Sie erwartet einen Slot und die Nummer des Schleifendurchgangs
	public void checkItemCount(GameObject newSlot, int c){
		// Referenz auf die Children (Texte) von new Slot, die dann in der Hierarchie erscheinen
		Text[] tempStackTxt = newSlot.GetComponentsInChildren<Text>();
		// Die Funktion ItemsCount() soll für das Item aus dem Array aufgerufen werden, bei dem sich die Schleife gerade befindet
		int countTmp = ItemsCount(arrayOfItems[c]);
		// Die Anzahl die in countTmp steckt soll an an das zweite Kind (Liste geht bei 0 los, daher 1) uebergeben werden
		// Das zweite Kindobjekt des Items Slots ist der Text mit der Anzahl der Objekte.
		tempStackTxt[1].text = countTmp.ToString();
	}

	public void checkItemCountAlways(){

	}

	void FixedUpdate(){
		if (GameObject.Find (slotName) == true) {

			int d = 0;
			foreach (Transform child in craftingPanel.transform) {

				Text[] tempStackTxt = child.GetComponentsInChildren<Text>();
				// Die Funktion ItemsCount() soll für das Item aus dem Array aufgerufen werden, bei dem sich die Schleife gerade befindet
				//Debug.Log (child);

				int countTmp = ItemsCount(arrayOfItems[d]);
				//Debug.Log(countTmp);
				// Die Anzahl die in countTmp steckt soll an an das zweite Kind (Liste geht bei 0 los, daher 1) uebergeben werden
				// Das zweite Kindobjekt des Items Slots ist der Text mit der Anzahl der Objekte.
				tempStackTxt[1].text = countTmp.ToString();
				d++;
			}
		} else {

		}

	}
}
*/