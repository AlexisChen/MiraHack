using UnityEngine;
using System.Collections;

public class SizeFromVolume : MonoBehaviour
{

	public int qSamples = 1024;
	public float refValue = 0.1f;
	public float rmsValue;
	public float dbValue;
	public float Volume = 2;

	float[] samples;

	// Use this for initialization
	void Start()
	{
		samples = new float[qSamples];
	}

	void GetVolume()
	{



		GetComponent<AudioSource>().GetOutputData(samples, 0); // fill array with samples
		int i;
		float sum = 0f;
		for (i = 0; i < qSamples; i++)
		{
			sum += samples[i] * samples[i]; // sum squared samples
		}
		rmsValue = Mathf.Sqrt(sum / qSamples); // rms = square root of average
		dbValue = 20 * Mathf.Log10(rmsValue / refValue); // calculate dB
		if (dbValue < -160) dbValue = -160; // clamp it to -160dB min
	}

	// Update is called once per frame
	void Update()
	{
		GetVolume();
		Vector3 scale = transform.parent.gameObject.transform.localScale;
		transform.parent.gameObject.transform.localScale = new Vector3(Volume * rmsValue, Volume * rmsValue, Volume * rmsValue);
	}
}
