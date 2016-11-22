// Dieses Script verwaltet die Inventarslots
// Es wird dem Slot prefab angehangen

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Wir brauchen neben MonoBehaviour auch den IPointerClickHandler
// https://www.youtube.com/watch?v=KLaGkc87dDQ 1:41:00 circa
// Dieses brauchen wir, damit wir Buttons mit Rechtsklick bedienen koennen
public class Slot : MonoBehaviour, IPointerClickHandler {

	/// The items that the slot contains
	private Stack<Item> items;

	// Eigenschaft von items, damit sie für andere Klassen verfuegbar ist
	public Stack<Item> Items{
		get { return items;}
		set { items = value;}
	}

	/// Indicates the number of items stacked on the slot as Text
	public Text stackTxt;

	/// The slot's empty sprite
	public Sprite slotEmpty;
	/// The slot's highlighted sprite
	public Sprite slotHighlight;

	/// Indicates if the slot is empty
	public bool IsEmpty{
		get { return items.Count == 0;}
	}

	// Sagt, ob Slot verfuegbar für Stacking ist
	// WENN die maxSize des Items (im Inspector einstellen) groesser als items.Count (Anzahl der items, die momentan im Inventar sind) ist
	// DANN ist es fuer das Stacken verfuegbar
	public bool IsAvailable{

		get { return CurrentItem.maxSize > items.Count; }
	}

	// 
	// Funktion gibt Itemtyp zurueck, um zu ueberpruefen, ob es stacken kann
	public Item CurrentItem{

		get { return items.Peek(); }
	}

	// Ob Slot anklickbar ist (wegen Prewviewslot)
	private bool clickAble = true;
	public bool ClickAble{
		get { return clickAble; }
		set { clickAble = value;}

	}

	private CanvasGroup canvasGroup;

