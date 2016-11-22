// Dieses Script wird dem Image beim Haendler angehangen
// Es macht das Item kaufbar und fuegt es zum Inventar hinzu
// Dem Item muss eine Buttonkomponente angehaengt werden, die dann auf dieses Script hier verweist

using UnityEngine;
using System.Collections;

public class PickUpItem : MonoBehaviour {


	//Objektrefernz auf das Inventar
	public Inventory inventory;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AddOnClick(){

		inventory.AddItem(gameObject.GetComponent<Item>());

	}
}
