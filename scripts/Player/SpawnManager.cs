// Sapwnmanager: Beinhaltet Gasttypen und die Coroutinen zum spawnen
// Wird dem Spieler angeheftet

using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour {

	// Singleton, um von anderen Klassen auf den Spawnmanager zugreifen zu können
	private static SpawnManager instance;
	public static SpawnManager Instance{
		get { 
			if (instance == null){				// Wenn Instanz nicht gesetzt ist, dann setze sie
				instance = GameObject.FindObjectOfType<SpawnManager>();
			}
			return SpawnManager.instance;
		}
	}

	// Spawnpunkt, andem die Gäste spawnen
	public GameObject spawnPoint;

	// https://www.youtube.com/watch?v=Vrld13ypX_I
	// bei 11:14 sieht man es gut
	[System.Serializable] // Notwendig, um Klasse im Inspector zu sehen
	// Gastklasse
	// Funktioniert wie eine Art Blaupause für Charaktere. Mit ihr werden die Charakterklassen erstellt, die dann spawnen
	public class GuestType{

		// Name des Gasttyps
		public string name;
		// Charakter Objekt des Gasts
		public GameObject guest;
		// Überprüft, ob der Gast schon gespawnt werden darf
		public bool isGuestAvailable;
		// Gibt an, wie viele Gäste von diesem Gasttyp gespawned werden dürfen
		public int guestLimit;
		// Zählt wie viele Gäste schon gespawned wurden
		public int guestCount = 0;

	}

	// Erstellung eines Arrays aus GuestType
	public GuestType[] guestType;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		// WENN endphase ist, dann stoppe coroutine
		if (RoundSystem.Instance.EndPhase == true) {
			StopCoroutine("Spawn");
		}
	}


	// https://www.youtube.com/watch?v=sKyIk9Nitwc
	// Spawn Coroutine
	// Spawn die Gäste in Zeitintervallen, solange die Coroutine läuft
	public IEnumerator Spawn(){

		// Solange die Coroutine läuft wird die Spawnschleife durchlaufen
		while (true) {

			// Für jeden Gasttyp, der in dem Array aus Gasttypen ist
			foreach (GuestType Guest in guestType){
				// WENN:
				// - Der Gasttyp verfügbar ist
				// - Das Gastzähler kleiner als das Gastlimit ist
				if(Guest.isGuestAvailable == true && Guest.guestCount < Guest.guestLimit){
					// DANN erstelle eine Instanz des CharakterObjekts des Gasttyps, spawne es am Spawnpunkt, mit der Rotation, die das Objekt hat
					Instantiate (Guest.guest, spawnPoint.transform.position, Quaternion.identity);
					// Erhöhe den Gastzähler für diesen Gasttyp um 1
					Guest.guestCount++;
				}
			}

			// Warte Sekunden bevor du nochmal spawnen lässt
			yield return new WaitForSeconds(4f);
		}


	}

	// Funktion die "Spawn" startet. Muss als Funktion verpackt werden, da Coroutinen nicht extern gestartet oder gestoppt werden können
	public void StartSpawn(){
		StartCoroutine ("Spawn");
	}




}
