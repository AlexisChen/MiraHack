using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;


public class triggerClips : MonoBehaviour {
	
	public int rootNote = 0;

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
