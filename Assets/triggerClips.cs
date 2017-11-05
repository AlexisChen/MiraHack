using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MidiJack;


public class triggerClips : Photon.PunBehaviour {

	Vector3 leftTop; 
	float width; 
	float height;
	public GameObject carp;


	[System.Serializable]
	public class MyIntEvent : UnityEvent<int>
	{
	}

	// private bool colorToggle;
	public int rootNote = 0;
	// public GameObject bassCarpet;

	void Awake()
	{
		PhotonNetwork.autoJoinLobby = false;
		//might not be what we want 
		PhotonNetwork.automaticallySyncScene = true;
		Connect();
	}

	public void Connect()
	{
		if (PhotonNetwork.connected) {
			PhotonNetwork.JoinRandomRoom ();
		} else {
			PhotonNetwork.ConnectUsingSettings ("1.0");
		}
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log ("DemoAnimator/Launcher: OnConnectedToMaster() was called by PUN");
		PhotonNetwork.JoinRandomRoom ();
	}

	public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
	{
		PhotonNetwork.CreateRoom (null, new RoomOptions (){ MaxPlayers =4}, null);
	}

	void Start () {

	}

	void Update () {



	}

		void NoteOn(MidiChannel channel, int note, float velocity)
		{



		if (velocity > 0) {
			for (int clipIndex = 0; clipIndex < 4; clipIndex++){
				
				if (note == clipIndex) {
					//PhotonView photonView = PhotonView.Get(this);
					this.photonView.RPC("triggerClip", PhotonTargets.All, (int) channel, clipIndex);
//					triggerClip ((int) channel, clipIndex);
				}
			}

			}

		}

	[PunRPC]
		void triggerClip(int group, int clipIndex){
		int index = 4 * group + clipIndex;

		//
			float frequency  = 0.25f + index* (4-0.25f)/16;
		Color color = Color.Lerp( Color.white, Color.blue ,  0.1f+index*0.9f/16);
//		Vector3 coord =  Vector3 (width * group, 0, height * clipIndex);






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
