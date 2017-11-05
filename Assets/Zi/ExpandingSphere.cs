using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class ExpandingSphere : MonoBehaviour {
	public float maxRadius;
	public int areaOfAffect;
	public float expandSpeed;

	[Header("Testing")]
	public bool run;

	private float amplitude;
	private float frequency;
	private Color color;
	public float startTime;

	public void Initialize(float _amplitude, float _frequency, Color _color)
	{
		amplitude = _amplitude;
		frequency = _frequency;
		color = _color;
	}

	// Use this for initialization
	void Start () {
		startTime = Time.fixedTime;
		//transform.localScale = Vector3.zero;
		Vector3 pos = transform.position;
		pos.y = 0.25f;
		transform.position = pos;
	}

	// Update is called once per frame
	void Update () {
		float expand = expandSpeed * Time.deltaTime;
		Vector3 cur = transform.localScale;
		Vector3 scale = new Vector3(cur.x + expand, cur.y + expand, cur.z + expand);
		if (scale.x > maxRadius)
			Destroy (gameObject);
		transform.localScale = scale;
	}

	void OnTriggerEnter(Collider other)
	{
		// If it isn't a carpet sphere, we don't care about it
		if(!other.name.Contains ("carpetSphere")) return;

		// Make the carpet move
		GameObject ball = other.gameObject;
		ball.GetComponent <CarpetSphere> ().StartBehavior (this, amplitude, frequency, color);

	}

}
