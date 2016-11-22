// Wird der CraftingBench angehängt

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CraftingBench : Inventory {

	// CraftingBench Singleton
	private static CraftingBench instance;
	public static CraftingBench Instance{
		get { 
			if (instance == null){
				instance = FindObjectOfType<CraftingBench>();
			}
			return CraftingBench.instance;
		}
	}

	// CraftingButton Prefab
	public GameObject prefabButton;

	// Anzeigeslot
	private GameObject previewSlot;

	// Dictionary mit Blueprints
	private Dictionary<string, GameObject> craftingItems = new Dictionary<string, GameObject>();

	// Item Refernezen
	public GameObject mana;
	public GameObject beer;


	// Layout des Craftingmenüs
	// Überschreibt Layout von Inventory und nutzt es als Basis
	public override void CreateLayout(){
		base.CreateLayout ();

		GameObject craftBtn;

		inventoryRect.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, inventoryHight + slotSize + slotPaddingTop * 2); 

		craftBtn = Instantiate (prefabButton);

		RectTransform btnRect = craftBtn.GetComponent<RectTransform> ();

		craftBtn.name = "CraftButton";

		craftBtn.transform.SetParent (this.transform.parent);

		btnRect.localPosition = inventoryRect.localPosition + new Vector3 (slotPaddingLeft, - slotPaddingTop * 4 - (slotSize * 1));

		btnRect.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, ((slotSize * 2) + (slotPaddingLeft * 2)) * InventoryManager.Instance.inventoryCanvas.scaleFactor);
		btnRect.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, (slotSize * InventoryManager.Instance.inventoryCanvas.scaleFactor));

		craftBtn.transform.SetParent(transform);

		craftBtn.GetComponent<Button>().onClick.AddListener(CraftItem);


		previewSlot = Instantiate (InventoryManager.Instance.slotPrefab);

		RectTransform slotRect = previewSlot.GetComponent<RectTransform> ();

		previewSlot.name = "PreviewSlot";

		previewSlot.transform.SetParent (this.transform.parent);

		slotRect.localPosition = inventoryRect.localPosition + new Vector3((slotPaddingLeft * 3) + (slotSize * 2), -slotPaddingTop * 4 - (slotSize * 1));

		slotRect.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, (slotSize * InventoryManager.Instance.inventoryCanvas.scaleFactor));
		slotRect.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, (slotSize * InventoryManager.Instance.inventoryCanvas.scaleFactor));

		previewSlot.transform.SetParent (this.transform);

		previewSlot.GetComponent<Slot> ().ClickAble = false;

		                               
	}

	// Blueprints die in das Dictionary geladen werden
	public void CreateBlueprints(){

		// Code
		craftingItems.Add ("EMPTY-Health Potion-EMPTY-", mana);
		craftingItems.Add ("Wasser-Gerste-EMPTY-", beer);

	}

	// Funktion um das Item zu Craften, wenn der Craftbutton gedrückt wird
	public void CraftItem(){

		// OutputString, der den Code beinhalten wird
		string output = string.Empty;

		// Gehe jeden Slot des Crafting menüs durch und gucke ob er leer ist oder ob ein Item dirnn ist
		foreach (GameObject slot in allSlots) {

			// Temporäer Slot
			Slot tmp = slot.GetComponent<Slot>();

			// WENN Slot leer ist, dann schreibe EMPTY- in den Output String
			// ANSONSTEN den Itemnamen
			if (tmp.IsEmpty){
				output += "EMPTY-";

			} else{
				output += tmp.CurrentItem.itemName + "-";
			}
		}

		// WENN der String im Dictionary ist
		if (craftingItems.ContainsKey(output)) {
			//GameObject tmpObj = Instantiate(mana); // WENN Clones enstehen, koennen diese mit Destroy zerstoert werden

			// CraftedItem
			GameObject craftedItem;
			// tmpItem
			GameObject tmpItem;

			// Das Item wird aus dem Dictionary "herausgeholt" und in tmpItem gespeichert
			craftingItems.TryGetValue(output, out tmpItem);

			// WENN es das tmpItem gibt (also wenn es aus dem Dictionary geholt werden konnte)
			if (tmpItem != null)
			{
				// craftedItem wird zur Instanz (die dann in der Hierarchy auftaucht) von tmpItem
				craftedItem = tmpItem;


				// WENN das Item dem Inventar hinzugefügt werden konnte
				// DANN cleare das Craftingmenü immer um 1 Item auf jedem Slot (bei jedem das oberste)
				if (Inventory.Instance.AddItem(craftedItem.GetComponent<Item>())){
					//UpdatePreview ();
					foreach (GameObject slot in allSlots){
						slot.GetComponent<Slot>().RemoveItem();

					}
				}
			}
		}
		// Update des Previewslots
		UpdatePreview ();
	}
	//
	public override void MoveItem(GameObject clicked){
		base.MoveItem (clicked);

		UpdatePreview ();

	}


	// Update Previews
	public void UpdatePreview(){
		
		string output = string.Empty;

		previewSlot.GetComponent<Slot> ().ClearSlot ();
		
		foreach (GameObject slot in allSlots) {
			
			Slot tmp = slot.GetComponent<Slot>();
			
			if (tmp.IsEmpty){
				output += "EMPTY-";
				
			} else{
				output += tmp.CurrentItem.itemName + "-";
			}
		}

		// WENN das Item im Dictionary ist, dann soll es dem Previewslot hinzugefügt werden
		if (craftingItems.ContainsKey(output)) {
			//GameObject tmpObj = Instantiate(mana); // WENN Clones enstehen, koennen diese mit Destroy zerstoert werden
			
			GameObject craftedItem;
			
			GameObject tmpItem;
			
			craftingItems.TryGetValue(output, out tmpItem);

			if (tmpItem != null)
			{
				craftedItem = tmpItem;

				previewSlot.GetComponent<Slot>().AddItem(craftedItem.GetComponent<Item>());
				
			}
		}
	}
}
