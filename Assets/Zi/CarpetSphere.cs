using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtensionMethods {
	public static float Map (this float value, float a1, float b1, float a2, float b2) {
	    return (value - a1) / (b1 - a1) * (b2 - a2) + a2;
	}
}

public class CarpetSphere : MonoBehaviour {
	private bool behaviorRunning;
	private Vector2Int _gridLocation;
	private float timeStart;

	// Frequency, Amplitude
	private Dictionary<float, float> fSin = new Dictionary<float, float>();
	private Dictionary<float, float> fCos = new Dictionary<float, float>();

	// Color, Amplitude
	private Dictionary<Color, float> cSin = new Dictionary<Color, float>();
	private Dictionary<Color, float> cCos = new Dictionary<Color, float>();
	private Dictionary<Color, float> cFreq = new Dictionary<Color, float>();

    private float[] _groupWeights;

	private float timeDecay = 0.9f;
	private float colorDecay = 0.75f;
	private float spaceDecay = 0.65f;

	private float TWOPI = 2 * Mathf.PI;
	private float freq = 1;

	private Color baseColor;

    public CarpetManager Manager { get; set; }

    // Use this for initialization
	void Start () {
		behaviorRunning = false;
		baseColor = Color.white;

		this.GetComponent<Renderer>().material.SetColor("_Color", baseColor);

        _groupWeights = new float[StemGroupManager.Instance.GroupCount];
	    CalculateGroupWeights();
	}

	public void SetGridLocation(Vector2Int x)
	{
		_gridLocation = x;
	}

	// Update is called once per frame
	void Update () {
		float time = Time.fixedTime - timeStart;

		// Change position
		Vector3 pos = transform.position;
		pos.y = 0.0f;
		foreach(float freq in fSin.Keys.ToList()) {
			pos.y += fSin[freq] * Mathf.Sin (TWOPI * freq * Time.fixedTime);
			fSin[freq] *= timeDecay;
		}
		foreach(float freq in fCos.Keys.ToList()) {
			pos.y	+= fCos[freq] * Mathf.Cos (TWOPI * freq * Time.fixedTime);
			fCos[freq] *= timeDecay;
		}

	    pos.y += Manager.GetSphereOffset(_groupWeights);

		transform.position = pos;

		// Change color
		Color notFinalColor = Color.black;
		foreach(Color c in cSin.Keys.ToList()) {
			float s = Mathf.Sin (TWOPI * cFreq[c] * Time.fixedTime).Map(-1, 1, 0, 1);
			notFinalColor += cSin[c] * s * c;
			cSin[c] *= colorDecay;
		}
		foreach(Color c in cCos.Keys.ToList()) {
			float s = Mathf.Cos (TWOPI * cFreq[c] * Time.fixedTime).Map(-1, 1, 0, 1);
			notFinalColor += cCos[c] * s * c;
			cCos[c] *= colorDecay;
		}
		notFinalColor.a = 1.0f;
		float weight = Mathf.Max(notFinalColor.r, Mathf.Max(notFinalColor.g, notFinalColor.b));
		Color finalColor = Color.Lerp(notFinalColor, baseColor, weight);
		this.GetComponent<Renderer>().material.SetColor("_Color", finalColor);
	}

	public void StartBehavior(ExpandingSphere sphere, float amp, float freq, Color c)
	{
		behaviorRunning = true;

		float time = Time.fixedTime;

		int ampFreq;
		float scaledAmp = amp * Mathf.Pow (spaceDecay, sphere.transform.localScale.x);

		float fSinDelta = scaledAmp * Mathf.Cos (- TWOPI * freq * time);
		float fCosDelta = scaledAmp * Mathf.Sin (- TWOPI * freq * time);
		float cSinDelta = scaledAmp * Mathf.Cos (- TWOPI * freq * time);
		float cCosDelta = scaledAmp * Mathf.Sin (- TWOPI * freq * time);

		float fSinVal;
		if (fSin.TryGetValue(freq, out fSinVal)) {
			fSin[freq] = fSinVal + fSinDelta;
		} else {
			fSin.Add(freq, fSinDelta);
		}

		float fCosVal;
		if (fCos.TryGetValue(freq, out fCosVal)) {
			fCos[freq] = fCosVal + fCosDelta;
		} else {
			fCos.Add(freq, fCosDelta);
		}

		float cSinVal;
		if (cSin.TryGetValue(c, out cSinVal)) {
			cSin[c] = cSinVal + cSinDelta;
		} else {
			cSin.Add(c, cSinDelta);
		}

		float cCosVal;
		if (cCos.TryGetValue(c, out cCosVal)) {
			cCos[c] = cCosVal + cCosDelta;
		} else {
			cCos.Add(c, cCosDelta);
		}

		cFreq[c] = freq;
	}

    private void CalculateGroupWeights() {
        // Drums: radial out from center
        var distFromCenter = Mathf.Clamp01(transform.localPosition.magnitude / Manager.Width * 1.5f);
        distFromCenter *= distFromCenter;
        _groupWeights[0] = 1 - distFromCenter;

        // Percussion
        _groupWeights[1] = 0;

        // Chords
        _groupWeights[2] = 1;//Mathf.Clamp01((transform.localPosition.x - 5) / 10);

        // Bass
        _groupWeights[3] = 0;
    }
}
