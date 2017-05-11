﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour {
    public static InventoryManager thisManager;

    public GameObject buttonPref;
    
    GameObject[] contentObjects = new GameObject[3];

    public GameObject optionWindowPref;


    List<Item> inventory = new List<Item>();
    List<Transform> inventoryButtons = new List<Transform>();
    void Update() {

    }

    void Awake() {
        thisManager = this;
        for (int i = 0; i < 3; i++) {
            contentObjects[i] = GameObject.FindGameObjectWithTag("Content" + i);
        }

    }
    void Start()
    {
        InventoryAdd(0, 10);
        InventoryAdd(1, 10);
    }
    // Is called upon when adding an item to the inventory
    public void InventoryAdd(int item,int count) {
        if (DataBaseManager.thisManager.ReturnItem(item) != null) {
            Item newItem = DataBaseManager.thisManager.ReturnItem(item);
            int i = SearchInventory(newItem);
            switch (newItem.category) {
                case 1:
                    Consumable newCon = newItem as Consumable;
                    if (i != -1) {
                        newCon = inventory[i] as Consumable;
                        newCon.count += count;
                        inventoryButtons[i].GetComponent<ItemButton>().UpdateCount(newCon.count);
                        return;
                    }
                    else {
                        newCon.count = count;
                        newItem = newCon;
                    }
                    break;
                case 2:
                    CraftingObject newCraft = newItem as CraftingObject;
                    if (i != -1) {
                        newCraft = inventory[i] as CraftingObject;
                        newCraft.count += count;
                        inventoryButtons[i].GetComponent<ItemButton>().UpdateCount(newCraft.count);
                        return;
                    }
                    else {
                        newCraft.count = count;
                        newItem = newCraft;
                    }
                    break;
            }
            inventoryButtons.Add(Visualize(newItem));
            inventory.Add(newItem);
        }

    }
    //Instantiates the button which represents the item.
    Transform Visualize(Item item) {
        Transform newButton = Instantiate(buttonPref, Vector3.zero, Quaternion.identity).transform;
        newButton.SetParent(contentObjects[item.category].transform);
        helpArrange(newButton);
        newButton.GetComponent<ItemButton>().FillValues(item);
        return newButton;
    }
    //Used to delete the item received from the inventory
    public void Delete(Item item) {
        if(item.category == 0) {
            //check if equiped else delete
        }
        else {
            print("koek");
            int a = 0;
            List<Item> checkList = GetCategory(item.category);
            foreach(Item i in checkList) {

                print("InvCheck");
                if(checkList[a] == item) {
                    inventory.RemoveAt(a);
                    Destroy(inventoryButtons[a].gameObject);
                    inventoryButtons.RemoveAt(a);
                }
                a++;

            }
        }

    }
    //Activated when consuming or equiping an item.
    public void Use(Item item) {
        switch (item.category) {
            case 1:
                //Use potion somewhere
                Consumable itemCon = inventory[SearchInventory(item)] as Consumable;
                HealthManager.thisManager.UpdateHP(100);
                if (itemCon.count - 1 == 0) {
                    Delete(itemCon);
                }
                else {
                    itemCon.count--;
                    inventoryButtons[SearchInventory(item)].GetComponent<ItemButton>().UpdateCount(itemCon.count);

                }
                break;
        }
    }
    //Gives back a list full of items which are of the requested category
    List<Item> GetCategory(int category) {
        List<Item> scanList = new List<Item>();
        for (int a = 0; a < inventory.Count; a++) {
            if (inventory[a].category == category) {
                scanList.Add(inventory[a]);
            }
        }
        return scanList;
    }
    public void helpArrange(Transform newButton) {
        newButton.localPosition = Vector3.zero;
        newButton.localRotation = Quaternion.identity;
        newButton.localScale = new Vector3(1, 1, 1);
    }
    //Gives back the index of the given item.
    int SearchInventory(Item item) {
        int i = 0;
        foreach(Item it in inventory) {
            if (it == item) {
                return i;
            }
            i++;
        }
        return -1;
    }
} 