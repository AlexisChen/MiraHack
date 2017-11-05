using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class SphereTrigger : MonoBehaviour {
	public float maxRadius;
	public int areaOfAffect;
	public float _expandSpeed;

	[Header("Testing")]
	public bool run;

	private float _currentDistance;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (!run)
			return;
		float expand = _expandSpeed * Time.deltaTime;
		Vector3 cur = transform.localScale;
		Vector3 scale = new Vector3(cur.x + expand, cur.y + expand, cur.z + expand);
		if (scale.x > maxRadius)
			Destroy (gameObject);
		transform.localScale = scale;
	}

	void OnTriggerEnter(Collider other)
	{
		//perform a transform on to the thing
	}
}
