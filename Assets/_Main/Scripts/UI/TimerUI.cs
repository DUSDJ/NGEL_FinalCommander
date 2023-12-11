using System;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace FC
{
    public class TimerUI : MonoBehaviour
    {
        public TextMeshProUGUI TimerText;



        public void Init()
        {
            TimerText.text = "00:00.00";
        }

        public void SetTimer(double seconds)
        {
            var ts = TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss\.ff");
            TimerText.text = ts;
        }

    }
}