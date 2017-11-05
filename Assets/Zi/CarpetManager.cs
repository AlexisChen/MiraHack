using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class CarpetManager : MonoBehaviour {

	public int _initialSize;
	public float _initialParticleScale;
	public float _spacing;
	public Transform prefab;
	public Transform transformTrigger;

	[Header("Testing")]
	public float peakHeight;
	public float dampening;
	public float multiplier = 0.01f;
	public float speed = 10.0f;
	public float phase = 1.0f;

	private Transform [,] m_spheres;
	private int _size;
	private float _width;

	// Use this for initialization
	void Start () {
		_size = _initialSize;
		m_spheres = new Transform[_size,_size];
		_width = _size * _spacing;

		Instantiate (transformTrigger,transform);

		float xpos = 0, zpos = 0;
		for (int x = 0; x < _size; x++)
		{
			for (int z = 0; z < _size; z++)
			{
				xpos = x; zpos = z;
				m_spheres [x, z] = Instantiate (prefab);
				Transform t = m_spheres [x, z];
				t.localScale = new Vector3 (1, 1, 1) * _initialParticleScale;
				t.localPosition = new Vector3 (xpos - _width / 2.0f + xpos * _spacing, 0, zpos - _width / 2.0f + zpos * _spacing);
				t.SetParent (transform);
			}
		}
	}

	bool onn = true;

	// Update is called once per frame
	void Update () {
		float time = Time.fixedTime;
		dampening = 1;//(1 - Mathf.Pow (Mathf.Min (time * 0.1f, 1), 2));
		//float xpos = 0, zpos = 0;
		for (int x = 0; x < _size; x++)//, xpos = x)
		{
			for (int z = 0; z < _size; z++)//, zpos = z)
			{
				Transform t = m_spheres [x, z];
				Vector3 pos = t.position;
				t.position = new Vector3(pos.x, pos.y + dampening * Mathf.Cos (time * Mathf.PI * 1.0f) * 1.0f, pos.z);
//					(pos.y == 0 ? 1 : pos.y) * Mathf.Sin ((Time.time * speed) + (phase) * multiplier)
				//m_spheres [x, z].localPosition = new Vector3 (xpos - _width / 2.0f + xpos * _spacing, 0, zpos - _width / 2.0f + zpos * _spacing);
			}
		}
	}
}
