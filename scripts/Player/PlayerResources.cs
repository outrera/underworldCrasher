// Dieses Script verwaltet die Ressourcen des Spielers

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerResources : MonoBehaviour {

	// Singleton, damit andere Klassen auf PlayerResources zugreifen können
	private static PlayerResources instance;
	public static PlayerResources Instance{
		get { 
			if (instance == null){				// Wenn Instanz nicht gesetzt ist, dann setze sie
				instance = GameObject.FindObjectOfType<PlayerResources>();
			}
			return PlayerResources.instance;
		}
	}

	// Interne Geldvariable
	private int Money = 200;
	// Text Variable, die Geld repräsentiert
	public Text moneyText;
	// Oeffentliche Geld Variable, welche aus der internen besteht, welche stehts geupdatet wird
	public int money{
		get{
			return Money;
		}
		set{
			Money = value;
			if(Money < 0)
				Money = 0;
			UpdateView ();
		}
	}

	// Update GUI beim Start der Szene
	void Start () {
		UpdateView ();
	}
	
	// GUI aktualisieren
	void UpdateView () {
		moneyText.text = Money.ToString ();
	}
}
