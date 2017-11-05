using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MidiJack;



public class triggerClips : MonoBehaviour {

	[System.Serializable]
	public class MyIntEvent : UnityEvent<int>
	{
	}

	// private bool colorToggle;
	public int rootNote = 0;
	// public GameObject bassCarpet;

	void Start () {

	}

	void Update () {



	}

		void NoteOn(MidiChannel channel, int note, float velocity)
		{

		if (velocity > 0) {

			for (int clipIndex = 0; clipIndex < 4; clipIndex++){

				if (note == clipIndex) {
					triggerClip ((int) channel, clipIndex);
				}
			}

			}

		}

		void triggerClip(int group, int clipIndex){

		Debug.Log ("Clip "+clipIndex+" in Group "+group+" has been triggered");


		// bassCarpet.GetComponent<CarpetManager> ().HandleEvent (0.0f,0.0f);


		}


		void OnEnable()
		{
			MidiMaster.noteOnDelegate += NoteOn;
		}

		void OnDisable()
		{
			MidiMaster.noteOnDelegate -= NoteOn;
		}



}
