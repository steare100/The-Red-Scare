using UnityEngine;
using System.Collections;

public class TextRotationScript : MonoBehaviour {

	Camera cameraM = Camera.main;

	void Start() {
		cameraM = Camera.main;
	}

	void Update () {
		
		transform.LookAt (cameraM.gameObject.transform.position);
	}
}
