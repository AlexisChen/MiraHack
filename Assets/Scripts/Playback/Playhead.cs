using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Tracks the current song playback time and sends out song events.
/// </summary>
public sealed class Playhead : Playable {
    public delegate void BeatHandler(int beat);
    public delegate void SubdivisionHandler(int beat, int subdivision);
    public delegate void SkipHandler(float timeDelta);

    /// <summary>
    /// Invoked on each beat.
    /// May be invoked multiple times in a frame (in order) if multiple beats have elapsed between
    /// frames.
    /// </summary>
    public event BeatHandler Beat;

    /// <summary>
    /// Invoked on each subdivision.
    /// May be invoked multiple times in a frame (in order) if multiple subdivisions have elapsed
    /// between frames.
    /// </summary>
    public event SubdivisionHandler Subdivision;

    /// <summary>
    /// Invoked once when the song finishes playing.
    /// </summary>
    public event System.Action SongEnd;

    /// <summary>
    /// Invoked when SeekTo is called and part of the song is skipped.
    /// Stems will not have been updated yet.
    /// </summary>
    public event SkipHandler Skip;

    private SongData _song;
    private int _bpmPointIndex;
    private int _currentBeat, _currentSubdivision;
    private StemPlayhead[] _stemPlayheads;

    public SongData Song {
        get { return _song; }
        set {
            if (_song != null) Debug.LogFormat("[Playhead] Replacing {0} with {1}", _song, value);
            _song = value;
            CreateStemPlayheads();
        }
    }

    public int CurrentBeat {
        get { return _currentBeat; }
    }

    public int CurrentSubdivision {
        get { return _currentSubdivision; }
    }

    public int CurrentMeasure {
        get { return _currentBeat / _song.BeatsPerMeasure; }
    }

    public int CurrentMeasureBeat {
        get { return _currentBeat % _song.BeatsPerMeasure; }
    }

    protected override float AbsoluteTime {
        get { return (float) AudioSettings.dspTime; }
    }

    public Playhead() {
        _currentBeat = -1;
        _currentSubdivision = -1;
        CreateStemPlayheads();
    }

    /// <summary>
    /// Get the StemPlayhead for a specific stem.
    /// </summary>
    public StemPlayhead GetStemPlayhead(StemData stem) {
        if (stem == _song.MainStem) {
            return _stemPlayheads[0];
        }

        int index = System.Array.IndexOf(_song.Stems, stem);
        return _stemPlayheads[index + 1];
    }

    /// <summary>
    /// Skip forward to a specific point in song time.
    /// Beat and subdivision events between now and the specified time won't be invoked, but all
    /// StemPlayhead notes in between will be (in order).
    /// </summary>
    public override void SeekTo(float time) {
        Assert.IsTrue(IsPlaying && time > CurrentTime);

        var oldTime = CurrentTime;
        base.SeekTo(time);

        CalculateBeatSubdiv(out _currentBeat, out _currentSubdivision);

        if (Skip != null) Skip(time - oldTime);
    }

    /// <summary>
    /// Skip forward to a specific point in song time.
    /// Beat and subdivision events between now and the specified time won't be invoked, but all
    /// StemPlayhead notes in between will be (in order).
    /// </summary>
    public void SeekTo(int beat, int subdivision) {
        Assert.IsTrue(IsPlaying);
        // We can't skip backward because then we would need to reset and skip forward each StemPlayhead,
        // and skipping to now doesn't make any sense
        Assert.IsTrue(beat > _currentBeat || (beat == _currentBeat && subdivision > _currentSubdivision));

        var timeDelta = TimeDifference(beat, subdivision);
        _startTime -= timeDelta;
        Debug.LogFormat("[Playhead] Skipping {0:0.##}s from {1}.{2} to {3}.{4}", timeDelta,
            _currentBeat, _currentSubdivision, beat, subdivision);

#if UNITY_ASSERTIONS
        // Compute the new time and make sure it's consistent with the args
        int newBeat, newSubdivision;
        CalculateBeatSubdiv(out newBeat, out newSubdivision);
        Assert.IsTrue(beat == newBeat && subdivision == newSubdivision,
            string.Format("Skipped time is consistent: {0}.{1} == {2}.{3}", beat, subdivision,
                newBeat, newSubdivision));
#endif

        _currentBeat = beat;
        _currentSubdivision = subdivision;

        if (Skip != null) Skip(timeDelta);
    }

