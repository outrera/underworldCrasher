// Verwaltet die Sitzplätze

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpotManager : MonoBehaviour {

	// Singleton
	private static SpotManager instance;
	public static SpotManager Instance{
		get { 
			if (instance == null){				// Wenn Instanz nicht gesetzt ist, dann setze sie
				instance = GameObject.FindObjectOfType<SpotManager>();
			}
			return SpotManager.instance;
		}
	}

	// Liste von Sitzplätzen (emptys)
	public List<GameObject> spotList;


}
