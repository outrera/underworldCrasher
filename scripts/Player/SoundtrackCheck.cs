using UnityEngine;
using System.Collections;

public class SoundtrackCheck : MonoBehaviour {

	private RoundSystem nextRound;

	public AudioSource drinkmusic;

	public AudioSource calmmusic;

	// Use this for initialization
	void Start () {
		// Auf RoundSystem verweisen, um daraus den Rundenstatus abzufragen
		nextRound = GameObject.FindGameObjectWithTag("GameController").GetComponent<RoundSystem>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void SoundCheck(){
		if (nextRound.DrinkingPhase == false) {
			drinkmusic.Stop();
			calmmusic.Play ();
		} else {
			calmmusic.Stop();
			drinkmusic.Play();

		}

	}
}
