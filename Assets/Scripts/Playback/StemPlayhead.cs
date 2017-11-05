using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Tracks the current playback point in a stem and sends out stem note events.
/// </summary>
public sealed class StemPlayhead : Playable {
    public delegate void NoteHandler(StemPlayhead playhead, int beat, int subdiv);

    /// <summary>
    /// Invoked when a stem has a note.
    /// May be invoked multiple times in a frame (in order).
    /// </summary>
    public event NoteHandler Note;

    private StemData _stem;
    private Playhead _playhead;

    /// Pointer into the _stem.NoteTimes array tracking the next note
    private int _stemNoteIndex;
    private int _currentBeat, _currentSubdivision;

    public StemData Stem {
        get { return _stem; }
    }

    public override float CurrentTime {
        get {
            return _stem.Loop && IsPlaying
                ? base.CurrentTime % _stem.Clip.length
                : base.CurrentTime;
        }
    }

    internal StemPlayhead(Playhead playhead, StemData stem) {
        _playhead = playhead;
        _stem = stem;
    }

    protected override float AbsoluteTime {
        get { return (float) AudioSettings.dspTime; }
    }

    public override void Play() {
        base.Play();
        _stemNoteIndex = 0;
        _currentBeat = 0;
        _currentSubdivision = 0;
    }

    /// <summary>
    /// Seek to a specific point in stem time.
    /// </summary>
    public override void SeekTo(float time) {
        base.SeekTo(time);

        int newBeat, newSubdivision;
        CalculateBeatSubdiv(out newBeat, out newSubdivision);

        // Find the right stem note index
        _stemNoteIndex = 0;
        if (_stem.NoteTimes.Length > 0) {
            var noteTime = _stem.NoteTimes[0];
            while (noteTime.Beat < newBeat || (noteTime.Beat == newBeat && noteTime.Subdivision <= newSubdivision)) {
                if (Note != null) Note(this, newBeat, newSubdivision);

                // Move to next
                ++_stemNoteIndex;
                if (_stemNoteIndex < _stem.NoteTimes.Length) {
                    noteTime = _stem.NoteTimes[_stemNoteIndex];
                } else {
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Seek to a specific point in stem time.
    /// </summary>
    public void SeekTo(int beat, int subdivision) {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Check if there is a note on this beat and subdivision and invoke OnNote if there is
    /// </summary>
    internal void Update(int beat, int subdivision) {
        // Update the note on a new beat
        if (beat != _currentBeat || subdivision != _currentSubdivision) {
            // Stop if past the end of the stem
            if (_stem.NoteTimes == null || _stemNoteIndex >= _stem.NoteTimes.Length) return;

            // Invoke all events up to and including this beat and subdivision if they haven't been invoked yet
            var noteTime = _stem.NoteTimes[_stemNoteIndex];
            while (noteTime.Beat < beat || (noteTime.Beat == beat && noteTime.Subdivision <= subdivision)) {
                if (Note != null) Note(this, beat, subdivision);

                // Move to next
                ++_stemNoteIndex;
                if (_stemNoteIndex < _stem.NoteTimes.Length) {
                    noteTime = _stem.NoteTimes[_stemNoteIndex];
                } else {
                    break;
                }
            }
        }

        _currentBeat = beat;
        _currentSubdivision = subdivision;
    }

    /// <summary>
    /// Get the volume of the stem at the current time.
    /// </summary>
    /// <param name="offset">A time offset forward.</param>
    /// <returns>The RMS volume.</returns>
    public float GetVolume(float offset = 0) {
        if (!IsPlaying) return 0;

        Assert.IsNotNull(_stem.Clip);

        var bin = GetBinForTime(CurrentTime + offset, _stem.Clip, _stem.RmsSampleInterval);
        if (bin < 0 || (int) bin >= _stem.RmsData.Length) return 0;

        // Don't go off the end
        if ((int) bin == _stem.RmsData.Length - 1) {
            return _stem.RmsData[(int) bin];
        }

        // Interpolate between prev bin and next bin
        return Mathf.LerpUnclamped(_stem.RmsData[(int) bin], _stem.RmsData[(int) bin + 1], bin % 1);
    }

    private static float GetBinForTime(float time, AudioClip clip, int sampleInterval) {
        var bin = time * ((float) clip.frequency / sampleInterval);
        bin *= clip.channels; // samples are interleaved, so move a multiple of the time
        return bin;
    }

    private void CalculateBeatSubdiv(out int beat, out int subdivision) {
        // TODO: HACK which just uses the first BPM
        // Figure out how to design this so that BPMs are associated with stems
        var beatTime = CurrentTime / 60f * _playhead.Song.BpmPoints[0].Bpm;
        beat = (int) beatTime;
        subdivision = (int) ((beatTime % 1) * _playhead.Song.Subdivisions);
    }
}