	// Use this for initialization
	void Start () {

		//Instantiates the items stack
		items = new Stack<Item> ();

		//Creates a reference to the slot slot's recttransform
		RectTransform slotRect = GetComponent<RectTransform> ();
		//Creates a reference to the stackTxt's recttransform
		RectTransform txtRect = stackTxt.GetComponent<RectTransform> ();

		//Calculates the scalefactor of the text by taking 60% of the slots width
		int txtScaleFactor = (int)(slotRect.sizeDelta.x * 0.60);

		//Sets the min and max textSize of the stackTxt
		stackTxt.resizeTextMaxSize = txtScaleFactor;
		stackTxt.resizeTextMinSize = txtScaleFactor;

		//Sets the actual size of the txtRect
		txtRect.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, slotRect.sizeDelta.x);
		txtRect.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, slotRect.sizeDelta.y);

		if (transform.parent != null)
		{
			canvasGroup = transform.parent.GetComponent<CanvasGroup>();
		}

	}


	/// Adds a single item to th inventory
	public void AddItem(Item item){ //The item to add (item)

		if (IsEmpty) //if the slot is empty
		{
			transform.parent.GetComponent<Inventory>().EmptySlot--; //Reduce the number of empty slots
		}

		//Adds the item to the stack
		items.Push (item);

		//Checks if we have a stacked item
		//IF the item is stacked then we need to write the stack number on top of the icon
		if (items.Count > 1) {
			stackTxt.text = items.Count.ToString(); 
		}

		//Changes the sprite so that it reflects the item the slot is occupied by
		ChangeSprite (item.spriteNeutral, item.spriteHighlighted);

	}

	// Diese Funktion ist dazu da, wenn man Stacks im Inventar verschieben will
	public void AddItems(Stack<Item> items){ // Eckige Klammern in diesem Fall bedeutet "stack". Also ein Stack vom Typ Item

		// Diese Zeile ersetzt das Item, was gerade im Slot ist mit einem neuen Stack von Items
		this.items = new Stack<Item> (items);

		// Verwalten der Stacknummern
		// Das ist eine Art einzeiliges if-Statement
		// Wenn die Anzahl der Items groesser als 1 ist, dann wird diese Zahl zum String für den stackTxt
		// Ansonsten ist der String leer, damit nicht 1 oder 0 als Anzahl angezeigt werden 
		// (Zahl soll nur angezeigt werden wenn man mindestens 2 Items hat)
		stackTxt.text = items.Count > 1 ? items.Count.ToString() : string.Empty;

		// Diese Funktion passt das Sprite an, wenn das aktuelle Item herumgeschoben wird
		ChangeSprite(CurrentItem.spriteNeutral, CurrentItem.spriteHighlighted);

	} 

	/// Changes the sprite of a slot
	private void ChangeSprite(Sprite neutral, Sprite highlight){

		//Sets the neutralsprite
		GetComponent<Image>().sprite = neutral;

		//Creates a spriteState, so that we can change the sprites of the different states
		SpriteState st = new SpriteState ();
		st.highlightedSprite = highlight;
		st.pressedSprite = neutral;

		//Sets the sprite state
		GetComponent<Button> ().spriteState = st;
	}

	// FUnktion nimmt Item, dass den Slot besetzt und "verkauft" es
	private void UseItem(){

		// WENN der Slot nicht leer ist
		// DANN wird das Item aus dem Stack genommen (Pop()) und verkauft
		if (!IsEmpty && clickAble) {
			items.Pop().Sell();

			// Verwalten der Stacknummern
			// Das ist eine Art einzeiliges if-Statement
			// Wenn die Anzahl der Items groesser als 1 ist, dann wird diese Zahl zum String für den stackTxt
			// Ansonsten ist der String leer, damit nicht 1 oder 0 als Anzahl angezeigt werden 
			// (Zahl soll nur angezeigt werden wenn man mindestens 2 Items hat)
			stackTxt.text = items.Count > 1 ? items.Count.ToString() : string.Empty;

			// WENN der Slot durch das benutzen leer wird
			// DANN soll das Sprite wieder zu leer gewechselt werden und erhoehe die Anzahl der leeren Slots
			if (IsEmpty){
				ChangeSprite(slotEmpty, slotHighlight);
				transform.parent.GetComponent<Inventory>().EmptySlot++;

			}
			
		}

	}

	// Resettet Slot, wenn wir ein Item davon verschieben (macht ihn wieder leer)
	public void ClearSlot(){
		// Items werden gecleared
		items.Clear ();
		// Sprite werden resettet
		ChangeSprite(slotEmpty, slotHighlight);
		// Text wird leer gemacht
		stackTxt.text = string.Empty;

		if (transform.parent != null)
		{
			transform.parent.GetComponent<Inventory>().EmptySlot++;
		}

	}

	// Funktion die erlaubt einen spezifischen Anteil der items aus dem Stack zu nehmen
	// Der Funktion wie die Anzahl, die man entfernen will uebergeben
	public Stack<Item> RemoveItems (int amount){
		// Temporaerer Stack
		Stack<Item> tmp = new Stack<Item> ();

		// Schleife die über die Anzahl iteriert und dabei jedes mal das "top" Item des Slots in den stack pushed
		for (int i = 0; i < amount; i++) {
			tmp.Push(items.Pop());
		}

		// Smart if-Statement welches den Text aktualisiert, damit die 1 nicht angezeigt wird, nur ab 2 Items soll Zahl angezeigt werden
		stackTxt.text = items.Count > 1 ? items.Count.ToString() : string.Empty;

		// Gibt den Splitstack (der temporaere Stack den man entfernt hat) zurueck
		return(tmp);

	}

	// FUnktion die sicherstellt, dass man beim platzieren der gesplittet Items nicht ueber das Stacklimit kommen kann
	// Sie nimmt dabei immer nur 1 Item nacheinander, damit in der inventory merge Funktion das Stack Limit abgefangen werden kann
	public Item RemoveItem(){

		// WENN Slot nicht leer ist
		if (!IsEmpty) {
			// Tempraeres Item
			Item tmp;
			
			// Nimmt das Item und packe es in tmp
			tmp = items.Pop ();
			
			// Smart if-Statement fuer den stackText
			stackTxt.text = items.Count > 1 ? items.Count.ToString() : string.Empty;

			// WENN Slot empty ist
			// DANN cleare ihn
			if (IsEmpty){
				ClearSlot();
			}

			// Gibt temporaeres Item wieder
			return(tmp);
		}
		return null;
	}


	// Funktion die dazu fuehrt, die Clicks verwaltet
	public void OnPointerClick(PointerEventData eventData){
		// WENN die eventData (also die Eingabe) ein Rechtsklick war UND kein HoverIcon zufinden war UND das Inventar nicht geschlossen ist (also alpha groesser als 0 ist
		// DANN verkaufe Item
		// ANSONSTEN WENN Linksklick und Shift UND der Slot NICHT leer ist und KEIN Hover Icon gefunden wurde, dann splitte
		if (eventData.button == PointerEventData.InputButton.Right && !GameObject.Find ("Hover") && canvasGroup.alpha > 0) {
			UseItem ();

		} else if (eventData.button == PointerEventData.InputButton.Left && Input.GetKey (KeyCode.LeftShift) && !IsEmpty && !GameObject.Find ("Hover") && transform.parent.GetComponent<Inventory>().IsOpen) {

			// Vektor der Mausposition, da das Splitfesnter immer bei der Mausposition spawnen soll
			Vector2 position;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(InventoryManager.Instance.inventoryCanvas.transform as RectTransform, Input.mousePosition, InventoryManager.Instance.inventoryCanvas.worldCamera, out position);

			// aktiviert Stack Split Panel
			InventoryManager.Instance.selectStackSize.SetActive(true);
			// Setzt Stackfesnter zur Mausposition
			InventoryManager.Instance.selectStackSize.transform.position = InventoryManager.Instance.inventoryCanvas.transform.TransformPoint(position);
			// Ruft Stackinfo ab, um zu wissen,was mit dem Stack moeglich ist bzw. wie viel items entfernt werden koennen
			InventoryManager.Instance.SetStackInfo(items.Count);
		}

	}

	public static void SwapItems(Slot from, Slot to)
	{
		
		if (to != null && from != null)
		{
			// Das Item bzw die Items die in to sind werden in einem temporaeren Stack gespeichert
			Stack<Item> tmpTo = new Stack<Item> (to.Items);  //Stores the items from the to slot, so that we can do a swap
				
			to.AddItems(from.Items); //Stores the items in the "from" slot in the "to" slot
				
			if (tmpTo.Count == 0) //If "to" slot if 0 then we dont need to move anything to the "from " slot.
			{
				to.transform.parent.GetComponent<Inventory>().EmptySlot--;
				from.ClearSlot(); //clears the from slot
			}
			else
			{
				from.AddItems(tmpTo); //If the "to" slot contains items thne we need to move the to the "from" slot
			}
		}
	}

		
		
		

}
