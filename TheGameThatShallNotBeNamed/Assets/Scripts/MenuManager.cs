﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System;

public class MenuManager : MonoBehaviour {
	
	public GameObject mainMenu;
	public GameObject workshopMenu;
	public GameObject destMenu;
	public GameObject tavernMenu;
	public GameObject equipmentPrefab;

	List<GameObject> weaponButtons;
	List<GameObject> armorButtons;

	Equipment equip;
	
	//set correct menu options to active
	void Start(){
		equip = GameObject.Find ("PersistentData").GetComponent<Equipment>();
		destMenu.SetActive(false);
		workshopMenu.SetActive(false);
		tavernMenu.SetActive(false);
		SetupWorkshop();
	}
	
	//setup workshop
	public void SetupWorkshop(){
		int currentY = 200;
		for(int i=0;i<equip.allWeapons.Count;i++){
			GameObject currentEquip = equipmentPrefab;
			GameObject finished = (GameObject)Instantiate(currentEquip, new Vector3(workshopMenu.transform.position.x-200.0f,workshopMenu.transform.position.y+currentY,0.0f), Quaternion.identity);
			finished.transform.parent = workshopMenu.transform.FindChild("Window/AllObjects");

			GameObject imgObj = finished.transform.FindChild("Image").gameObject;
			//set image = equipment image path
			GameObject nameObj = finished.transform.FindChild("NameDesc").gameObject;
			nameObj.GetComponent<Text>().text = equip.allWeapons[i].name;
			GameObject statsObj = finished.transform.FindChild("Stats").gameObject;
			string text = equip.allWeapons[i].str + "    " + equip.allWeapons[i].end + "     " + equip.allWeapons[i].agi + "     " + equip.allWeapons[i].mag + "     " + equip.allWeapons[i].luck + "    " + equip.allWeapons[i].rangeMin + " - " + equip.allWeapons[i].rangeMax;
			statsObj.GetComponent<Text>().text = text;
			GameObject recipeObj = finished.transform.FindChild("Recipe").gameObject;

			GameObject createObj = finished.transform.FindChild("Create").gameObject;
			int captured = i;
			createObj.GetComponent<Button>().onClick.AddListener(() => GiveWeapon(captured));

			currentY -= 75;
		}
		
		for(int i=0;i<equip.allArmor.Count;i++){
			GameObject currentEquip = equipmentPrefab;
			GameObject finished = (GameObject)Instantiate(currentEquip, new Vector3(workshopMenu.transform.position.x-200.0f,workshopMenu.transform.position.y+currentY,0.0f), Quaternion.identity);
			finished.transform.parent = workshopMenu.transform.FindChild("Window/AllObjects");

			GameObject imgObj = finished.transform.FindChild("Image").gameObject;
			//set image = equipment image path
			GameObject nameObj = finished.transform.FindChild("NameDesc").gameObject;
			nameObj.GetComponent<Text>().text = equip.allArmor[i].name;
			GameObject statsObj = finished.transform.FindChild("Stats").gameObject;
			string text = equip.allArmor[i].str + "    " + equip.allArmor[i].end + "     " + equip.allArmor[i].agi + "     " + equip.allArmor[i].mag + "     " + equip.allArmor[i].luck + "     " ;
			statsObj.GetComponent<Text>().text = text;
			GameObject recipeObj = finished.transform.FindChild("Recipe").gameObject;
			
			GameObject createObj = finished.transform.FindChild("Create").gameObject;
			int captured = i;
			createObj.GetComponent<Button>().onClick.AddListener(() => GiveArmor(captured));
			//armorButtons.Add(createObj);

			currentY -= 75;
		}

	}

	void GiveWeapon(int i){
		GameObject perData = GameObject.FindGameObjectWithTag("Persistent");

		Debug.Log (perData.GetComponent<PlayerData>().obtainedWeapons.Count);
		perData.GetComponent<PlayerData>().obtainedWeapons.Add(equip.allWeapons[i]);

        saveXML(i, true);
	}
	void GiveArmor(int i){
		GameObject perData = GameObject.FindGameObjectWithTag("Persistent");

		Debug.Log (perData.GetComponent<PlayerData>().obtainedArmor.Count);
		perData.GetComponent<PlayerData>().obtainedArmor.Add(equip.allArmor[i]);

        saveXML(i, false);
	}


	//function handlers for each button yes this is ugly lay off...
	public void MainToDest(){
		mainMenu.SetActive(false);
		destMenu.SetActive(true);
	}
	public void DestToMain(){
		destMenu.SetActive(false);
		mainMenu.SetActive(true);
	}
	public void MainToWorkshop(){
		mainMenu.SetActive(false);
		workshopMenu.SetActive(true);
	}
	public void MainToTavern(){
		mainMenu.SetActive(false);
		tavernMenu.SetActive(true);
	}
	public void TaverToMain(){
		mainMenu.SetActive(true);
		tavernMenu.SetActive(false);
	}
	public void WorkToMain(){
		workshopMenu.SetActive(false);
		mainMenu.SetActive(true);
	}
	public void Quit(){
		Application.Quit();
	}

