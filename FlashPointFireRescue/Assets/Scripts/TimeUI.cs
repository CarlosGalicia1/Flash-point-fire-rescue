using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TimeUI : MonoBehaviour
{

    public TextMeshProUGUI timeText;
    // Start is called before the first frame update
    private void OnEnable()
    {
        TimeManager.OnMinuteChanged += UpdateTime;
        TimeManager.OnHourChanged += UpdateTime;
    }

    private void OnDisable()
    {
        TimeManager.OnMinuteChanged -= UpdateTime;
        TimeManager.OnHourChanged -= UpdateTime;
    }

    private void UpdateTime()
    {
        timeText.text = $"{TimeManager.Hour.ToString("00")}:{TimeManager.Minute:00}";
    }
}
