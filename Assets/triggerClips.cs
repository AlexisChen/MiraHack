﻿using UnityEngine;
using UnityEngine.Events;
using MidiJack;

public class triggerClips : Photon.PunBehaviour {
    float width = 40;
    float height = 40;
    public GameObject carp;

    [System.Serializable]
    public class MyIntEvent : UnityEvent<int> {
    }

    // private bool colorToggle;
    public int rootNote = 0;
    // public GameObject bassCarpet;

    void Awake() {
        PhotonNetwork.autoJoinLobby = false;
        //might not be what we want 
        PhotonNetwork.automaticallySyncScene = true;
        Connect();
    }

    public void Connect() {
        if (PhotonNetwork.connected) {
            PhotonNetwork.JoinRandomRoom();
        } else {
            PhotonNetwork.ConnectUsingSettings("1.0");
        }
    }

    public override void OnConnectedToMaster() {
        Debug.Log("DemoAnimator/Launcher: OnConnectedToMaster() was called by PUN");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg) {
        PhotonNetwork.CreateRoom(null, new RoomOptions() {MaxPlayers = 4}, null);
    }

    void Start() {
    }

    void Update() {
    }

    void NoteOn(MidiChannel channel, int note, float velocity) {
        if (velocity > 0) {
            for (int clipIndex = 0; clipIndex < 4; clipIndex++) {
                if (note == clipIndex) {
                    //PhotonView photonView = PhotonView.Get(this);
                    this.photonView.RPC("triggerClip", PhotonTargets.All, (int) channel, clipIndex);
//					triggerClip ((int) channel, clipIndex);
                }
            }
        }
    }

    [PunRPC]
    void triggerClip(int group, int clipIndex) {
        int index = 4 * group + clipIndex;

        //
        float frequency = 0.25f + index * (4 - 0.25f) / 16;
        Color color = Color.white;
        switch (group) {
            case 1:
                color = Color.Lerp(Color.red, Color.yellow, (clipIndex - 1) / 4.0f);
                break;
            case 2:
                color = Color.Lerp(Color.yellow, Color.green, (clipIndex - 1) / 4.0f);
                break;
            case 3:
                color = Color.Lerp(Color.green, Color.blue, (clipIndex - 1) / 4.0f);
                break;
            case 4:
                color = Color.Lerp(Color.blue, Color.red, (clipIndex - 1) / 4.0f);
                break;

            default:
                color = Color.white;
                break;
        }
//		Vector3 coord =  Vector3 (, 0, height * clipIndex);

        CarpetManager other = (CarpetManager) carp.GetComponent(typeof(CarpetManager));
        other.HandleEvent(10 * group - 5, 10 * clipIndex + 5, 1, frequency, color);

        StemGroupManager.Instance.SetGroupClip(group - 1, clipIndex);


        Debug.Log("Clip " + clipIndex + " in Group " + group + " has been triggered");


        // bassCarpet.GetComponent<CarpetManager> ().HandleEvent (0.0f,0.0f);
    }


    void OnEnable() {
        MidiMaster.noteOnDelegate += NoteOn;
    }

    void OnDisable() {
        MidiMaster.noteOnDelegate -= NoteOn;
    }
}