﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour {

	public GameObject citizen;

	public float detectiveSkill;
	GameObject[] spawnPoints;
	public DossierDataClass dossier;
	public bool dossierActive;
	public static int population = 100;
	public Text dossierText;
	public GameObject arsonPrefab;
	public GameObject robberyPrefab;

	GameObject[] citizens = new GameObject[population];
	List<GameObject> communists = new List<GameObject> (); 
	GameObject[] buildings;
	GameObject[] possibleCrimes = new GameObject[2];
	float crimeCooldown = 120f;
	float cooldownRemaining = 0f;
	public Text score;
	public Vector2 scrollPosition;
	public Canvas dossierCanvas;

	public static int communistPower = 40;
	int defaultCommunistPower = 20;

	public static GameObject currentPerpetrator;
	public static CrimeDataClass currentCrime;

	int communistLimit;


	// TODO: put smarter code for determining how many communists there are at game start
	//For example, there can never be more than 10 or less than 2
	void Start() {
		dossierCanvas.enabled = false;
		dossierActive = false;
		dossier = ScriptableObject.CreateInstance<DossierDataClass> ();
		score.text = communistPower.ToString ();
		spawnPoints = GameObject.FindGameObjectsWithTag ("WayPoint");
		SetBuildings ();

		Debug.Log ("Spawning Citizens");

		for (int i = 0; i < population; i++) {
			int spawnPointChoice = Random.Range(0, 16);
			citizens[i] =(GameObject) Instantiate (citizen, spawnPoints[spawnPointChoice].transform.position, Quaternion.identity);

			possibleCrimes [0] = robberyPrefab;
			possibleCrimes [1] = arsonPrefab;
		}

		FindCommunists ();
		addPeopleToBuildings ();
		Debug.Log (communists.Count);
	}

	void Update(){
		PrintDossier ();
		//The communist limit is determined by the communist power divided by five, as can be seen here
		//This determines the real number of citizens who are also communists
		communistLimit = communistPower / 5;
		BalanceCommunists();

		HandleCrimes ();
		UpdateScore ();
	}

	void FindCommunists() {
		//Takes all the citizens, and finds the magnitude of their communist characteristic
		//If it is at or above five, it adds them to the communism group
		foreach (GameObject person in citizens) {
			AIScript script = person.GetComponent<AIScript> ();

			if (script.GetCommunism () >= 5) communists.Add (person);
		}
	}

	void BalanceCommunists(){
		//This method makes sure that the number of communists never exceeds the communist limit, and tries to add new communists when possible
		//the first if statement finds the current communist with the lowest communism characteristic, and removes it from the communist list
		//The second simply tries to add more communist to the list
		//This method activates every frame, so hopefully it won't affect the fps. If not we can optimise it later
		if (communists.Count > communistLimit) {
			int communismCheck = 100;
			GameObject deleteTarget = communists [0];
			foreach (GameObject person in communists) {
				AIScript script = person.GetComponent<AIScript> ();
				if (script.GetCommunism() < communismCheck) {
					deleteTarget = person;
					communismCheck = script.GetCommunism ();
				}
			}
			 communists.Remove (deleteTarget);
		}

		if (communists.Count < communistLimit) {
			FindCommunists ();
		}
	}

	void SetBuildings(){
		buildings = GameObject.FindGameObjectsWithTag ("Building");

	}

	void HandleCrimes(){
		cooldownRemaining -= Time.deltaTime;
		if (cooldownRemaining <= 0) {

			GameObject chosenCrime = possibleCrimes [Random.Range (0, possibleCrimes.Length)];
			GameObject newCrime = (GameObject) Instantiate (chosenCrime, new Vector3(0f,.2f,0f), Quaternion.identity);
			CrimeDataClass crimeData = newCrime.GetComponent<CrimeDataClass>();
			Debug.Log (crimeData);
			crimeData.SetData (chosenCrime.name, communists, communistPower, buildings);
			dossier.dossierCrimeEntries.Add (crimeData);
			SetBuildings ();
			communistPower += 10;
			cooldownRemaining = AdjustCrimeCooldown(crimeCooldown);
			currentCrime = crimeData;
		}
	}


	void UpdateScore() {
		if (communistPower != defaultCommunistPower) {
			score.text = communistPower.ToString ();
			Debug.Log (communistPower);
			defaultCommunistPower = communistPower;
			if (communistPower <= 0) {
				Debug.Log ("You Win! No more communists in Levittburg!");
				SceneManager.LoadScene ("WinningScene");

			}
			if (communistPower >= 100) {
				Debug.Log ("You lose! The radicals have overthrown Levittburg!");
				SceneManager.LoadScene ("LosingScene");
			}
		}
	}

	float AdjustCrimeCooldown(float currentCooldown) {
		float cooldown = currentCooldown * 50*50/communistPower/communistPower;
		return cooldown;
	}

	public void PrintDossier() {
		if (dossierActive) {	
			dossierText.text = dossier.GetDossierText ();
			dossierCanvas.enabled = true;
		} else {
			dossierText.text = "";
			dossierCanvas.enabled = false;
		}
	}


	void addPeopleToBuildings(){

		//we'll need to change this later to only add people to houses

		int counter = 0;
		for (int i = 0; i < citizens.Length; i++) {
			if (counter > buildings.Length)
				counter = 0;
			
			AIScript citScript = citizens[i].GetComponent<AIScript> ();
			BuildingScript buildScript = buildings[counter].GetComponent<BuildingScript> ();


			citScript.addHomeBuilding (buildings [counter]);
			buildScript.addCitizen (citizens [i]);
			counter++;
		}

	}


	public void ToggleDossier() {
		if (dossierActive != true) {
			dossierActive = true;
		} else {
			dossierActive = false;
		}
	}

}

