// Dieses Script steuer den Input der vom User kommt
// Um die Kamera zu bewegen

using UnityEngine;
using System.Collections;
using Simulation;

public class UserInput : MonoBehaviour {

	// Referenz zu dem Spieler, für den wir den Input machen
	private Player player;
	// Initialisierung des Spielers

	// Use this for initialization
	void Start () {
		// Initialisierung des Spielers
		// Finde die Klasse Player (die im Player.cs Script ist)
		// Suche dafür auf der "root" (also auf dem Objekt, zu dem UserInput.cs gehoert)
		player = transform.root.GetComponent<Player>();
	

	}
	
	// Update is called once per frame
	// Bewegung wird also jeden Frame neu kalkuliert
	void Update () {
		// Wenn Spieler menschlich ist
		// Dann erlaube ihm MoveCamera und RotateCamera auszufuehren
		if(player.human) {
			// Rufe Funktion MoveCamera() auf
			MoveCamera();
			// Rufe Funktion RotateCamera() auf
			//RotateCamera();
		}
	}

	private void MoveCamera() {
		// Definieren der aktuellen Mausposition
		float xpos = Input.mousePosition.x;
		float ypos = Input.mousePosition.y;
		// Bewegungsvektor
		Vector3 movement = new Vector3 (0, 0, 0);

		// Horizontale Kamera Bewegung
		// WENN die X-Position der Maus groessergleich 0 UND kleiner als die Scrollbreite ist (also dazwischen Liegt bzw. am linken Bildschrimrand ist)
		// DANN bewege dich negativ auf der X-Achse (Immer Wert auf X Achse abziehen), entsprechend dem Scrolling Speed (reduziere die Position auf der x-Achse entsprechend dem Scrollingspeed)
		// ANSONSTEN WENn die X-Position der Maus kleinergleich der maximalen Bildschirmbreite UND größer als Bildschirmbreite minus Scrollbreite ist (also im Scrollbereich rechts liegt)
		// DANN bewege dich positiv auf der X-Achse (Immer Wert auf X Achse addieren), entsprechend dem Scrolling speed (addiere die Position auf der x-Achse entsprechend dem Scrollingspeed)
		if (xpos >= 0 && xpos < ResourceManager.ScrollWidth) {
			movement.x -= ResourceManager.ScrollSpeed;
		} else if (xpos <= Screen.width && xpos > Screen.width - ResourceManager.ScrollWidth) {
			movement.x += ResourceManager.ScrollSpeed;
		}

		// Vertikale Kamera Bewegung
		// Gleiche Code-Logik wie bei Horizontal, nur auf z Achse und mit Bildschirmhoehe statt Breite
		if(ypos >= 0 && ypos < ResourceManager.ScrollWidth) {
			movement.z -= ResourceManager.ScrollSpeed;
		} else if(ypos <= Screen.height && ypos > Screen.height - ResourceManager.ScrollWidth) {
			movement.z += ResourceManager.ScrollSpeed;
		}

		// Alternative Bewegung mit WASD
		if(Input.GetKey (KeyCode.A)){
			movement.x -= ResourceManager.ScrollSpeed;
		}
		if(Input.GetKey (KeyCode.D)){
			movement.x += ResourceManager.ScrollSpeed;
			
		}
		if(Input.GetKey (KeyCode.S)){
			movement.y -= ResourceManager.ScrollSpeed;
		}
		
		if(Input.GetKey (KeyCode.W)){
			movement.y += ResourceManager.ScrollSpeed;
		}

		// Zoomen der Kamera
		// make sure movement is in the direction the camera is pointing
		// but ignore the vertical tilt of the camera to get sensible scrolling
		movement = Camera.main.transform.TransformDirection(movement);
		movement.y = 0;

		// away from ground movement
		// Bewegung auf der y-achse soll definiert werden durch Input durch Mausrad MAL ScrollSpeed 
		movement.y -= ResourceManager.ScrollSpeed * Input.GetAxis("Mouse ScrollWheel");


		// Kalkulieren der Kameraposition, basierend auf Input des Users
		// Definiere die aktuelle Kameraposition und nenne sie "origin"
		Vector3 origin = Camera.main.transform.position;
		// Definiere die Kamera destination, welche gleich dem origin sein soll
		Vector3 destination = origin;
		// Die Destianations ergeben sich auf dem Movementvektor der drei Achsen
		destination.x += movement.x;
		destination.y += movement.y;
		destination.z += movement.z;

		// Definieren des Bewegungslimits
		// Zoomlimit
		// WENN Bewegung auf der Y-Achse groesser als maximale Kamerahoehe ist, setze Bewegungsvektor (Position) auf maximale Kamerahoehe (Hoehe kann so nicht überschritten werden)
		// ANSONSTEN WENN Bewegung auf der Y-Achse kleiner als mindest Kamerahoehe ist, setze Bewegungsvektor (Position) auf mindes Kamerahoehe (Kamera kann nicht unter diese Hoehe kommen)
		if(destination.y > ResourceManager.MaxCameraHeight) {
			destination.y = ResourceManager.MaxCameraHeight;
		} else if(destination.y < ResourceManager.MinCameraHeight) {
			destination.y = ResourceManager.MinCameraHeight;
		}

		// WENN Die destination UNGLEICH der aktuellen Position ist (also wenn eine Veraenderung in aktueller Position geschieht)
		// DANN Setze die aktuelle Kameraposition auf die destination bzw. bewege die Kamera von origin zu destination mit dem Scrollspeed
		if(destination != origin) {
			Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * ResourceManager.ScrollSpeed);
		}
	}
	
	private void RotateCamera() {
		// Rotierende Kamera?? Notwendig?
	}
}

