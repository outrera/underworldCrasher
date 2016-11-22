// Screenfader. Dieses Script fadet das BlackImage ein und aus. Es wird dem Player angehangen.
// Es bezieht sich auf dieses Tutorial: https://www.youtube.com/watch?v=0HwZQt94uHQ

using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class ScreenFader : MonoBehaviour
{

	public Image fadeOutTexture; // the fading image
	public float fadeSpeed = 0.8f;   // the fading speed
	
	private float alpha = 1.0f; // the alpha Value of the Image
	private int fadeDir = -1; // the fading direction ( -1 = from black to visible; 1 = from visible to black)

	// Roundsystem um Rundenstatus abzufragen
	public RoundSystem fadeControl;


	void Update(){
		// Rufe FadeControl auf
		FadeControl ();
	}

	void OnGUI () {
		// Der AlphavalueFade, der sich aus Richtung, Speed und der Zeit ergibt
		alpha += fadeDir * fadeSpeed * Time.deltaTime;

		// Er wird in einen Wert zwischen 0 und 1 umgewandelt (weil Alpha immer zwischen 0 und 1 sein muss
		alpha = Mathf.Clamp01 (alpha);

		// Die Farbe des Images wird auf eine neue Farbe gesetzt, bei der sich RGB NICHT veraendern, der alphawert aber um "alpha"
		fadeOutTexture.color = new Color (fadeOutTexture.color.r, fadeOutTexture.color.g, fadeOutTexture.color.b, alpha);
		// Passt das Image auf den Bildschirm an (funktioniert aber gar nicht, stattdessen wurde es im Inspector vom Image so eingestellt)
		// fadeOutTexture.rectTransform.sizeDelta = new Vector2 (Screen.width, Screen.height);
	}

	// Diese Funktion startet das Faden und erwartet die Richtung (1 oder -1)
	public float BeginFade (int direction){
		fadeDir = direction;
		return (fadeSpeed);
	}

	// Wenn eine Szene neu geladen wird, wird eingefadet
	void OnLevelWasLoaded(){
		BeginFade (-1);
	}

	// Initialisierung des Roundsystems
	void Start(){
		fadeControl = GameObject.FindGameObjectWithTag("GameController").GetComponent<RoundSystem>();
	}

	// Wenn die Endphase beginnt, soll gefadet werden. Ausgefadet wird mit einem Click auf den EndPhaseButton, mit dem Script ControlEndPanel
	void FadeControl(){
		if (fadeControl.DrinkingPhase == false && fadeControl.EndPhase == true) {
			BeginFade (1);
		} 
	}
}   

