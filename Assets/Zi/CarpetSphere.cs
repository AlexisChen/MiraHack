using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarpetSphere : MonoBehaviour {
	private bool behaviorRunning;
	private Vector2Int _gridLocation;
	private float timeStart;
	// Use this for initialization
	void Start () {
		behaviorRunning = false;
		//_gridLocation = CarpetManager.WorldToGrid (transform.position);
	}

	public void SetGridLocation(Vector2Int x)
	{
		_gridLocation = x;
	}
	
	// Update is called once per frame
	void Update () {
		if (!behaviorRunning)
			return;
		float time = Time.fixedTime - timeStart;
		Vector3 pos = transform.position;
		pos.y += (1 - Mathf.Pow(Mathf.Min(time * 0.1f, 1),2)) * Mathf.Cos (time * Mathf.PI * 1.0f) * 1.0f;
		transform.position = pos;
		//
	}

	public void StartBehavior(int layerNum)
	{
		behaviorRunning = true;
		timeStart = Time.fixedTime;

	}
}
