// Dieses Script controlliert das EndPanel.
// Es wird dem EndPhasePanel, was man anklicken muss, um das Panel zu oeffnen angeheftet

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlEndPanel : MonoBehaviour {
	
	// Panel definieren
	public GameObject thePanel;
	// Rundensystem
	public RoundSystem nextRound;
	// Screenfading
	private ScreenFader nextRoundFadeIn;
	
	// Funktion, die oben definiertes Panel schliesst und dabei wieder einfadet und eine Runde weiter geht
	public void close()
	{

		nextRound = GameObject.FindGameObjectWithTag("GameController").GetComponent<RoundSystem>();
		nextRoundFadeIn = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScreenFader>();

		// Die Endphase vom Roundsystem wird auf false gesetzt. Die Kaufphase beginnt also wiede
		nextRound.EndPhase = false;
		// Die Rundenzahl wird um 1 erhoeht
		nextRound.currentRound ++;
		// Das Panel wird inaktiv gesetzt
		thePanel.SetActive(false);

		// Das FadeIn wird initialisiert
		nextRoundFadeIn.BeginFade (-1);
	}
	
	
}