	public void VisitDest(int level){
		Application.LoadLevel (2);
	}

	public void RecruitGuildMember(){
		XDocument doc = XDocument.Load("Assets/Characters/GuildList.xml");
		XElement root = new XElement("char");
		root.Add(new XAttribute("name", "New Member"));
		root.Add(new XAttribute("desc", "A new recruit"));
		root.Add(new XAttribute("health", "50"));
		root.Add(new XAttribute("str", "7"));
		root.Add(new XAttribute("end", "7"));
		root.Add(new XAttribute("agi", "7"));
		root.Add(new XAttribute("mag", "7"));
		root.Add(new XAttribute("luck", "7"));
		root.Add(new XAttribute("range", "2"));
		root.Add(new XAttribute("active", "True"));
		doc.Element("guild").Add(root);
		doc.Save("Assets/Characters/GuildList.xml");
	}

	public void RemoveGuildMember(){
		
	}

    //Loads a dungeon based off of its name
    public void goToDest(string name)
    {
        //Debug.Log("Going to " + name);
        string filePath = Application.dataPath + @"/Dungeons/DungeonList.xml";

        XmlDocument dunXML = new XmlDocument();

        if (File.Exists(filePath))
        {
            dunXML.Load(filePath);

            //List of our possible dungeons
            XmlNodeList dungeons = dunXML.GetElementsByTagName("dungeon");

            //Set the selected dungeon to active
            foreach (XmlNode member in dungeons)
            {
                if (member["name"].InnerText == name)
                {
                    member["active"].InnerText = "true";
                    dunXML.Save(filePath);
                }

            }
        }
        Application.LoadLevel(2);
    }

    public void saveXML(int i, bool wep)
    {
        XmlDocument xmlDoc = new XmlDocument();
        string path;

        //Add a weapon
        if(wep)
        {
            path = Application.dataPath + @"/ItemsAndEquipment/WeaponInventory.xml";

            if (File.Exists(path))
            {
                xmlDoc.Load(path);

                XmlNodeList weapons = xmlDoc.GetElementsByTagName("weapon");

                    bool newEntry = true;

                    //Check to see if we already have that item
                    foreach (XmlNode member in weapons)
                    {
                        if (member.Attributes["id"].Value == i.ToString())
                        {
                            //Update the count of the item
                            Debug.Log("Found a: " + member.Attributes["name"].Value);
                            int currentCount = int.Parse(member.Attributes["count"].Value);
                            currentCount++;
                            member.Attributes["count"].Value = currentCount.ToString();

                            //Don't need to make a new entry
                            newEntry = false;
                        }
                    }

                    //If we don't have that item yet
                    if (newEntry)
                    {
                        Debug.Log("Adding a new " + equip.allWeapons[i].name);

                        //Create the new item
                        XmlNodeList root = xmlDoc.GetElementsByTagName("inventory");
                        XmlElement newItem = xmlDoc.CreateElement("weapon");
                        newItem.SetAttribute("name", equip.allWeapons[i].name);
                        newItem.SetAttribute("id", i.ToString());
                        newItem.SetAttribute("count", "1");
                        root[0].AppendChild(newItem);
                    }

                }
                xmlDoc.Save(Application.dataPath + @"/ItemsAndEquipment/WeaponInventory.xml");
            }

        //Add armor
        else
        {
            path = Application.dataPath + @"/ItemsAndEquipment/ArmorInventory.xml";

            if (File.Exists(path))
            {
                xmlDoc.Load(path);

                XmlNodeList armor = xmlDoc.GetElementsByTagName("armor");

                bool newEntry = true;

                //Check to see if we already have that item
                foreach (XmlNode member in armor)
                {
                    if (member.Attributes["id"].Value == i.ToString())
                    {
                        //Update the count of the item
                        Debug.Log("Found a: " + member.Attributes["name"].Value);
                        int currentCount = int.Parse(member.Attributes["count"].Value);
                        currentCount++;
                        member.Attributes["count"].Value = currentCount.ToString();

                        //Don't need to make a new entry
                        newEntry = false;
                    }
                }

                //If we don't have that item yet
                if (newEntry)
                {
                    Debug.Log("Adding a new " + equip.allWeapons[i].name);

                    //Create the new item
                    XmlNodeList root = xmlDoc.GetElementsByTagName("inventory");
                    XmlElement newItem = xmlDoc.CreateElement("armor");
                    newItem.SetAttribute("name", equip.allArmor[i].name);
                    newItem.SetAttribute("id", i.ToString());
                    newItem.SetAttribute("count", "1");
                    root[0].AppendChild(newItem);
                }

            }
            xmlDoc.Save(Application.dataPath + @"/ItemsAndEquipment/ArmorInventory.xml");

        }
    }
}
