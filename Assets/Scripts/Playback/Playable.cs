using UnityEngine.Assertions;

public abstract class Playable {
    protected float _startTime;

    public bool IsPlaying {
        get { return !float.IsNaN(_startTime); }
    }

    public virtual float CurrentTime {
        get { return !float.IsNaN(_startTime) ? AbsoluteTime - _startTime : -1; }
    }

    public float StartTime {
        get { return _startTime; }
    }

    protected abstract float AbsoluteTime {
        get;
    }

    public Playable() {
        _startTime = float.NaN;
    }

    public virtual void Play() {
        _startTime = AbsoluteTime;
    }

    public virtual void SeekTo(float time) {
        // Implementations might need to do setup in Play before being able to seek
        Assert.IsTrue(IsPlaying);

        _startTime = AbsoluteTime - time;
    }

    public virtual void Stop() {
        _startTime = float.NaN;
    }
}
