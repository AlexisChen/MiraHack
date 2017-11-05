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

    private float[] _volumeCache;
    private int _lastVolumeCacheFrame = -1;

    public float Width {
        get { return _width; }
    }

    void Start() {
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

                var carpetSphere = t.GetComponent<CarpetSphere>();
			    carpetSphere.Manager = this;
                carpetSphere.SetGridLocation(new Vector2Int(x,z));

				t.localPosition = new Vector3 (xpos, 0 ,zpos);//GridToWorld ((float)x, (float)z);
				t.SetParent (transform);
				zpos += _spacing;
			}
			xpos += _spacing;
		}
	}

	void Update() {

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

    public float GetSphereOffset(float[] stemWeights) {
        float offset = 0;
        offset += stemWeights[0] * GetGroupVolume(0) * 5;
        offset += stemWeights[1] * GetGroupVolume(1) * 5;
        offset += stemWeights[2] * GetGroupVolume(2) * 5;
        offset += stemWeights[3] * GetGroupVolume(3) * 5;
        return offset;
    }

    public float GetGroupVolume(int group) {
        // Cache the volume for each group this frame
        if (_lastVolumeCacheFrame != Time.frameCount) {
            if (_volumeCache == null) {
                _volumeCache = new float[StemGroupManager.Instance.GroupCount];
            }

            for (int i = 0; i < StemGroupManager.Instance.GroupCount; ++i) {
                var playhead = StemGroupManager.Instance.GetGroupPlayhead(i);

                // Get the volume and let it jump up but damp it going down so it lasts longer
                var volume = playhead != null ? playhead.GetVolume() : 0;
                float damping;
                damping = volume > _volumeCache[i]
                    ? 0.1f // going up
                    : 0.5f;
                _volumeCache[i] = Mathf.Lerp(volume, _volumeCache[i], damping);
            }

            _lastVolumeCacheFrame = Time.frameCount;
        }

        return _volumeCache[group];
    }

	public Vector3 GridToWorld(float x, float z)
	{
		return new Vector3 (x * _spacing - _width / 2.0f, 0, z * _spacing - _width / 2.0f);
	}

	public Vector2 WorldToGrid(float x, float z)
	{
		return new Vector2((x + _width / 2.0f) / _spacing, (z + _width / 2.0f) / _spacing);
	}
}