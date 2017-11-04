using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpDown : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		transform.position = new Vector3 (transform.position.x, Mathf.Sin(Time.time * Random.Range(500.0f, 800.0f))/8, transform.position.z);

	}
}
