using System.Collections;
using System.Collections.Generic;
using Reaktion;
using UnityEngine;

[RequireComponent(typeof(Reaktor))]

public class getAudioData : MonoBehaviour {

	public float Output;
	public float Peak;
	public float Gain;
	public Reaktor audio;

	[Header("Frame Storage")]

	public float[] frames;
	int countFrames = 1;

	void Start () {
		audio = gameObject.GetComponent<Reaktor> ();

	}
	
	// Update is called once per frame
	void Update () {
		Output = audio.Output;
		Peak = audio.Peak;
		Gain = audio.Gain;

		frames [0] = Output;

		if (countFrames < frames.Length) {
			frames [countFrames] = frames [countFrames - 1];
			countFrames++;
		} else {
			countFrames = 1;
		}


					
				
			


	}

}
