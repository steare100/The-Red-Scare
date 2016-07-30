using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AIScript : MonoBehaviour {

	Animator anim;

	NavMeshAgent agent;
	public GameObject gameManager;
	//I made this into a list to make it more flexible, in that we can add more waypoints (and have the amount of waypoints vary)
	List<GameObject> WayPoints = new List<GameObject>();


	int communism = Random.Range (0, 10);
	int honesty = Random.Range (0, 10);
	int violence = Random.Range (0, 10);


	//state variables
	bool isSelected = false;
	bool isAttacking = false;
	bool isFleeing = false;
	bool commitingCrime = false;
	bool inBuilding = false;



	CrimeSceneScript currentCrimeScript;

	//player unit
	public GameObject detective;

	GameObject homeBuilding;

	//Is it possible to add these waypoints in the editor to gameobjects that are being instantiated?
	//for now, I changed these to private, and found the waypoints by name
	//probably want to change this later

	GameObject WayPoint1;
	GameObject WayPoint2; 
	GameObject WayPoint3; 
	GameObject WayPoint4;

	public float walkSpeed;
	public float runSpeed;

	//Reference for pathfinding 
	int wayPointChoice;

	List <int> crimesCommitted = new List<int>();
	List <int> crimesWitnessed; 

	SkinnedMeshRenderer civRenderer;
	CapsuleCollider civCollider;

	float buildingInsideCooldown;

	float cooldownRemaining = 0f;

	private bool guiActive;

	private Ray ray;
	private RaycastHit hit;


	void Start() {

		guiActive = false;

		civRenderer = gameObject.transform.GetChild (0).GetComponent<SkinnedMeshRenderer>();
		civCollider = gameObject.GetComponent<CapsuleCollider> ();
		
		anim = GetComponent<Animator> ();

		WayPoints.Add(WayPoint1 = GameObject.Find("WayPoint1"));
		WayPoints.Add(WayPoint2 =  GameObject.Find("WayPoint2"));
		WayPoints.Add(WayPoint3 =  GameObject.Find("WayPoint3"));
		WayPoints.Add(WayPoint4 = GameObject.Find("WayPoint4"));
		WayPoints.Add (homeBuilding);
		gameObject.tag = "Citizen";


		//We're gonna want to change this to a list, to make it more flexible
		//WayPoints = new GameObject[4]{WayPoint1, WayPoint2, WayPoint3, WayPoint4};

		agent = GetComponent<NavMeshAgent> ();
		agent.speed = walkSpeed;

		startAtHouse ();

		// TODO: put smarter code for determining how many communists there are at game start
		//For example, there can nenver be more than 10 or less than 2

		//changed name from choices to actual trait names




		int wayPointChoice = Random.Range (0, WayPoints.Count);



		//Commented out the boolean aspects of the ai traits

		/*
		if (choice1 == 0) {
			isCommunist = true;
			gameObject.tag = "Communist";
		} 

		if (choice2 == 0) {
			isHonest = true;
		}

		if (choice3 == 0) {
			isViolent = true;
		} */

		agent.SetDestination(WayPoints[wayPointChoice].transform.position);
	}



	void Update() {

		cooldownRemaining -= Time.deltaTime;

		float rowSpeed = Mathf.Abs (agent.velocity.x);
		float collumnSpeed = Mathf.Abs (agent.velocity.z);
		float speed = GetHypotenuse (rowSpeed, collumnSpeed);
		anim.SetFloat ("Speed", speed/2);



		if (isAttacking) {
			Attack ();
		} else if (isFleeing) {
			Escape ();
			isFleeing = false;
		} else if (isSelected) {
			
		} else if (commitingCrime) {
			if (agent.remainingDistance <= 1f) {
				commitingCrime = false;
				currentCrimeScript.crimeActive ();
				currentCrimeScript = null;
			}

		}else if(inBuilding == true){
			if (cooldownRemaining <= 0)
				exitBuilding ();
			
		} else {
			if (agent.remainingDistance <= 1f || agent.destination == null) {
				if (WayPoints [wayPointChoice].tag == "Building") {
					enterBuilding ();
					wayPointChoice = Random.Range (0, WayPoints.Count);
					agent.SetDestination (WayPoints [wayPointChoice].transform.position);
					if (inBuilding == false) {
						agent.speed = walkSpeed;
					}
				} else {
					
					wayPointChoice = Random.Range (0, WayPoints.Count);
					agent.SetDestination (WayPoints [wayPointChoice].transform.position);
						agent.speed = walkSpeed;

				}


			} else {
				agent.Resume ();
			}
		}
	}

	void OnGUI(){

		//Have you heard of this flexible space thing? This is what I'm using to make the gui for the npc's for now
		//It's easy to set up, but isn't as flexible as normal gui
		//Basically you just define an area size, and put buttons and stuff into it
		//The rest it does for you

		if (guiActive == false) {
			if (Input.GetMouseButtonDown (1) == true) {
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				Physics.Raycast (ray, out hit);
				if (hit.collider.gameObject == gameObject) {
					
					guiActive = true;

				}
			}
		}
		if (guiActive == true){
			
			GUIStyle whiteBackground = new GUIStyle();

			Texture2D texmex = MakeTex (100, 100, Color.white);
			whiteBackground.normal.background = texmex;

			Vector3 pos = gameObject.transform.position;

			Vector3 boxLocation = Camera.main.WorldToScreenPoint(gameObject.transform.position);
			//this inverts the y of the ui box, which is necesary to make it track the npc
			float boxY = (Screen.height - boxLocation.y) -  50;
			float boxX = boxLocation.x + 10;

			GUILayout.BeginArea (new Rect(boxX, boxY, 100, 100),"Test",whiteBackground );

			GUILayout.BeginVertical ();

			GUILayout.FlexibleSpace ();

			GUILayout.BeginHorizontal ();

			if(GUILayout.Button("Interrogate",GUILayout.Width(40))){
				
			}

			if(GUILayout.Button("Arrest",GUILayout.Width(40))){
				turnInvisible ();
			}
				
			GUILayout.EndHorizontal ();


			if(GUILayout.Button("Close")){
				guiActive = false;
			}

			GUILayout.EndVertical ();
			GUILayout.EndArea();
			//RectTransform g = new RectTransform ();


		}
	}

	void OnMouseDown() {
		if (isSelected) {
			isSelected = false;
		} else {
			isSelected = true;
		//	AnswerQuestion ();
		}
	}

	public int getCommunism(){
		return communism;
	}

	//I commented this out as well, just to avoid the errors I'd get from commenting out the earlier bools
	/*
	void AnswerQuestion() {
		if (isCommunist) {
			if (isHonest) {
				if (isViolent) {
					Attack ();
				} else {
					Escape ();
				}
			} else {
				if (isViolent) {
					AccuseInnocent ();
				} else {
					Deny ();
				}

			}
		} else {
			if (isViolent) {
				if (isHonest) {
					AccuseGuilty ();
				} else {
					AccuseInnocent ();
				}
			} else {
				if (isHonest) {
					Deny ();
				} else {
					Escape ();
				}
			}
		}
	}


	*/
	void Attack() {
		isAttacking = true;
		agent.SetDestination (detective.transform.position);
	}

	void Confess() {
		Debug.Log (gameObject.name + " has confessed his Communist nature");
	}

	/*
	void AccuseInnocent() {
		GameObject accused = gameManager.GetComponent<GameManager> ().AI [4];
		Debug.Log (gameObject.name + " has accused " + accused.name + " of Communism");
	}


	void AccuseGuilty() {
		GameObject accused = gameManager.GetComponent<GameManager> ().AI [3];
		Debug.Log (gameObject.name + " has accused " + accused.name + " of Communism");
	}

	void Deny() {
		Debug.Log (gameObject.name + " denies any relation to the Communist party");
		agent.Resume ();
	}
*/

	void Escape() {
		Debug.Log (gameObject.name + " is attempting to fleeeeeeee");
		agent.Resume ();
		agent.speed = walkSpeed;
		isFleeing = true;
	}

	public float GetHypotenuse(float x, float z) {
		float hyp = Mathf.Sqrt (x * x + z * z);
		return hyp;
	}

	public void commitCrime(Vector3 location, CrimeSceneScript crimeScript){
		agent.SetDestination (location);

		if (inBuilding)
			exitBuilding();
			
		commitingCrime = true;
		currentCrimeScript = crimeScript;
		crimesCommitted.Add (crimeScript.getCrimeNumber ());
	}

	public void turnInvisible(){
		civCollider.enabled = false;
		civRenderer.enabled = false;
	}

	public void turnVisible(){
		civCollider.enabled = true;
		civRenderer.enabled = true;
	}
		


	public void enterBuilding(){
		/*Vector3 buildingPos = building.transform.position;
		agent.SetDestination (building.transform.position);
		if (agent.remainingDistance <= 1f) {*/
			
		//add some building entering animation here

		buildingInsideCooldown = Random.Range (0f, 10f);
		cooldownRemaining = buildingInsideCooldown;


		inBuilding = true;
		turnInvisible ();

		agent.speed = 0f;

		guiActive = false;

			
	}


	public void exitBuilding(){
		inBuilding = false;
		turnVisible ();

		agent.speed = walkSpeed;
	}
		

	public void addHomeBuilding(GameObject building){
		homeBuilding = building;
	}

	void startAtHouse(){
		transform.position = homeBuilding.transform.position;
		enterBuilding ();

	}
	private Texture2D MakeTex(int width, int height, Color col)
	{
		Color[] pix = new Color[width*height];

		for(int i = 0; i < pix.Length; i++)
			pix[i] = col;

		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();

		return result;
	}

}
