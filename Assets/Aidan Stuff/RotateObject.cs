using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour {

	public float RotationX = 0.0f;
	public float RotationY = 0.0f;
	public float RotationZ = 0.0f;
	public float speed = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(RotationX,RotationY,RotationZ), 360 * Time.deltaTime * speed);
	}
}
