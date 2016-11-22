using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoad : MonoBehaviour {

    // List that saves all Items in the Game by hand to have them as reference
    // On load the game runs through the list and checks every item
    // If item was in the inventory at save, it will be loaded
    public GameObject[] itemList;
    public GameObject[] buyObjectsList;



    // Use this for initialization
    void Start () {
	
	}

    // Save function
    public void Save() {
       
        // A new Binary Format
        BinaryFormatter binary = new BinaryFormatter();
        // To write files: Open the FileStream and wirte saveFile.tavern in the persistentDataPath (appdata) of the Application
        FileStream fStream = File.Create(Application.persistentDataPath + "/saveFile.tavern");

        // To Read out the Data of the SaveManager class
        // Reference to Savemanager
        SaveManager saver = new SaveManager();
        // Saves the money to saver
        saver.currentMoney = PlayerResources.Instance.money;
        // saves the current round to saver
        saver.currentRound = RoundSystem.Instance.currentRound;


        // ===== Inventory Save =====
        string content = string.Empty; //Creates a string for containing infor about the items inside the inventory

        for (int i = 0; i < Inventory.Instance.AllSlots.Count; i++) //Runs through all slots in the inventory
        {
            Slot tmp = Inventory.Instance.AllSlots[i].GetComponent<Slot>(); //Creates a reference to the slot at the current index

            if (!tmp.IsEmpty) //We only want to save the info if the slot contains an item
            {
                //Creates a string with this format: SlotIndex-ItemType-AmountOfItems; this string can be read so that we can rebuild the inventory
                content += i + "-" + tmp.CurrentItem.type.ToString() + "-" + tmp.Items.Count.ToString() + ";";
            }
        }

        // saves the content string to saver
        saver.inventoryContent = content;
        // Saves the rows to saver
        saver.currentRows = Inventory.Instance.rows;
        // Saves the slots to saver
        saver.currentSlots = Inventory.Instance.slots;
        // ===== Inventory Save Finish =====

        // ===== BuyObject List =====
        content = string.Empty;
        foreach (GameObject objectTmp in buyObjectsList) {
            BuyObject buyObjectTmp = objectTmp.GetComponent<BuyObject>();
            if (objectTmp.activeSelf == true) {
                content += buyObjectTmp.objectName + "-true;";
            }
        }

        saver.buyObjectContent = content;

        // all other...

        // Save "saver" to FileStream "fStream"
        binary.Serialize(fStream, saver);
        // Close the file stream (end file writing)
        fStream.Close();

    }

    // Load function
    public void Load() {
        // If the savegame exists
        if (File.Exists(Application.persistentDataPath + "/saveFile.tavern")){

            // A new binary Formatter
            BinaryFormatter binary = new BinaryFormatter();
            // Open the filestream of the .tavern file
            FileStream fstream = File.Open(Application.persistentDataPath + "/saveFile.tavern", FileMode.Open);
            // Take Data out of File Stream, deserialize them and save them in "saver"
            SaveManager saver = (SaveManager)binary.Deserialize(fstream);
            // Close the file stream
            fstream.Close();

            // Read the playermoney of the savegame
            PlayerResources.Instance.money = saver.currentMoney;
            // Read the currentRound of the savegame
            RoundSystem.Instance.currentRound = saver.currentRound;

            // Creates a new string to read the Inventory String of the savegame file
            string content = saver.inventoryContent;
            // If the string is NOT empty
            if (content != string.Empty)
            {
                // Read the rows
                Inventory.Instance.rows = saver.currentRows;
                // Read the slots
                Inventory.Instance.slots = saver.currentSlots;

                // Layout muss neu geschrieben werden: FUNKTIONIERT AUS IRGENDEINEM GRUND NICHT
                // EINFACH KOMMENTAR WEGMACHEN ZUM TESTEN. Der uebrige Code, der die reihen und slots speichert ist noch aktiv
                // Inventory.Instance.CreateLayout(); // Should recreate the Layout...does not work?!
                

                //Splits the loaded content string into segments, so that each index in the splitContent array contains information about a single slot
                //e.g[0]0-MANA-3
                string[] splitContent = content.Split(';');

                //Runs through every single slot we have information about -1 is to avoid an empty string error
                for (int x = 0; x < splitContent.Length - 1; x++)
                {
                    //Splits the slot's information into single values, so that each index in the splitValues array contains info about a value
                    //E.g[0]InventorIndex [1]ITEMTYPE [2]Amount of items
                    string[] splitValues = splitContent[x].Split('-');

                    //int index = Int32.Parse(splitValues[0]); //InventorIndex 

                    string itemName = splitValues[1]; //ITEMTYPE

                    int amount = Int32.Parse(splitValues[2]); //Amount of items

                    Item tmp = null; // Resets the tmp Item

                    for (int i = 0; i < amount; i++) //Adds the correct amount of items to the inventory
                    {
                        // Run through the itemList which is attached to SaveLoad (look at the top of the script)
                        // This loop is necessary because we can not add items from nothing
                        // We need the reference list to check which Item is to add
                        foreach (GameObject tmpItem in itemList) {
                            // Create a temporary reference to the Item script
                            Item tmp1 = tmpItem.GetComponent<Item>();
                            // If tmp is empty
                            if (tmp == null)
                            {
                                // If the itemType (saved in itemName) is the same type as the one of the list (toString to make them compareable, because itemName is a string)
                                if (itemName == tmp1.type.ToString())
                                {
                                    // THEN save the current Item of the foreach-Loop in ladedItem
                                    GameObject loadedItem = tmpItem;
                                    // And add it to the Inventory
                                    Inventory.Instance.AddItem(loadedItem.GetComponent<Item>());
                                    //Destroy(loadedItem);// Necessary? If Item is destroyed it cannot be bought anymore?
                                }
                            }
                        }   
                    }
                }
            }

            // Resets content string string to read the buyObject String of the savegame file
            content = string.Empty;
            content = saver.buyObjectContent;

            if (content != string.Empty) {
                //Splits the loaded content string into segments, so that each index in the splitContent array contains information about a single slot
                //e.g[0]Table1-true;
                string[] splitContent = content.Split(';');

                for (int x = 0; x < splitContent.Length - 1; x++) {
                    //Splits the slot's information into single values, so that each index in the splitValues array contains info about a value
                    //E.g[0]InventorIndex [1]ITEMTYPE [2]Amount of items
                    string[] splitValues = splitContent[x].Split('-');

                    string buyObjectName = splitValues[0]; //ITEMNAME
                    bool isBought = Boolean.Parse(splitValues[1]); //IS AKTIVE?

                    BuyObject tmp = null; // Resets the tmp Item

                    foreach (GameObject tmpBuyObject in buyObjectsList) {
                        // Create a temporary reference to the Item script
                        BuyObject tmp1 = tmpBuyObject.GetComponent<BuyObject>();
                        // If tmp is empty
                        if (tmp == null) {
                            // If the itemType (saved in itemName) is the same type as the one of the list (toString to make them compareable, because itemName is a string)
                            if (buyObjectName == tmp1.objectName.ToString() && isBought == true) {
                                tmpBuyObject.SetActive(true);
                              
                            }
                        }
                    }
                }      
            }

            //all other...
        }
    }

    

}

// SaveManager Class
// Acts as a "translator" or temporary RAM between game and binary file
[Serializable] // It is able to written to a BinaryFile
class SaveManager {

    public int currentMoney;
    public int currentRound;

    public string inventoryContent;
    public int currentSlots;
    public int currentRows;

    public string buyObjectContent;
    //add stuff...
    // Serializable is:
    // int, float, bool, string
    // vector 2 & 3 & 4, quaternions, matrix 4x4, color, rect, layermask
    // unityengine.object - gameobject component monobehaviour texture2d animationclips
    // enums
    // arrays & lists



}
