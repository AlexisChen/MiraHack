using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarpetSphere : MonoBehaviour {
	private bool behaviorRunning;
	private Vector2Int _gridLocation;
	private float timeStart;
	private float aSin;
	private float aCos;
	private float startAmp = 10.0f;
	private float timeDecay = 0.98f;
	private float spaceDecay = 0.98f;

	private float TWOPI = 2 * Mathf.PI;
	private float freq = 1;

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

		// Change position
		Vector3 pos = transform.position;
		pos.y = 0.0f;
		pos.y += aSin * Mathf.Sin (TWOPI * freq * Time.fixedTime);
		pos.y	+= aCos * Mathf.Cos (TWOPI * freq * Time.fixedTime);
		transform.position = pos;

		// Do time decay
		aSin *= timeDecay;
		aCos *= timeDecay;
	}

	public void StartBehavior(float c)
	{
		behaviorRunning = true;
		timeStart = Time.fixedTime;

		float cTemp = startAmp * Mathf.Pow (spaceDecay, c);
		aSin += startAmp * Mathf.Cos (- TWOPI * freq * timeStart);
		aCos += startAmp * Mathf.Sin (- TWOPI * freq * timeStart);
	}
}
