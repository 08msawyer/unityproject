using System;

public record Countdown
{
    private Action _onComplete;
    private float _timeLeft;

    public void ElapseTime(float timeDelta)
    {
        _timeLeft -= timeDelta;
        if (IsFinished)
        {
            _onComplete();
        }
    }

    public bool IsFinished => _timeLeft <= 0;
}