using System;

public record Countdown
{
    private readonly Action _onComplete;
    private float _timeLeft;

    public Countdown(float timeLeft, Action onComplete)
    {
        _timeLeft = timeLeft;
        _onComplete = onComplete;
    }

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