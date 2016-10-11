using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingScript : MonoBehaviour {

	public string buildingType;
	string buildName;
	List<GameObject> owners = new List<GameObject> ();


	List<GameObject> peopleInside = new List<GameObject> ();



	//we need to fill some crimes into here
	List<CrimeDataClass> lvlOneCrimes;
	List<CrimeDataClass> lvlTwoCrimes;

	private bool guiActive;

	private Ray ray;
	private RaycastHit hit;




	// Use this for initialization
	void Start () {
		buildName = gameObject.name;
		/*string subName = name.Substring(0,(name.Length-4));
		if (subName == "Post_Office") {
			buildingType = "Post_Office";
		} */


	}
	
	// Update is called once per frame
	void Update () {

	}


	void OnGUI(){
		if (guiActive == false) {
			if (Input.GetMouseButtonDown (1) == true) {
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				Physics.Raycast (ray, out hit);

				if (hit.collider.gameObject == gameObject || hit.collider.gameObject.transform.parent.gameObject == gameObject) {
					Debug.Log (2);
					guiActive = true;

				}
			}
		}

		if (guiActive == true){

			GUIStyle whiteBackground = new GUIStyle();

			Texture2D texmex = MakeTex (100, 100, Color.white);
			whiteBackground.normal.background = texmex;

			//Vector3 pos = gameObject.transform.position;

			//Vector3 boxLocation = Camera.main.WorldToScreenPoint(gameObject.transform.position);

			int elements = 0;
			elements += owners.Count;
			elements += peopleInside.Count;

			float boxY = 300f + (elements * 10f);
			float boxX = 300f;

			GUILayout.BeginArea (new Rect(boxX, boxY, 100, 100),"Test",whiteBackground );

			GUILayout.EndArea ();
		}
	}



	public void addCitizen(GameObject citizen){
		owners.Add (citizen);

	}


	public void addPersonInside(GameObject citizen){
		peopleInside.Add (citizen);
	}

	public void removePersonInside(GameObject citizen){
		if(peopleInside.Contains(citizen)) peopleInside.Remove(citizen);

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
