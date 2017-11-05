using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class CarpetManager : MonoBehaviour {

	public static float _spacing;
	public Transform prefab;
	public Transform transformTrigger;

	[Header("Initializing")]
	private float _initialSize;
	private float _initialParticleScale;

	[Header("Testing")]
	public float peakHeight;
	public static float dampening;
	public float multiplier = 0.01f;
	public float speed;
	public float phase = 1.0f;

	private Transform [,] m_spheres;
	private int _size = 40;
	private const float _width = 10f;

	private int step = 0;

	// Use this for initialization
	void Start () {
		//prefab = Resources.Load("Resources/BassCarpet/carpetSphere") as Transform;
		//transformTrigger = Resources.Load("Resources/BassCarpet/transformTrigger") as Transform;
		//This is all in units of milimeters.
		_initialParticleScale = 2500f;
		float radius = _initialParticleScale;// diameter = _initialParticleScale * 2;
		_spacing = 0.25f;
		//_initialSize = (_width / radius + _spacing) / (_spacing + 2);

		//_size = (int)_initialSize;
		m_spheres = new Transform[_size,_size];

		//start at r-500 add 5r each time, end at 500-r
		float xpos = -_width / 2.0f;
		//float zpos = radius - _width / 2.0f;
		for (int x = 0; x < _size; x++)
		{
			float zpos = -_width / 2.0f;
			for (int z = 0; z < _size; z++)
			{
				m_spheres [x, z] = Instantiate (prefab) as Transform;
				Transform t = m_spheres [x, z];
				t.localScale = new Vector3 (1, 1, 1) * _initialParticleScale;
				t.GetComponent <CarpetSphere>().SetGridLocation(new Vector2Int(x,z));
				t.localPosition = new Vector3 (xpos, 0 ,zpos);//GridToWorld ((float)x, (float)z);
				t.SetParent (transform);
				zpos += _spacing;
			}
			xpos += _spacing;
		}
	}

	// Update is called once per frame
	void Update () {

		/* TEMPORARY CODE */
		if (step == 100) {
			HandleEvent(30, 10, 1f, 1f, Color.red);
			HandleEvent(10, 30, 1f, 4f, Color.green);
		}
		step ++;
	}

	public void HandleEvent(float x, float z, float amp, float freq, Color color)
	{
		Transform sphere1 = Instantiate (transformTrigger, transform);
		sphere1.localPosition = GridToWorld (x, z);
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