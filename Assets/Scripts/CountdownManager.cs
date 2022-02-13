using System.Collections.Generic;

public class CountdownManager
{
    private readonly List<Countdown> _countdowns = new();

    public void AddCountdown(Countdown countdown)
    {
        _countdowns.Add(countdown);
    }

    public void ElapseTime(float timeDelta)
    {
        for (var i = 0; i < _countdowns.Count;)
        {
            var countdown = _countdowns[i];
            countdown.ElapseTime(timeDelta);
            if (countdown.IsFinished)
            {
                _countdowns.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }
}