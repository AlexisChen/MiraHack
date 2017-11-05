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
	public float speed = 10.0f;
	public float phase = 1.0f;

	private Transform [,] m_spheres;
	private int _size;
	private static float _width;

	// Use this for initialization
	void Start () {
		//prefab = Resources.Load("Resources/BassCarpet/carpetSphere") as Transform;
		//transformTrigger = Resources.Load("Resources/BassCarpet/transformTrigger") as Transform;
		_spacing = 5;
		_size = _initialSize;
		m_spheres = new Transform[_size,_size];
		_width =  ((float)_size) * _spacing;

<<<<<<< HEAD
		//Instantiate (transformTrigger,transform);
=======
		float x1 = 20;
		float x2 = 10;
		float zzz = 15;

		Transform sphere1 = Instantiate (transformTrigger, GridToWorld (x1, zzz), Quaternion.identity);
		Transform sphere2 = Instantiate (transformTrigger, GridToWorld (x2, zzz), Quaternion.identity);
>>>>>>> aea595d7f36c01242b2ed1ccddffe320f03359ac

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
		float time = Time.fixedTime;
		dampening = 1;//(1 - Mathf.Pow (Mathf.Min (time * 0.1f, 1), 2));
		//float xpos = 0, zpos = 0;
		for (int x = 0; x < _size; x++)//, xpos = x)
		{
			for (int z = 0; z < _size; z++)//, zpos = z)
			{
				Transform t = m_spheres [x, z];
				Vector3 pos = t.position;
				//t.position = new Vector3(pos.x, pos.y + (1- Mathf.Pow( Mathf.Min(time*0.1f, 1), 2))*Mathf.Cos (time * Mathf.PI * 1.0f)*1.0f, pos.z);
				//m_spheres [x, z].localPosition = new Vector3 (xpos - _width / 2.0f + xpos * _spacing, 0, zpos - _width / 2.0f + zpos * _spacing);
			}
		}
	}

	public void HandleEvent(float frequency, float amplitude)
	{
<<<<<<< HEAD
		int x = Mathf.RoundToInt(_width/2.0f), z = Mathf.RoundToInt(_width/2.0f);

		Transform sphere = Instantiate (transformTrigger, GridToWorld (x, z), Quaternion.identity);
=======
		float x1 = -5;
		float x2 = 5;
		float z = 0;
		//Transform sphere1 = Instantiate (transformTrigger, GridToWorld (x1, z), Quaternion.identity);
		//Transform sphere2 = Instantiate (transformTrigger, GridToWorld (x2, z), Quaternion.identity);
>>>>>>> aea595d7f36c01242b2ed1ccddffe320f03359ac
		//generate the relevant information that the sphere needs
		//figure out where we want the center to be
		//sphere1.GetComponent<ExpandingSphere> ().Initialize(_size, new Vector2 (x1, z));
		//sphere2.GetComponent<ExpandingSphere> ().Initialize(_size, new Vector2 (x2, z));
	}

	public static Vector3 GridToWorld(float x, float z)
	{
		return new Vector3 (x - _width / 2.0f + x * _spacing, 0, z - _width / 2.0f + z * _spacing);
	}

	public static Vector2Int WorldToGrid(Vector3 p)
	{
		float x = p.x, z = p.z;
		return new Vector2Int (Mathf.RoundToInt((x + _width / 2.0f) / (1 + _spacing)),
			Mathf.RoundToInt((z + _width / 2.0f) / (1 + _spacing)));
	}
}
