using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class ExpandingSphere : MonoBehaviour {
	public float maxRadius;
	public int areaOfAffect;
	public float _expandSpeed;

	[Header("Testing")]
	public bool run;

	//private int _carpetSize;
	private float _currentDistance;
	private Vector2 _gridLocation;

	public void Initialize(int carpetSize, Vector2 gridLocation)
	{
		//initialize position, functionality of the sphere, etc.
		//_carpetSize = carpetSize;
		_gridLocation = gridLocation;
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		float expand = _expandSpeed * Time.deltaTime;
		Vector3 cur = transform.localScale;
		Vector3 scale = new Vector3(cur.x + expand, cur.y + expand, cur.z + expand);
		//if (scale.x > maxRadius)
			//Destroy (gameObject);
		transform.localScale = scale;
	}

	void OnTriggerEnter(Collider other)
	{
		// If it isn't a carpet sphere, we don't care about it
		if(!other.name.Contains ("carpetSphere")) return;

		//perform a transform on to the thing
		//find the layer that we're on from the center
		GameObject ball = other.gameObject;
		//Vector3 pos = ball.transform.position;
		//Vector2Int layer = CarpetManager.WorldToGrid (pos) - _gridLocation;
		//int layerNum = Mathf.Max (Mathf.Abs (layer.x), Mathf.Abs (layer.y));
		ball.GetComponent <CarpetSphere> ().StartBehavior (transform.localScale.x);
		//Calculate the value to pass in

		//Pass in the value to the ball

	}

}
