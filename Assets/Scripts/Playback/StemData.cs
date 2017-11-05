using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stem", menuName = "Mercive/Stem", order = 101)]
public sealed class StemData : ScriptableObject {
    private const int BufferSize = 1600;

    [SerializeField]
    private AudioClip _clip;

    [SerializeField]
    private bool _autoplay = true;

    [SerializeField]
    private bool _loop;

    [SerializeField]
    private BeatTime[] _noteTimes;

    [SerializeField]
    private float[] _rmsData;
    [SerializeField]
    private int _rmsSampleInterval;

    public AudioClip Clip {
        get { return _clip; }
    }

    public bool Autoplay {
        get { return _autoplay; }
    }

    public bool Loop {
        get { return _loop; }
    }

    public BeatTime[] NoteTimes {
        get { return _noteTimes; }
    }

    public float[] RmsData {
        get { return _rmsData; }
    }

    public int RmsSampleInterval {
        get { return _rmsSampleInterval; }
    }

    public void LoadRMS() {
        if (_clip == null) {
            Debug.LogError("[StemData] Clip is not set.");
            return;
        }

        float[] samples = new float[_clip.samples * _clip.channels];
        _clip.GetData(samples, 0);

        int offset = 0;
        List<float> rmsList = new List<float>();
        while (offset < samples.Length) {
            float sum = 0.0f;
            for (int i = 0; i < BufferSize; i++) {
                float b = (offset < samples.Length) ? samples[offset] : 0.0f;
                sum += b * b;
                offset++;
            }
            rmsList.Add(Mathf.Sqrt(sum / BufferSize));
        }

        _rmsData = rmsList.ToArray();
        _rmsSampleInterval = BufferSize;
    }

    public void ClearData() {
        _rmsData = new float[0];
        _rmsSampleInterval = 0;
    }
}
