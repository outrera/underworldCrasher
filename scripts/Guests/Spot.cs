// Eigenschaften des freien Spots
// Wird dem Spot angehangen
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spot : MonoBehaviour {

	// Ist der Spot zum sitzen?
	public bool isSeat;
	// Ist der Spot besetzt?
	public bool isTaken;

	// Use this for initialization
	void Start () {
		// Wenn der Spot Aktiv ist, füge ihn zur Spotlist des Spotmanagers hinzzu
		SpotManager.Instance.spotList.Add (gameObject);
	}
	

}
