﻿using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

    public Transform player;
	public GameObject selectedObject; //Currently selected player or enemy or tile
	public PlayerController[] allPlayers;
	public Enemy[] allEnemies;
	enum Turn {PlayerTurn, EnemyTurn};
	Turn currentTurn;

	// Use this for initialization
	void Start () {
		currentTurn = Turn.PlayerTurn;


		GameObject[] tempPlayers = GameObject.FindGameObjectsWithTag("Player");
		allPlayers = new PlayerController[tempPlayers.Length];
		for(int i = 0; i < tempPlayers.Length; i++){
			allPlayers[i] = tempPlayers[i].GetComponent<PlayerController>();
		}

		GameObject[] tempEnemies = GameObject.FindGameObjectsWithTag("Enemy");
		allEnemies = new Enemy[tempEnemies.Length];
		for(int i = 0; i < tempEnemies.Length; i++){
			allEnemies[i] = tempEnemies[i].GetComponent<Enemy>();
		}
		//Debug.Log (allPlayers.Length);
		//Debug.Log (allEnemies.Length);
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log(currentTurn);
		if(currentTurn == Turn.PlayerTurn){
			foreach( PlayerController p in allPlayers){
				p.PlayerUpdate();
			}
		}
		else if(currentTurn == Turn.EnemyTurn){
			foreach( Enemy e in allEnemies){
				e.EnemyUpdate();
			}
		}



		if (Input.GetMouseButtonDown(0)) {
			
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if (Physics.Raycast(ray, out hit, 100)) {
				
				//Debug.Log(hit.collider.gameObject.GetComponent<PathTile>());
				if(hit.collider.gameObject.tag == "Tile")
				{
					if(selectedObject && selectedObject.GetComponent<PlayerController>() && !selectedObject.GetComponent<Character>().end)
					{
						//Debug.Log("here");
						selectedObject.GetComponent<Character>().end = hit.collider.gameObject.GetComponent<PathTile>();
						//Debug.Log(selectedObject.GetComponent<Character>().end);
					}
//					else if(selectedObject.GetComponent<Enemy>())
//					{
//						selectedObject.GetComponent<Enemy>().end = hit.collider.gameObject.GetComponent<PathTile>();
//					}
				}
				else if(hit.collider.gameObject.tag == "Player") //FUTURE REFERENCE, SELECTED PLAYER TAG
				{
					if(selectedObject)
					{
						selectedObject.GetComponent<MeshRenderer>().material.color = Color.white;
					}
					selectedObject = hit.collider.gameObject;
					selectedObject.GetComponent<MeshRenderer>().material.color = Color.green;
					//Debug.Log("Hit player");
				}
				else if(hit.collider.gameObject.tag == "Enemy")
				{
                    if (selectedObject) {
						selectedObject.GetComponent<MeshRenderer>().material.color = Color.white;
						if(selectedObject.GetComponent<PlayerController>())
						{
                        	selectedObject.GetComponent<Character>().basicAttack(hit.collider.gameObject.GetComponent<Enemy>());
						}
                    }
					selectedObject = hit.collider.gameObject;
					selectedObject.GetComponent<MeshRenderer>().material.color = Color.green;
					//Debug.Log("Hit enemy");
				}
			}
		}
		if(currentTurn == Turn.PlayerTurn)
		{
			if(Inactive(allPlayers))
			{
				currentTurn = Turn.EnemyTurn;
				foreach(PlayerController pc in allPlayers)
				{
					pc.GetComponent<PlayerController>().resetStatus();
				}
			}
		}
		else //Enemy Turn
		{
			//find closest player, find path to player and stop 1 tile before. Attack player.
			for(int i = 0; i < allEnemies.Length; i++){
				allEnemies[i].end = allEnemies[i].FindClosestPlayer(allPlayers).GetComponent<PlayerController>().start;
			}

			if(Inactive(allEnemies))
			{
				Debug.Log ("Here");
				currentTurn = Turn.PlayerTurn;
				foreach(Enemy e in allEnemies)
					e.active = true;
			}
		}
	}

	//checks if all of a team is inactive, to progress turns
	bool Inactive(Character[] characters)
	{
		foreach(Character c in characters)
		{
			if(currentTurn == Turn.PlayerTurn)
			{
				if(c.active)
				{
					return false;
				}
				else
				{
					continue;
				}
			}
			else
			{
				if(c.active)
				{
					return false;
				}
				else
				{
					continue;
				}
			}
		}
		return true;
	}
}