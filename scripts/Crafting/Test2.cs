using UnityEngine;
using System.Collections;

public class Test2 : MonoBehaviour {

	public Inventory inventory;
	public GameObject other;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Bla (){
		inventory.AddItem(other.GetComponent<Item>());
	}
}
