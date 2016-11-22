// https://www.youtube.com/watch?v=KLaGkc87dDQ
// Dieses Script verwaltet das Inventar
// https://www.youtube.com/watch?v=AzkRpc1SMJc
// Dieses Script ist ein abgewandeltes Inventar Script. Es ist also durchaus moeglich, dass ueberschneidungen vorkommen
// Oder manche Kommentare noch nicht aktualisiert wurden

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Shop : MonoBehaviour {
	
	// Inventorygroese (vom Layout her)
	private RectTransform inventoryRect;
	
	// Breite und Hoehe des Inventars
	private float inventoryWidth, inventoryHight;
	
	//Anzahl der Slots
	public int slots;
	
	// Anzahl der Reihen
	public int rows;
	
	// Anzahl der Abstände zwischen der Slots, links und oben zu einander
	public float slotPaddingLeft, slotPaddingTop;
	
	// Anzahl der groesse der Slots (vom Layout her)
	public float slotSize;

	// Referenz zum Canvas in dem das Inventar ist
	public Canvas inventoryCanvas;
	
	// Slot Prefab (vom Slot Image)
	public GameObject slotPrefab;
		
	// Liste
	private List<GameObject> allSlots;

	// Canvas Group zum ein und ausfaden
	private static CanvasGroup canvasGroup;
	// Property der Canvas Group, um von außen darauf zugreifen zu können
	public static CanvasGroup CanvasGroup{
		get { return Shop.canvasGroup; }
	}
	
	// Canvas Fade Variable
	private bool fadingIn;
	private bool fadingOut;
	public float fadeTime;
	
	// Anzahl der leeren Slots
	// Statisch, damit es nur eine Instanz davon gibt
	private static int emptySlot;
	
	// Eigenschaft der leeren Slots, die public ist, damit andere Klassen darauf zugreifen können
	// Statisch, damit es nur eine Instanz davon gibt
	public static int EmptySlot{
		get { return emptySlot;}
		set { emptySlot = value;}
		
	}

	// Tooltip Variablen
	public GameObject tooltipObject;
	private static GameObject tooltip;
	private static Text sizeText;
	public Text sizeTextObject;
	private static Text visualText;
	public Text visualTextObject;

	// Variable und Propertie die steuert, ob Shop open ist
	private bool isOpen = false;
	public bool IsOpen
	{
		get { return isOpen; }
		set { isOpen = value; }
	}

	// Variable die überprüft, ob Maus über dem Shop ist
	public bool mouseInsideShop = false;

	// Rundensystem
	RoundSystem roundDetect;

	// Button zum öffnen des Schops
	public Button shopButton;
	
	// Use this for initialization
	void Start () {
		roundDetect = GameObject.FindGameObjectWithTag("GameController").GetComponent<RoundSystem>(); // Referenz auf das Rundensystem
		tooltip = tooltipObject;
		sizeText = sizeTextObject;
		visualText = visualTextObject;
		//Refernz zur Canvas Group
		canvasGroup = gameObject.GetComponent<CanvasGroup> ();
		
		// Wenn Inventar "gestartet" wird, soll das Layout erstellt werden
		CreateLayout ();
	}
	
	// Update is called once per frame
	void Update () {
		// WENN die Drinkphase an ist
		// DANN ist Button nicht anklickbar
		// ANSONSTEN schon
		if (roundDetect.DrinkingPhase == true) {
			shopButton.interactable = false;
		} else {
			shopButton.interactable = true;
		}

		// Fading des Inventars
		// WENN I gedrueckt wird
		if (Input.GetKeyDown (KeyCode.B) && PauseMenu.Instance.pauseMenuStatus == false) {
			
			// WENN der Alpha groesser als 0 ist, ist der Shop geoeffnet und die Coroutine zum FadeOut wird gestartet
			// ANSONSTEN ist das Shopmenue geschlossen und die Coroutine zum FadeIn wird gestartet
			if(canvasGroup.alpha > 0){
				
				StartCoroutine("FadeOut");
				isOpen=false;
				HideToolTip ();

			} else {
				StartCoroutine("FadeIn");
				isOpen=true;
			}
		}
	}

	// Funktionen, die überprüfen ob Maus über dem Shop ist
	public void PointerExit(){
		mouseInsideShop = false;
	}
	public void PointerEnter(){
		if (canvasGroup.alpha > 0){			
			mouseInsideShop = true;
		}
	}

	// Funktion die Tooltip anzeigt
	public void ShowToolTip (GameObject slot){
		
		ShopSlot tmpSlot = slot.GetComponent<ShopSlot>();

		if (slot.GetComponentInParent<Shop>().isOpen == true && !tmpSlot.IsEmpty) {
			visualText.text = tmpSlot.CurrentItem.GetTooltip();
			sizeText.text = visualText.text;
			
			tooltip.SetActive(true);
			
			float xPos = slot.transform.position.x + slotPaddingLeft;
			float yPos = slot.transform.position.y - slot.GetComponent<RectTransform>().sizeDelta.y - slotPaddingTop;
			
			tooltip.transform.position = new Vector2(xPos, yPos);
			
		}	
	}
	
	// Funktion die Tooltip versteckt
	public void HideToolTip (){
		tooltip.SetActive (false);
	}
	
	
	// Funktion zum Layout generieren
	private void CreateLayout(){
		// Instanzieren von allSlots
		allSlots = new List<GameObject> ();
		
		// Sehr kleiner offset für hoverOffset, basierend auf der slotSize
		// Damit das Icon beim Platzieren mit der Maus nicht im weg der Maus ist, wenn man Klicken will

		emptySlot = slots;
		
		// Berechnen der Inventar Breite
		inventoryWidth = (slots / rows) * (slotSize + slotPaddingLeft) + slotPaddingLeft;
		
		// Berechnen der inventar Hoehe
		inventoryHight = rows * (slotSize + slotPaddingTop) + slotPaddingTop;
		
		// inventoryRect wird auf die RectTransform Komponente des Objektes (im Inspektor zu sehen) verwiesen
		inventoryRect = GetComponent<RectTransform> ();
		
		// Setzen der groesse fuer Horizontal und Vertikal mit den Ankern
		inventoryRect.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, inventoryWidth);
		inventoryRect.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, inventoryHight);
		
		
		// Calculate number of Columns
		int columns = slots / rows;
		
		// Verschachtelte schleife, die für jede Zeile (y) eine bestimmte Anzahl an Spalten(x) zeichnet
		// An die jweiligen Orte soll das Slot Prefab gesetzt werden. 
		for (int y = 0; y < rows; y++) {
			for (int x = 0; x < columns; x++){
				
				//Instantiates the slot and creates a reference to it
				GameObject newSlot = (GameObject)Instantiate(slotPrefab);
				
				//Makes a reference to the rect transform
				RectTransform slotRect = newSlot.GetComponent<RectTransform>();
				
				//Sets the slots name
				newSlot.name = "ShopSlot";
				
				//Sets the canvas as the parent (mit this bekommt es den gleichen Parent wie "this" ( Das Shopscript) of the slots, so that it will be visible on the screen
				newSlot.transform.SetParent(this.transform.parent);
				
				//Sets the slots position
				slotRect.localPosition = inventoryRect.localPosition + new Vector3(slotPaddingLeft * (x + 1) + (slotSize * x), -slotPaddingTop * (y + 1) - (slotSize * y));
				
				//Sets the size of the slot
				slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotSize * inventoryCanvas.scaleFactor);
				slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize * inventoryCanvas.scaleFactor);

				newSlot.transform.SetParent(this.transform);﻿

				//Adds the new slots to the slot list
				allSlots.Add(newSlot);
			}
		}
	}
	
	// Item hinzufiegen Funktion
	// WENN die maxSize vom item 1 ist, DANN Platziere das Item im Shop
	public bool AddItem(Item item){
		if (item.maxSize == 1) {
			PlaceEmpty (item);
			return(true);
		} else {
			// ANSONSTEN gehe jeden Slot der Liste durch und ueberpruefe ihn
			foreach (GameObject slot in allSlots){
				ShopSlot tmp = slot.GetComponent<ShopSlot>(); // Referenz auf das ShopSlotscript
				// WENN der Slot nicht leer ist dann ueberpruefe: 
				if (!tmp.IsEmpty){
					// OB das Item in dem Slot vom gleichen Item ist, wie das, was hinzugefuegt werden soll
					// UND ob der Slot zum Stacken verfuegbar ist
					// DANN fuege das Item hinzu
					if (tmp.CurrentItem.type == item.type && tmp.IsAvailable){
						tmp.AddItem(item);
						return true;
					}
				}
			}
			// WENN das Item nicht gestackt werden konnte
			// DANN soll das item einfach platziert werden, sofern es mindestens einen leeren Slot gibt
			if (emptySlot > 0){
				PlaceEmpty(item);
			}
		}
		return false;
	}
	
	// FUnktion: Places an item on an empty slot
	private bool PlaceEmpty(Item item){
		
		// If we have atleast 1 empty slot
		if (emptySlot > 0) {
			foreach(GameObject slot in allSlots){		//Runs through all slots
				ShopSlot tmp = slot.GetComponent<ShopSlot>();	//Creates a reference to the slot
				
				if (tmp.IsEmpty){						//If the slot is empty
					tmp.AddItem(item);					//Adds the item
					emptySlot--;						//Reduces the number of empty slots
					return true;
				}
			}
		}
		return false; // Ansonsten false
	}
	
		
	// Die Coroutine zum ausfaden
	private IEnumerator FadeOut(){
		// WENN nicht ausgefadet wird
		// DANN setze fadingOut auf true und fadingIn auf false und stoppe fadingIn. Mit anderen Worten: Starte den fadingOut Modus
		if (!fadingOut) {
			fadingOut = true;
			fadingIn = false;
			StopCoroutine("FadeIn");
			
			// Der startAlpha Wert ist der Wert, den das Alpha der CanvasGroup momentan hat
			float startAlpha = canvasGroup.alpha;
			
			// Die Geschwindigkeit ist 1 geteilt durch die fadeTime (1 ist die maximale Alpha stufe, die dann auf die Zeit aufgeteilt wird und so die Zeitschritte erzeugt)
			float rate = 1.0f / fadeTime;
			
			// Der Fortschritt des Fadens
			float progress = 0.0f;
			
			// SOLANGE der Fortschritt nicht 1 ist (also der Fade-Prozess nicht abgeschlossen ist
			while(progress < 1.0){
				// Ausfaden vom startAlpha zum Ziel (0) ueber die "Zeit" des Fortschritts
				canvasGroup.alpha = Mathf.Lerp(startAlpha,0,progress);
				// Fortschritt gleich rate mal Zeit (erhoehe den Fortschritt)
				progress += rate * Time.deltaTime;
				
				yield return null;
			}
			
			// Setze den Alpha der canvasGroup defintiv auf 0
			canvasGroup.alpha = 0;
			// Setze fadingOut auf false, da der Prozess nun beendet ist
			fadingOut = false;
		}
	}
	
	// Die Coroutine zum einfaden (genauso wie FadeOut nur gespiegelt)
	private IEnumerator FadeIn(){
		// WENN nicht eingefadet wird
		// DANN setze fadingIn auf true und fadingOut auf false und stoppe fadingOut. Mit anderen Worten: Starte den fadingIn Modus
		if (!fadingIn) {
			fadingOut = false;
			fadingIn = true;
			StopCoroutine("FadeOut");
			
			float startAlpha = canvasGroup.alpha;
			
			float rate = 1.0f / fadeTime;
			
			float progress = 0.0f;
			
			while(progress < 1.0){
				// Einfaden vom startAlpha zum Ziel (1) ueber die "Zeit" des Fortschritts
				canvasGroup.alpha = Mathf.Lerp(startAlpha,1,progress);
				
				progress += rate * Time.deltaTime;
				
				yield return null;
			}
			// Setze den Alpha der canvasGroup defintiv auf 0
			canvasGroup.alpha = 1;
			// Setze fadingIn auf false, da der Prozess nun beendet ist
			fadingIn = false;
		}
	}
	
	// Inventar mit Button oeffnen
	public void OpenInventoryButton(){
        // NUR wenn das PauseMenü nicht offen ist!
        // WENN der Alpha groesser als 0 ist, ist das Inventar geoeffnet und die Coroutine zum FadeOut wird gestartet
        // Außerdem wird das PutItemBack gestartet, dass dafuer sorgt, dass ein aufgenommenes Item zurueckgesetzt wird
        // ANSONSTEN ist das Inventar geschlossen und die Coroutine zum FadeIn wird gestartet
        if (PauseMenu.Instance.pauseMenuStatus == false) {
            if (canvasGroup.alpha > 0) {

                StartCoroutine("FadeOut");
                isOpen = false;
                HideToolTip();


            }
            else {
                StartCoroutine("FadeIn");
                isOpen = true;
            }

        }
    }

        
}
