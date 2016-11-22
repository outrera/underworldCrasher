using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour {

	// Singleton
	private static InventoryManager instance;
	public static InventoryManager Instance{
		get{
			if (instance == null){
				instance = FindObjectOfType<InventoryManager>();
			}
			return instance;
		}
	}


	// Slot Prefab (vom Slot Image)
	public GameObject slotPrefab;

	// Prefab fuer das Icon welches der Maus folgt, wenn man das Item bewegt
	public GameObject iconPrefab;

	// Instanziertes GameObjekt, welches das eigentliche Gameobject ist, dass der Maus folgt
	private GameObject hoverObject;
	public GameObject HoverObject{
		get { return hoverObject; }
		set { hoverObject = value;}
	}

	// Hintergrund des ToolTips
	public GameObject tooltipObject;
	// Text, der den Tooltip anhand des Inhalts skaliert
	public Text sizeTextObject;
	// Sichtbarer Tooltiptext
	public Text visualTextObject;

	// Referenz zum Canvas in dem das Inventar ist
	public Canvas inventoryCanvas;

	// Variablen zum verschieben von Objekten. Slot von dme wir etwas bewegen und Slot zu dem wir etwas bewegen
	private Slot from;
	public Slot From{
		get { return from; }
		set { from = value;}
	}

	private Slot to;
	public Slot To{
		get { return to; }
		set { to = value;}
	}

	// clicked für HoverIcon
	private GameObject clicked;
	public GameObject Clicked{
		get { return clicked; }
		set { clicked = value;}
	}


	public Text stackText;

	public GameObject selectStackSize;

	// Anzahl der Items, die wir raussplitten
	private int splitAmount;
	public int SplitAmount{
		get { return splitAmount; }
		set { splitAmount = value;}
	}
	// Speichert die maximale Anzahl, die wir raussplitten koennen
	private int maxStackCount;
	public int MaxStackCount{
		get { return maxStackCount; }
		set { maxStackCount = value;}
	}
	// Temporaerer Slot wo die Items die gesplittet werden gespeichert werden
	private Slot movingSlot;
	public Slot MovingSlot{
		get { return movingSlot; }
		set { movingSlot = value;}
	}

	// Eventsystem
	public EventSystem eventSystem;

	public void Start(){
		CraftingBench.Instance.CreateBlueprints ();
	}

	// Funktion die Informationen über den Slot zurueckgibt
	// Es benoetigt den maxStackCount (Slot "sagt" Inventar, wie viel maximal stacken koennen)
	public void SetStackInfo(int maxStackCount){
		
		// UI Element aktivieren
		selectStackSize.SetActive (true);
		//
		tooltipObject.SetActive (false);
		// Split Amount ist anfangs immer 0
		splitAmount = 0;
		// Slot "sagt" Inventory wie viel Maximum zum splitten ist (?)
		this.maxStackCount = maxStackCount;
		// split Amount zeigt stackText wie viele Items man splitten will
		stackText.text = splitAmount.ToString ();
	}
}
