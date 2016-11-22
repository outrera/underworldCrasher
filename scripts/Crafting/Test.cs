/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Test : MonoBehaviour {

	public Stack<Item> Items;

	public Item healthPotion;

	private int itemCounter;



	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ItemsCount(){


		int x = 0;
		foreach (GameObject slot in Inventory.AllSlots){
			Slot tmp = slot.GetComponent<Slot>(); // Referenz auf das Slotscript

			if (!tmp.IsEmpty == true){
				if (healthPotion.type == tmp.CurrentItem.type){
					itemCounter += tmp.Items.Count;
					Debug.Log("TRUE");
				} else {
					Debug.Log ("FALSE");
				}
			} else {
				Debug.Log("False");
			}
		}

		Debug.Log (itemCounter);

	}

}
*/