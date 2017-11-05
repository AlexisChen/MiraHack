using System.Collections;
using System.Collections.Generic;
using UnityEngine;
	


	public class getAudioInput : MonoBehaviour
	{
		AudioSource audioSource;

	public string deviceName;

		
		void Awake()
		{
			// Create an audio source.
			audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.loop = true;
			
			StartInput();
		}

		void OnApplicationPause(bool paused)
		{
			if (paused)
			{
				audioSource.Stop();
				Microphone.End(null);
				audioSource.clip = null;
			}
			else
				StartInput();
		}

		void StartInput()
		{
			var sampleRate = AudioSettings.outputSampleRate;

			// Create a clip which is assigned to the default microphone.
		audioSource.clip = Microphone.Start(deviceName, true, 1, sampleRate);

			if (audioSource.clip != null)
			{
				// Wait until the microphone gets initialized.
				int delay = 0;
				while (delay <= 0) delay = Microphone.GetPosition(null);

				// Start playing.
				audioSource.Play();

				// Estimate the latency.

			}
			else
				Debug.LogWarning("GenericAudioInput: Initialization failed.");
		}
	}

