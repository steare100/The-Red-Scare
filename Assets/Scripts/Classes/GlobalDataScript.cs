using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalDataScript : MonoBehaviour {

	public static string[] firstNames  = {"Jeffrey", "Johnny", "Joan", "Johanna", "Jane", "Jack", "Janice", "Jacques", "Jeremy", "Jill", "Jesse", "Jonah", "Jim", "Jacob", "Jo", "June", "James", "Joseph"};
	public static string[] lastNames = {"Depp", "Djikstra", "Davidson", "Donner", "Dickens", "Doppler", "DeVito", "DuHast", "Djokovic"};

	public static Vector3[] wayPoints; 
	public static GameObject[] wayPointObjs = GameObject.FindGameObjectsWithTag("WayPoint");
	public static GameObject[] buildings;
	public static List<GameObject> possibleCrimes = new List<GameObject>();


	static void Awake() {
		for (int i = 0; i < wayPointObjs.Length; i++) {
			wayPoints [i] = wayPointObjs [i].transform.position;
		}
		possibleCrimes.Add (Resources.Load ("RobberyPrefab") as GameObject);
		possibleCrimes.Add (Resources.Load ("ArsonPrefab") as GameObject);
	}

	static void Start() {
		buildings = GameObject.FindGameObjectsWithTag ("Building");
	}


	//Hey Rig, I added some code here to account for each citizen's "home building", which I intended to be another waypoint
	//Of course, adding a waypoint like that, which is different for every citizen, would mean that these waypoints can't be class variables like they are now
	//I liked the idea you had going here, with the GlobalDataScript and class variables, so I didn't want to outright change it
	//Instead I made it so whenever MoveToWaypoint is called, it must accept another building , the home building
	//When waypoint choice iterates, it has a chance to return the next number above the length of the waypoint array
	//If that happens, the waypoint is set to the "homeBuilding"

	public static void MoveToWayPoint(GameObject citizen, GameObject homeBuilding) {
		int length = wayPointObjs.Length;
		int wayPointChoice = Random.Range (0, length);
		NavMeshAgent agent = citizen.GetComponent<NavMeshAgent> ();

		if (wayPointChoice == length){
			agent.SetDestination (homeBuilding.transform.position);
		} else {
			agent.SetDestination (wayPointObjs [wayPointChoice].transform.position);
		}

	}

	public static string GenerateName() {
		int firstNameChoice = Random.Range (0, firstNames.Length);
		int lastNameChoice = Random.Range (0, lastNames.Length);
		string name = firstNames[firstNameChoice] + " " + lastNames[lastNameChoice];
		return name;
	}
	public static bool GetRandomBool() {
		int boolNumber = Random.Range (0, 2);
		if (boolNumber == 0) {
			return true;
		} else {
			return false;
		}
	}

	public static TraitDataClass PickRandomTrait(AIScript perpScript) {
		int traitNumber = Random.Range (0, 4);
		TraitDataClass[] traits = { perpScript.coat, perpScript.glasses, perpScript.hat, perpScript.hair };
		TraitDataClass traitChoice = traits [traitNumber];
		return traitChoice;
	}
}
