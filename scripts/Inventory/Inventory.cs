// https://www.youtube.com/watch?v=KLaGkc87dDQ
// Dieses Script verwaltet das Inventar
// https://www.youtube.com/watch?v=AzkRpc1SMJc

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour {

	// Inventorygroese (vom Layout her)
	protected RectTransform inventoryRect;

	// Breite und Hoehe des Inventars
	protected float inventoryWidth, inventoryHight;

	//Anzahl der Slots
	public int slots;

	// Anzahl der Reihen
	public int rows;

	// Anzahl der Abstände zwischen der Slots, links und oben zu einander
	public float slotPaddingLeft, slotPaddingTop;

	// Anzahl der groesse der Slots (vom Layout her)
	public float slotSize;
	


	// Liste
	//private static List<GameObject> allSlots;


	protected  List<GameObject> allSlots;

	public List<GameObject> AllSlots{
		get { return allSlots;}
	}


	// Offset, damit das hoverIcon nicht im weg ist, dann man das Item im Inventar mit einem Klick platzieren will
	private float hoverYOffset;

	// Prüft in Verbindung mit den dazugehörigen Funktionen und dem Triggerevent, ob die Maus auf dem Inventar ist oder nicht
	public static bool mouseInside = false;

	// Schalter, ob Inventar geöffnet ist oder nicht
	// Mit dazugehöriger Propertie, auf die von Außen zugegriffen werden kann
	private bool isOpen;
	public bool IsOpen
	{
		get { return isOpen; }
		set { isOpen = value; }
	}

	/// <summary>
	/// The inventory's canvas group, this is used for hiding the inventory
	/// </summary>
	public CanvasGroup canvasGroup;

	// Singelton
	// Gibt allen anderen Klassen Zugriff auf die Inventory Klasse
	private static Inventory instance;
	public static Inventory Instance{
		get { 
			if (instance == null){				// Wenn Instanz nicht gesetzt ist, dann setze sie
				instance = GameObject.FindObjectOfType<Inventory>();
			}
			return Inventory.instance;
		}
	}



	// UI Elemente zum Stack splitten
	private static GameObject selectStackSizeStatic;

	// Shop Variable, um in Update Funktion zu überprüfen, ob Maus über Shop ist
	public Shop shop;


	// Canvas Fade Variable
	private bool fadingIn;
	private bool fadingOut;
	public float fadeTime;

	// Anzahl der leeren Slots
	// Statisch, damit es nur eine Instanz davon gibt
	private int emptySlot;

	// Eigenschaft der leeren Slots, die public ist, damit andere Klassen darauf zugreifen können
	// Statisch, damit es nur eine Instanz davon gibt
	public int EmptySlot{
		get { return emptySlot;}
		set { emptySlot = value;}

	}



	// Use this for initialization
	void Start () {

		// Inventar ist am Anfang geschlossen
		isOpen = false;



		// Wenn Inventar "gestartet" wird, soll das Layout erstellt werden
		CreateLayout ();

		// Referenz zum MovingSlot
		InventoryManager.Instance.MovingSlot = GameObject.Find ("MovingSlot").GetComponent<Slot> ();
	}
	
	// Update is called once per frame
	void Update () {

		// Diese Funktion LOESCHT Items aus dem Inventar, wenn sie aufgenommen werden und man ausserhalb des Inventars klickt
		// WENN der Linke Mousbutton released wird
		if (Input.GetMouseButtonUp (0)) {
			// DANN WENN der Pointer nicht im Inventar ist UND from nicht null ist, also etwas aufgenommen wurde)
			// DANN cleare den Slot, Zerstoere das Hover Icon und resette to, from und fuege einen neuen leeren Slot hinzu
			// ANSONSTEN WENN man außerhalb klickt UND der moving Slot nicht leer ist (also man gerade splittet
			// DANN resette den slot und cleare HoverIcon (Außer WENN man auf den Shop klick. DANN verkaufe das Item)

			if(!mouseInside && InventoryManager.Instance.From != null && shop.mouseInsideShop == true){ // Verkaufen
				
				Debug.Log (InventoryManager.Instance.From.GetComponent<Slot>().CurrentItem.price);
				Debug.Log (InventoryManager.Instance.From.Items.Count);
				InventoryManager.Instance.From.GetComponent<Image>().color = Color.white;
				InventoryManager.Instance.From.ClearSlot();
				Destroy(GameObject.Find("Hover"));
				InventoryManager.Instance.To = null;
				InventoryManager.Instance.From = null;
				

			} else if (!mouseInside && InventoryManager.Instance.From != null){ // Zerstören
				Debug.Log ("UpdateIf zweite");
				InventoryManager.Instance.From.GetComponent<Image>().color = Color.white;
				InventoryManager.Instance.From.ClearSlot();
				Destroy(GameObject.Find("Hover"));
				InventoryManager.Instance.To = null;
				InventoryManager.Instance.From = null;


			} else if (!mouseInside && !InventoryManager.Instance.MovingSlot.IsEmpty){ // Verkaufen/Zerstören beim Splitten
				if (shop.mouseInsideShop == true){
					Debug.Log ("Cleare Slot in Update ueber Shop");
					Debug.Log (InventoryManager.Instance.MovingSlot.CurrentItem.guestPrice);
					Debug.Log (InventoryManager.Instance.MovingSlot.Items.Count);
				}

				InventoryManager.Instance.MovingSlot.ClearSlot();
				Destroy(GameObject.Find("Hover"));

				Debug.Log ("Cleare Slot in Update");
			}

		}

		// WENN das hoverObject nicht null ist
		// DANN soll es der Maus folgen
		if (InventoryManager.Instance.HoverObject != null) {
			// Positions Vektor mit x und y Achse anlegen
			Vector2 position;
			// Dieser Codeabschnitt sorgt dafür, dass das hoverObjekt der Maus folgt
			RectTransformUtility.ScreenPointToLocalPointInRectangle(InventoryManager.Instance.inventoryCanvas.transform as RectTransform, Input.mousePosition, InventoryManager.Instance.inventoryCanvas.worldCamera, out position);
			position.Set(position.x,position.y - hoverYOffset); // Offset, damit das Icon nicht im WEg der Maus ist
			InventoryManager.Instance.HoverObject.transform.position = InventoryManager.Instance.inventoryCanvas.transform.TransformPoint(position);
		}

		if (Input.GetKeyDown(KeyCode.I))//Checks if we press the I button
		{
			PlayerPrefs.DeleteAll();
		}
	
	}


	// Funktionen die überprüfen, ob Mauszeiger auf dem inventar ist oder nicht
	public void PointerExit(){
		mouseInside = false;
	}
	
	public void PointerEnter(){
		if (canvasGroup.alpha > 0){
			mouseInside = true;
		}
	}

	// Funktion die das Inventar öffnet und schließt und den isOpen Status setzt
	public void Open()
	{
		if (canvasGroup.alpha > 0) //If our inventory is visible, then we know that it is open
		{
			StartCoroutine("FadeOut"); //Close the inventory
			PutItemBack(); //Put all items we have in our hand back in the inventory
			HideToolTip();
            canvasGroup.blocksRaycasts = false;
			isOpen = false;
			Debug.Log(isOpen);
		}
		else if (canvasGroup.alpha <= 0 && PauseMenu.Instance.pauseMenuStatus == false)//If it isn't open then it's closed and we neeed to fade in. ONLY if the pauseMenu ist not Open!
		{
			StartCoroutine("FadeIn");
            canvasGroup.blocksRaycasts = true;
            isOpen = true;
			Debug.Log(isOpen);
		}
	}

	// Funktion die den Tooltip fuer den aktuellen Slot zeigt
	public void ShowToolTip (GameObject slot){

		// Temporaerer Slot, der aus dem aktuellen Slot besteht
		Slot tmpSlot = slot.GetComponent<Slot>();

		// WENN der Slot NICHT leer ist, kein hoverObject existiert (wie also nichts aufgenommen haben) UND wir nichts splitten wollen
		// DANN zeige tooltip
		if (slot.GetComponentInParent<Inventory>().isOpen && !tmpSlot.IsEmpty && InventoryManager.Instance.HoverObject == null && !InventoryManager.Instance.selectStackSize.activeSelf) {

			// Speichere im sichtbaren Text den Tooltip des Items des Slots
			InventoryManager.Instance.visualTextObject.text = tmpSlot.CurrentItem.GetTooltip();
			// Passe die groesse des Tooltips an
			InventoryManager.Instance.sizeTextObject.text = InventoryManager.Instance.visualTextObject.text;

			// Aktiviere den tooltip
			InventoryManager.Instance.tooltipObject.SetActive(true);

			// Position des Tooltips (immer unter dem Slot)
			float xPos = slot.transform.position.x + slotPaddingLeft;
			float yPos = slot.transform.position.y - slot.GetComponent<RectTransform>().sizeDelta.y - slotPaddingTop;
			InventoryManager.Instance.tooltipObject.transform.position = new Vector2(xPos, yPos);

		}


	}

	// Funktion um den Tooltip zu schliessen
	public void HideToolTip (){
		InventoryManager.Instance.tooltipObject.SetActive (false);
	}



	// Funktion zum Layout generieren - Virtual damit CraftingBench diese Funktion ueberschreiben kann (da es vom Inventory erbt)
	public virtual void CreateLayout(){
		// Instanzieren von allSlots
		allSlots = new List<GameObject> ();

		// Sehr kleiner offset für hoverOffset, basierend auf der slotSize
		// Damit das Icon beim Platzieren mit der Maus nicht im weg der Maus ist, wenn man Klicken will
		hoverYOffset = slotSize * 0.01f;

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
				GameObject newSlot = (GameObject)Instantiate(InventoryManager.Instance.slotPrefab);

				//Makes a reference to the rect transform
				RectTransform slotRect = newSlot.GetComponent<RectTransform>();

				//Sets the slots name
				newSlot.name = "Slot";

				//Sets the canvas as the parent (mit this bekommt es den gleichen Parent wie "this" ( Das Inventarscript) of the slots, so that it will be visible on the screen
				newSlot.transform.SetParent(this.transform.parent);

				//Sets the slots position
				slotRect.localPosition = inventoryRect.localPosition + new Vector3(slotPaddingLeft * (x + 1) + (slotSize * x), -slotPaddingTop * (y + 1) - (slotSize * y));

				//Sets the size of the slot
				slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotSize * InventoryManager.Instance.inventoryCanvas.scaleFactor);
				slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize * InventoryManager.Instance.inventoryCanvas.scaleFactor);

				// Damit die Slots nicht hinter dem Hintergrund verschwinden
				newSlot.transform.SetParent(this.transform);﻿

				//Adds the new slots to the slot list
				allSlots.Add(newSlot);

				// Delegate zum MovingSlot 
				newSlot.GetComponent<Button>().onClick.AddListener(
					delegate{MoveItem(newSlot);}
				);
			}
		}
	}

	// Item hinzufiegen Funktion
	// WENN die maxSize vom item 1 ist, DANN Platziere das Item im Inventar
	public bool AddItem(Item item){
		Debug.Log ("AddItem wird ausgeführt");
		if (item.maxSize == 1) {
			//Places the item at an empty slot
			return PlaceEmpty(item);
		} else {
			// ANSONSTEN gehe jeden Slot der Liste durch und ueberpruefe ihn
			foreach (GameObject slot in allSlots){
				Slot tmp = slot.GetComponent<Slot>(); // Referenz auf das Slotscript
				// WENN der Slot nicht leer ist dann ueberpruefe: 
				if (!tmp.IsEmpty){
					// OB das Item in dem Slot vom gleichen Item ist, wie das, was hinzugefuegt werden soll
					// UND ob der Slot zum Stacken verfuegbar ist
					// DANN fuege das Item hinzu
					if (tmp.CurrentItem.type == item.type && tmp.IsAvailable){

						// WENN wir ein Item splitten (also die "Hand" nicht leer ist und clicked == dem temporaeren item ist)
						// DANN mach weiter
						// ANSONSTEN fuege das Item erneut hinzu
						// Das ist notwendig, damit, wenn man neue Items hinzfuegt waehrend man gerade eins auf der Hand hat, der Stack von dem man das Item hat, nicht groesser werden kann
						if (!InventoryManager.Instance.MovingSlot.IsEmpty && InventoryManager.Instance.Clicked.GetComponent<Slot>() == tmp.GetComponent<Slot>()){
							continue;
						} else {
							tmp.AddItem(item);
							Debug.Log ("Item wurde gestacked");
							return true;
						}
					}
				}
			}
			// WENN das Item nicht gestackt werden konnte
			// DANN soll das item einfach platziert werden, sofern es mindestens einen leeren Slot gibt
			if (emptySlot > 0){
				return PlaceEmpty(item);
			}
		}
		return (false);
	}

	// FUnktion: Places an item on an empty slot
	private bool PlaceEmpty(Item item){
		if (emptySlot > 0) //If we have atleast 1 empty slot
		{
			foreach (GameObject slot in allSlots) //Runs through all slots
			{
				Slot tmp = slot.GetComponent<Slot>(); //Creates a reference to the slot 
				
				if (tmp.IsEmpty) //If the slot is empty
				{
					tmp.AddItem(item); //Adds the item
					
					return true;
				}
			}
		}
		return false; // Ansonsten false
	}

	// Funktion zum Bewegen von Items
	// Clicked ist der Mausklick
	public virtual void MoveItem(GameObject clicked){
		// Nur wenn Inventar offen ist
		if (isOpen) {

			// Referenz zur Canvascroup des geclickten Items
			CanvasGroup cg = clicked.transform.parent.GetComponent<CanvasGroup>();

			// WENN cg da ist ODER die CanvasGroup angezeigt wird
			if (cg != null && cg.alpha > 0 || clicked.transform.parent.GetComponent<CanvasGroup> ().alpha > 0) {
				
				// Refernz zu clicked
				InventoryManager.Instance.Clicked = clicked;
												
				// WENN der movingSlot nicht leer ist
				if (!InventoryManager.Instance.MovingSlot.IsEmpty) {
					// Temporaerer slot der aus dem Slot besteht, den man angekickt hat
					Slot tmp = clicked.GetComponent<Slot> ();
					
					// WENN der Slot leer ist, auf den wir unsere gesplitteten Items ablegen wollen
					// DANN fuege die Items aus dem movingSlot hinzu, cleare den movingSlot und zerstoere HoverIcon
					// ANSONSTEN WENN es nicht leer ist, der Typ dem entsprcht was wir bewegen wollen UND man noch stacken kann
					// DANN merge die Stacks
					if (tmp.IsEmpty) {
						tmp.AddItems (InventoryManager.Instance.MovingSlot.Items);
						InventoryManager.Instance.MovingSlot.Items.Clear ();
						Destroy (GameObject.Find ("Hover"));
						Debug.Log ("Cleare Slot in MovingSlot 1. If Abfrage");
					} else if (!tmp.IsEmpty && InventoryManager.Instance.MovingSlot.CurrentItem.type == tmp.CurrentItem.type && tmp.IsAvailable) {
						MergeStacks (InventoryManager.Instance.MovingSlot, tmp);
						Debug.Log ("Merge Slot in MovingSlot 1. If Abfrage");
					}
				}
				// WENN der Slot von dem etwas bewegt werden soll null ist, heisst das, dass es das erste Item ist, das angeklickt wurde UND das Inventar an ist (also alpha = 1) UND wir nicht shift klicken, also nicht splitten
				// Das bedeutet es ist das Item, das wir bewegen wollen
				else if (InventoryManager.Instance.From == null && clicked.transform.parent.GetComponent<Inventory> ().isOpen && !Input.GetKey (KeyCode.LeftShift)) {
					Debug.Log ("MovingSlot zweite If-Abfrage");
					// WENN der Slot, der angeklickt wurde NICHT leer ist, dann ist dieser Slot von einem Item besetzt
					// DANN kann das Item bewegt werden und daher wird das geklickte Item (bzw. der geklickte Slot) in "from" gespeichert
					// MACHE ausserdem das Bild des geklickten Items grau
					if (!clicked.GetComponent<Slot> ().IsEmpty && !GameObject.Find ("Hover")) {
						InventoryManager.Instance.From = clicked.GetComponent<Slot> ();
						InventoryManager.Instance.From.GetComponent<Image> ().color = Color.gray;
						
						//Erschaffe HoverIcon
						CreateHoverIcon ();
					}
				} // ANSONSTEN WENN from nicht null ist, wird ueberprueft ob to null ist UND Shift nicht gedrueckt wird
				// Also haben wir bereits ein Objekt angewahlt bzw. aufgenommen
				// DANN wird das Objekt in "to" gespeichert. Das Item wird also platziert
				// Das HoverObjekt wird dabei zerstoert.
				else if (InventoryManager.Instance.To == null && !Input.GetKey (KeyCode.LeftShift)) {
					Debug.Log ("MovingSlot vorletzte If-Abfrage");
					InventoryManager.Instance.To = clicked.GetComponent<Slot> ();
					Destroy (GameObject.Find ("Hover"));
				}
				// WENN gar nichts von null ist, werden die Items getauscht bzw gemerged
				if (InventoryManager.Instance.To != null && InventoryManager.Instance.From != null) {
					Debug.Log ("MovingSlot letzte If-Abfrage");
					if (!InventoryManager.Instance.To.IsEmpty && InventoryManager.Instance.From.CurrentItem.type == InventoryManager.Instance.To.CurrentItem.type && InventoryManager.Instance.To.IsAvailable) {
						MergeStacks (InventoryManager.Instance.From, InventoryManager.Instance.To);
					} else {
						Slot.SwapItems(InventoryManager.Instance.From, InventoryManager.Instance.To);
						/*
						// Das Item bzw die Items die in to sind werden in einem temporaeren Stack gespeichert
						Stack<Item> tmpTo = new Stack<Item> (InventoryManager.Instance.To.Items);
						// Das to Item wird durch das from Item ersetzt
						InventoryManager.Instance.To.AddItems (InventoryManager.Instance.From.Items);
						
						// WENN to 0 ist, also das Item an einen leeren Slot soll
						// DANN cleare das from Item (es wird nichts getauscht, nur verschoben)
						// ANSONSTEN wenn Slot zu dem wir hinwollen nicht leer ist, dann ersetze den from slot durch den  toslo
						// also tausche die beiden aus
						if (tmpTo.Count == 0) {
							InventoryManager.Instance.From.ClearSlot ();
						} else {
							InventoryManager.Instance.From.AddItems (tmpTo);
						} */
					}
					// Das Ausgrauen wird resettet
					InventoryManager.Instance.From.GetComponent<Image>().color = Color.white;
					// To und from resetten und hoverObjekt zerstoeren
					InventoryManager.Instance.To = null;
					InventoryManager.Instance.From = null;
					Destroy (GameObject.Find ("Hover"));
				}

				// WENN die CraftingBench angezeigt wird, soll diese immer geupdatet werden, wenn ein item bewegt wird
				if (CraftingBench.Instance.isOpen) {
					CraftingBench.Instance.UpdatePreview ();
				}
			}
		}
	}

	//Funktion um Hover Icon zu erschaffen
	private void CreateHoverIcon(){
		Debug.Log ("Erschaffe HoverIcon");
		// ===hover Icon===
		// Instantieren des iconPrefabs und casten in ein GameObject
		InventoryManager.Instance.HoverObject = (GameObject)Instantiate(InventoryManager.Instance.iconPrefab);
		// Nimmt den Sprite von dme Slot der geklickt wurde und bringt ihm zum Hoverobjekt
		InventoryManager.Instance.HoverObject.GetComponent<Image>().sprite = InventoryManager.Instance.Clicked.GetComponent<Image>().sprite;
		// Objekt wird als "Hover" angezeigt im Editor
		InventoryManager.Instance.HoverObject.name = "Hover";
		
		// Speichern der Transforms
		RectTransform hoverTransform = InventoryManager.Instance.HoverObject.GetComponent<RectTransform>();
		RectTransform clickedTransform = InventoryManager.Instance.Clicked.GetComponent<RectTransform>();
		
		// hoverTransform soll die Horizontale und vertikale groesse von clickedTransform bekommen
		hoverTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, clickedTransform.sizeDelta.x);
		hoverTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, clickedTransform.sizeDelta.y);
		
		// hoverObjekt soll Child vom InventoryCanvas werden
		InventoryManager.Instance.HoverObject.transform.SetParent(GameObject.Find("InventoryCanvas").transform, true);
		// hoverObjekt bekommt die lokale Groesse zugewiesen
		InventoryManager.Instance.HoverObject.transform.localScale = InventoryManager.Instance.Clicked.gameObject.transform.localScale;

		// Textanzeige fuer das HoverIcon
		InventoryManager.Instance.HoverObject.transform.GetChild (0).GetComponent<Text> ().text = InventoryManager.Instance.MovingSlot.Items.Count > 1 ? InventoryManager.Instance.MovingSlot.Items.Count.ToString () : string.Empty;
	}

	// Funktion, die das Item zuruecksetzt wenn das Inventar geschlossen wird
	private void PutItemBack(){

		// WENN das aufgenomme Item nicht null ist (also wenn tatsaechlich ein item aufgenommen wurde)
		// DANN zerstoere das HoverIcon, setze die Farbe des Bildes von ausgegraut auf weiss und resette from
		// ANSONSTEN WENN der moving Slot NICHT leer ist (man also gerade splittet)
		// DANN Zerstoere Hover, gehe jedes Item auf der "Hand" durch und tue es ins Inventar zurureck und cleare danach den movingslot. Schliesse dann das Stacksplitpanel
		if (InventoryManager.Instance.From != null) {
			Destroy (GameObject.Find ("Hover"));
			InventoryManager.Instance.From.GetComponent<Image> ().color = Color.white;
			InventoryManager.Instance.From = null;
		} else if (!InventoryManager.Instance.MovingSlot.IsEmpty) {
			Destroy(GameObject.Find("Hover"));
			foreach (Item item in InventoryManager.Instance.MovingSlot.Items){
				InventoryManager.Instance.Clicked.GetComponent<Slot>().AddItem(item);
			}

			InventoryManager.Instance.MovingSlot.ClearSlot();
		}

		InventoryManager.Instance.selectStackSize.SetActive (false);
	}



	// Funktion die aufgerufen wird, wenn man "Ok" beim splitten drueckt
	// Sie splittet also die Items, so dass man sie woanders platzieren kann
	public void SplitStack(){
		// Wenn man ok drueckt wird das Stackpanel wieder deaktiviert
		InventoryManager.Instance.selectStackSize.SetActive (false);

		// WENN die Anzahl der Items die gesplittet werden sollen == der maximalen Stackgroesse ist
		// DANN splitte nicht, sondern bewege den gesamten Stack
		// ANSONSTEN wenn es groesser als 0 ist
		// DANN packe die Anzahl der Items die gesplittet werden sollen (aus RemoveItems()) in den movingSlot und erstelle ein HoverIcon
		if (InventoryManager.Instance.SplitAmount == InventoryManager.Instance.MaxStackCount) {
			MoveItem (InventoryManager.Instance.Clicked);
			Debug.Log ("SplitAmount == MaxStackCount");
		} else if (InventoryManager.Instance.SplitAmount > 0) {
			InventoryManager.Instance.MovingSlot.Items = InventoryManager.Instance.Clicked.GetComponent<Slot>().RemoveItems(InventoryManager.Instance.SplitAmount);
			Debug.Log ("InventoryManager.Instance.SplitAmount > 0");
			CreateHoverIcon();

		}
	}

	// Funktion die die Links und Rechts Pfeile des Stacksplitpanels bedient
	// i ist die Anzahl um die der Split erhoeht oder gesenkt wird
	public void ChangeStackText(int i){

		// Split amount wird immer um 1 erhoeht
		InventoryManager.Instance.SplitAmount += i;
		// WENN man weniger als 0 Items splitten will, dann wird splitAmount 0 (damit man nie unter 0 gehen kann)
		if (InventoryManager.Instance.SplitAmount < 0) {
			InventoryManager.Instance.SplitAmount = 0;
		}
		// WENN man mehr Items splitten will, als der Stack ueberhaupt hergeben kann, dann ist die Anzahl der Items die maximale Stackgroesse
		// So kann man nie mehr Items splitten als der Stack beinhaltet
		if (InventoryManager.Instance.SplitAmount > InventoryManager.Instance.MaxStackCount){
			InventoryManager.Instance.SplitAmount = InventoryManager.Instance.MaxStackCount;
		}

		// Der Stacktext im Stacksplitpanel der angezeigt wird
		InventoryManager.Instance.stackText.text = InventoryManager.Instance.SplitAmount.ToString();
	}

	// Funktion die Stack zusammenfuehrt, wenn man gesplittet hat
	// Sie nimmt die Slot quelle und den Zielslot
	public void MergeStacks(Slot source, Slot destination){
		// Variable, die angibt, wie viel Items zusammengefuhert werden koennen (wie viele man im Stack ablegen kann)
		// Sie ist die maximale Slotgroesse des aktuellen Items MINUS die Anzahl der Items, die momentan schon im Slot sind
		int max = destination.CurrentItem.maxSize - destination.Items.Count;

		// count berechnet wie viele Items aus der dem Quellslot entfernt werden koennen
		// WENN die Anzahl der Items kleiner als das Maximum des Zielslots ist
		// DANN ist count diese Anzahl
		// ANSONSTEN ist count maximum
		int count = source.Items.Count < max ? source.Items.Count : max;

		// Schleife die Items aus der Hand nacheinander im Zielstack ablegt (mit RemoveItem())
		for (int i = 0; i < count; i++){
			destination.AddItem(source.RemoveItem());
			// Textanzeige fuer das HoverIcon
			InventoryManager.Instance.HoverObject.transform.GetChild (0).GetComponent<Text> ().text = InventoryManager.Instance.MovingSlot.Items.Count.ToString();
		}
		// WENN In der "Hand" keine Items mehr sind, dann wird die Hand gecleared und das Hoverobjekt zerstoert
		if (source.Items.Count == 0){
			source.ClearSlot();
			Destroy(GameObject.Find("Hover"));
		}
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

	// KANN VIELLEICHT WEG?
	// Inventar mit Button oeffnen
	public virtual void OpenInventoryButton(){
		// WENN der Alpha groesser als 0 ist, ist das Inventar geoeffnet und die Coroutine zum FadeOut wird gestartet
		// Außerdem wird das PutItemBack gestartet, dass dafuer sorgt, dass ein aufgenommenes Item zurueckgesetzt wird
		// ANSONSTEN ist das Inventar geschlossen und die Coroutine zum FadeIn wird gestartet
		if(canvasGroup.alpha > 0){
			
			StartCoroutine("FadeOut");
			PutItemBack();
		} else {
			StartCoroutine("FadeIn");
		}

	}
	public void EmptySlotShow(){
		Debug.Log (EmptySlot);
	}
}



