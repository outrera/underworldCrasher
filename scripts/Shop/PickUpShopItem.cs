// Dieses Script wird dem ShopObjects Empty angehangen
// Es fuegt kaufbare Items zu beginn des Spiels in den Shop hinzu


using UnityEngine;
using System.Collections;

public class PickUpShopItem : MonoBehaviour {
	
	
	//Objektrefernz auf den Shop
	public Shop shop;
	
	// Use this for initialization
	void Start () {

		// Gegenstände werden mit halber Sekunde verspaetung hinzugefuegt, um sicherzustellen, dass das Inventar richtig initialisiert wurde
		Invoke("AddShopItem", 0.5f);

	}
	
	// Update is called once per frame
	void Update () {

	}

	// Fuegt Item dem Shop hinzu
	public void AddShopItem(){


		shop.AddItem(gameObject.GetComponent<Item>());
		
	}
}
