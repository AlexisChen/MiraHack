using UnityEngine;

[CreateAssetMenu(fileName = "New Song", menuName = "Mercive/Song", order = 100)]
public sealed class SongData : ScriptableObject {
    [System.Serializable]
    public struct BpmPoint {
        [ReadOnly] public float StartTime;

        [Space]
        public int StartBeat;
        public float Bpm;
    }

    [SerializeField]
    private float _duration;

    [SerializeField]
    private StemData _mainStem;

    [SerializeField]
    private StemData[] _stems;

    [Header("Time")]
    [SerializeField]
    private int _beatsPerMeasure = 4;

    [SerializeField]
    private int _subdivisions = 1;

    [SerializeField]
    private float _startDelay = 0;

    [SerializeField]
    private BpmPoint[] _bpmPoints;

    public float Duration {
        get { return _duration; }
    }

    public StemData MainStem {
        get { return _mainStem; }
    }

    public StemData[] Stems {
        get { return _stems; }
    }

    public int BeatsPerMeasure {
        get { return _beatsPerMeasure; }
    }

    public int Subdivisions {
        get { return _subdivisions; }
    }

    public float StartDelay {
        get { return _startDelay; }
    }

    public BpmPoint[] BpmPoints {
        get { return _bpmPoints; }
    }

    private void OnValidate() {
        if (_duration < 0) _duration = 0;
        if (_beatsPerMeasure < 1) _beatsPerMeasure = 1;
        if (_subdivisions < 1) _subdivisions = 1;
        if (_startDelay < 0) _startDelay = 0;

        if (_bpmPoints == null || _bpmPoints.Length < 1) {
            _bpmPoints = new BpmPoint[1];
        }

        CalculateBpmStartTimes();
    }

    public void CalculateBpmStartTimes() {
        // Calculate start time for each BPM point
        float time = _startDelay;
        for (int i = 0; i < _bpmPoints.Length; ++i) {
            if (i > 0) {
                var beatInterval = 60f / _bpmPoints[i - 1].Bpm;
                time += beatInterval * (_bpmPoints[i].StartBeat - _bpmPoints[i - 1].StartBeat);
            }

            _bpmPoints[i].StartTime = time;
        }
    }
}
