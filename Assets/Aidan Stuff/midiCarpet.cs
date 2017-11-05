using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;

public class midiCarpet : MonoBehaviour {

	public int noteNumber;
	public float minimum;
	public float maximum;
	public float speed;

	float currentTime = 0f;
	float timeToMove = 0.5f;

	void Start () {
		
	}
	

	void Update () {

			if (currentTime <= timeToMove)
			{
				currentTime += Time.deltaTime;
			GetComponent<particleGrid> ().peakHeight = Mathf.Lerp(maximum, minimum, currentTime / timeToMove);


			}
			
//			GetComponent<particleGrid> ().peakHeight = Mathf.SmoothDamp (maximum, minimum, ref velocity, timeToMove);


		}

	void NoteOn(MidiChannel channel, int note, float velocity)
	{
		Debug.Log("NoteOn: " + channel + "," + note + "," + velocity);
		if (velocity > 0) {
			currentTime = 0f;
		}
			
	}

	void NoteOff(MidiChannel channel, int note)
	{
		Debug.Log("NoteOff: " + channel + "," + note);
	}

	void Knob(MidiChannel channel, int knobNumber, float knobValue)
	{
		Debug.Log("Knob: " + knobNumber + "," + knobValue);
	}

	void OnEnable()
	{
		MidiMaster.noteOnDelegate += NoteOn;
		MidiMaster.noteOffDelegate += NoteOff;
		MidiMaster.knobDelegate += Knob;
	}

	void OnDisable()
	{
		MidiMaster.noteOnDelegate -= NoteOn;
		MidiMaster.noteOffDelegate -= NoteOff;
		MidiMaster.knobDelegate -= Knob;
	}


}