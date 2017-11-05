using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class CarpetManager : MonoBehaviour {

	public static float _spacing;
	public Transform prefab;
	public Transform transformTrigger;

	[Header("Initializing")]
	public int _initialSize;
	public float _initialParticleScale;

	[Header("Testing")]
	public float peakHeight;
	public static float dampening;
	public float multiplier = 0.01f;
	public float speed;
	public float phase = 1.0f;

	private Transform [,] m_spheres;
	private int _size;
	private static float _width;

	private int step = 0;

	// Use this for initialization
	void Start () {
		//prefab = Resources.Load("Resources/BassCarpet/carpetSphere") as Transform;
		//transformTrigger = Resources.Load("Resources/BassCarpet/transformTrigger") as Transform;

		_spacing = 0.25f;
		_size = _initialSize;
		m_spheres = new Transform[_size,_size];
		_width =  ((float)_size) * _spacing;

		float x1 = 30;
		float x2 = 10;
		float zz = 20;

		for (int x = 0; x < _size; x++)
		{
			for (int z = 0; z < _size; z++)
			{
				m_spheres [x, z] = Instantiate (prefab) as Transform;
				Transform t = m_spheres [x, z];
				t.localScale = new Vector3 (1, 1, 1) * _initialParticleScale;
				t.GetComponent <CarpetSphere>().SetGridLocation(new Vector2Int(x,z));
				t.localPosition = GridToWorld ((float)x, (float)z);
				t.SetParent (transform);
			}
		}
	}

	// Update is called once per frame
	void Update () {

		/* TEMPORARY CODE */
		if (step == 3) {
			HandleEvent(30, 10, 1f, 1f, Color.red);
			HandleEvent(10, 30, 1f, 4f, Color.green);
		}
		step ++;

		float time = Time.fixedTime;
		for (int x = 0; x < _size; x++)//, xpos = x)
		{
			for (int z = 0; z < _size; z++)//, zpos = z)
			{
				Transform t = m_spheres [x, z];
				Vector3 pos = t.position;
			}
		}
	}

	public void HandleEvent(float x, float z, float amp, float freq, Color color)
	{
		Transform sphere1 = Instantiate (transformTrigger, GridToWorld (x, z), Quaternion.identity);
		sphere1.GetComponent<ExpandingSphere> ().Initialize(amp, freq, color);
	}

	public static Vector3 GridToWorld(float x, float z)
	{
		return new Vector3 (x * _spacing - _width / 2.0f, 0, z * _spacing - _width / 2.0f);
	}

	public static Vector2 WorldToGrid(float x, float z)
	{
		return new Vector2((x + _width / 2.0f) / _spacing, (z + _width / 2.0f) / _spacing);
	}
}