// Dieses Script verwaltet Konstante Werte, wie die Scrollgeschwindigkeit

using UnityEngine;
using System.Collections;

//Definiere eigenen Namespace namens Simulation
namespace Simulation {
	public static class ResourceManager {
		// Definiere Scrollspeed
		//  this is the standard C# way of define getters for private variables within a class. 
		// Here we are defining that we always want to get a constant value back. 
		// This value could easily be stored in a private variable within the class. 
		// Doing that would also make things easier to customise later on (e.g. from an options menu), but this is fine for now. 
		public static float ScrollSpeed { get { return 10; } }
		// Definiere RotateSpeed
		public static float RotateSpeed { get { return 100; } }
		// Definiere die Anzahl an Pixeln vom Bildschirmrand, bei denen Scrolling startet
		public static int ScrollWidth { get { return 25; } }
		// Definiere die minimale Kamera Zoomhoehe
		public static float MinCameraHeight { get { return 6; } }
		// Definiere die maximale Kamera Zoomhoehe
		public static float MaxCameraHeight { get { return 10; } }
	}
}
