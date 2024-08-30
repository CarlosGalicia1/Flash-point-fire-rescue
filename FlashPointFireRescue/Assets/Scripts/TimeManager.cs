using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static Action OnMinuteChanged;
    public static Action OnHourChanged;

    public static int Minute { get; private set; }
    public static int Hour { get; private set; }

    private float timer;

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Minute++;

            OnMinuteChanged?.Invoke();

            if (Minute >= 60)
            {
                Hour++;
                OnHourChanged?.Invoke();
                Minute = 0;
            }

            timer = GameConstants.minuteToRealTime;
        }
    }

    public void ResetTime()
    {
        Minute = 0;
        Hour = 0;
        timer = GameConstants.minuteToRealTime;
    }
}