    /// <summary>
    /// Get the time difference in seconds between now and the specified time.
    /// </summary>
    public float TimeDifference(int beat, int subdivision) {
        Assert.IsTrue(IsPlaying);
        // Skipping backward not supported for now
        Assert.IsTrue(beat > _currentBeat || (beat == _currentBeat && subdivision >= _currentSubdivision));

        float timeDelta = 0;
        var movingBpmPointIndex = _bpmPointIndex;
        var movingBeat = _currentBeat;
        var movingSubdiv = _currentSubdivision;
        var movingBeatInterval = 60f / _song.BpmPoints[movingBpmPointIndex].Bpm;
        var movingSubdivInterval = movingBeatInterval / _song.Subdivisions;

        // Move to be on beat so that BPM point math is easier
        if (movingSubdiv != 0) {
            timeDelta -= movingSubdivInterval * movingSubdiv;
            movingSubdiv = 0;
        }

        // Move forward to the next BPM point if the beat is past it
        while (movingBpmPointIndex + 1 < _song.BpmPoints.Length &&
               beat >= _song.BpmPoints[movingBpmPointIndex + 1].StartBeat) {
            var bpmStartBeatDiff = _song.BpmPoints[movingBpmPointIndex + 1].StartBeat - movingBeat;
            timeDelta += bpmStartBeatDiff * movingBeatInterval;
            movingBeat += bpmStartBeatDiff;
            ++movingBpmPointIndex;

            movingBeatInterval = 60f / _song.BpmPoints[movingBpmPointIndex].Bpm;
            movingSubdivInterval = movingBeatInterval / _song.Subdivisions;
        }

        // Move to the beat
        var beatDiff = beat - movingBeat;
        timeDelta += beatDiff * movingBeatInterval;
        movingBeat += beatDiff;

        // Move to the subdiv
        timeDelta += subdivision * movingSubdivInterval;
        movingSubdiv += subdivision;

        Assert.IsTrue(movingBeat == beat && movingSubdiv == subdivision);
        return timeDelta;
    }

    public override void Play() {
        base.Play();

        // Reset
        _bpmPointIndex = 0;
        _currentBeat = -1;
        _currentSubdivision = -1;

        // Reset where we're looking in each stem
        for (int i = 0; i < _stemPlayheads.Length; ++i) {
            if (_stemPlayheads[i] != null && _stemPlayheads[i].Stem.Autoplay) {
                _stemPlayheads[i].Play();
            }
        }
    }

    public override void Stop() {
        if (!IsPlaying) return;

        if (SongEnd != null) SongEnd();
        base.Stop();
    }

    internal void Update() {
        if (!IsPlaying) return;

        UpdateBeat();

        // Update stems too
        for (int i = 0; i < _stemPlayheads.Length; ++i) {
            if (_stemPlayheads[i] != null) {
                _stemPlayheads[i].Update(_currentBeat, _currentSubdivision);
            }
        }
    }

    private void CreateStemPlayheads() {
        if (_song == null) return;

        _stemPlayheads = new StemPlayhead[1 + _song.Stems.Length]; // main stem at index 0

        // Create playheads from stems
        if (_song.MainStem != null) {
            _stemPlayheads[0] = new StemPlayhead(this, _song.MainStem);
        } else {
            Debug.LogWarning("[Playhead] Main stem is not set");
        }
        for (int i = 0; i < _song.Stems.Length; ++i) {
            if (_song.Stems[i] != null) {
                _stemPlayheads[1 + i] = new StemPlayhead(this, _song.Stems[i]);
            }
        }
    }

    private void UpdateBeat() {
        var elapsed = CurrentTime;

        // Wait until beat starts
        if (elapsed < _song.StartDelay) return;

        // Stop when song ends if the duration is set
        if (_song.Duration > 0 && elapsed > _song.Duration) {
            Stop();
            return;
        }

        // Move to the next BPM point if we've passed it
        while (_bpmPointIndex + 1 < _song.BpmPoints.Length &&
            elapsed >= _song.BpmPoints[_bpmPointIndex + 1].StartTime) {
            ++_bpmPointIndex;
        }

        // Calculate the current beat/subdiv
        int beat, subdivision;
        CalculateBeatSubdiv(out beat, out subdivision);

        // Save state
        var lastBeat = _currentBeat;
        var lastSubdivision = _currentSubdivision;
        _currentBeat = beat;
        _currentSubdivision = subdivision;

        // Invoke events on a new beat
        if (beat != lastBeat) {
            if (Beat != null) Beat(beat);
        }

        // Invoke events on a new subdivision
        if (beat != lastBeat || subdivision != lastSubdivision) {
            if (Subdivision != null) Subdivision(beat, subdivision);
        }
    }

    private void CalculateBeatSubdiv(out int beat, out int subdivision) {
        var bpmPointTime = CurrentTime - _song.BpmPoints[_bpmPointIndex].StartTime;
        var bpmPointBeats = bpmPointTime / 60f * _song.BpmPoints[_bpmPointIndex].Bpm; // bpmPointTime / beat interval
        beat = _song.BpmPoints[_bpmPointIndex].StartBeat + (int)bpmPointBeats;
        subdivision = (int)((bpmPointBeats % 1) * _song.Subdivisions);
    }
}
