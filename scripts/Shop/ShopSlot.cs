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
public class ShopSlot : MonoBehaviour, IPointerClickHandler {
	
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
		
	}
	
	
	/// Adds a single item to th inventory
	public void AddItem(Item item){ //The item to add (item)
		
		//Adds the item to the stack
		items.Push (item);
		
		//Checks if we have a stacked item
		//IF the item is stacked AND it is not infinite then we need to write the stack number on top of the icon
		if (items.Count >= 1 && CurrentItem.infiniteItems == false) {
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
	
	// FUnktion nimmt Item, dass den Slot besetzt und "benutzt" es
	private void UseItem(){
		
		// WENN der Slot nicht leer ist
		// DANN wird das Item benutzt
		if (!IsEmpty) {
			// WENN das Item unendlich verfiegbar ist
			// DANN benutze das Item (mit peek, statt pop, damit es nicht verbraucht wird
			// ANSONSTEN WENN das Item nicht unendlich verfügbar ist
			// DANN benutze und verbrauche das Item, sofern es hinzugefügt werden konnte (Use() == true ist)
			if (CurrentItem.infiniteItems == true){
				items.Peek().Use();
			} else if (CurrentItem.infiniteItems == false){

				if(items.Peek().Use() == true){
					items.Pop();
					// Verwalten der Stacknummern
					// Das ist eine Art einzeiliges if-Statement
					// Wenn die Anzahl der Items groesser als 1 ist, dann wird diese Zahl zum String für den stackTxt
					// Ansonsten ist der String leer, damit nicht 1 oder 0 als Anzahl angezeigt werden 
					// (Zahl soll nur angezeigt werden wenn man mindestens 2 Items hat)
					stackTxt.text = items.Count >= 1 ? items.Count.ToString() : string.Empty;
					
					// WENN der Slot durch das benutzen leer wird
					// DANN soll das Sprite wieder zu leer gewechselt werden und erhoehe die Anzahl der leeren Slots
					if (IsEmpty){
						ChangeSprite(slotEmpty, slotHighlight);
						Shop.EmptySlot++;
					}
				}
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
		
	}
	
	// Funktion die dazu fuehrt, die Clicks verwaltet
	public void OnPointerClick(PointerEventData eventData){
		// WENN die eventData (also die Eingabe) ein Linksklick war UND das Inventar nicht geschlossen ist (also alpha groesser als 0 ist
		// DANN benutze Item
		if (eventData.button == PointerEventData.InputButton.Left && Shop.CanvasGroup.alpha > 0) {
			UseItem();
			
		}
		
	}
}
