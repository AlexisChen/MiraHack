using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;

public class midiTrigger : MonoBehaviour {



	void Start () {

	}


	void Update () {

	}

	void NoteOn(MidiChannel channel, int note, float velocity)
	{
		switch (note) {

		case 60:
			Debug.Log ("Group 1");
			break;
		case 61:
			Debug.Log ("Group 2");
			break;
		case 62:
			Debug.Log ("Group 3");
			break;
		case 63:
			Debug.Log ("Group 4");
			break;

		}

	}

	void NoteOff(MidiChannel channel, int note)
	{

	}

	void Knob(MidiChannel channel, int knobNumber, float knobValue)
	{
		
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