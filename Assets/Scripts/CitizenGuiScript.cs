using UnityEngine;
using System.Collections;

public class CitizenGuiScript : MonoBehaviour {


	private bool guiActive = false;

	private Ray ray;
	private RaycastHit hit;
	//private GameObject citizen;

	// Use this for initialization
	void start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		 
	}

	/*void OnGui(){
		if (Input.GetMouseButtonDown (1) == true) {
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Physics.Raycast (ray, out hit);
			if (hit.collider == citizen) {
				guiActive = true;

			}
		}
		if (guiActive = true){
			GUIStyle whiteBackground = new GUIStyle();
			whiteBackground.normal.background = Color.white;

			Vector3 boxLocation = Camera.main.WorldToScreenPoint(citizen.transform.position);
			GUILayout.BeginArea (new Rect(boxLocation.x, boxLocation.y, 100, 100),"Test",whiteBackground );
			GUILayout.EndArea();

		}
	}*/


}
