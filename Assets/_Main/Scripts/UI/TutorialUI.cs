using System.Collections;
using UnityEngine;

namespace FC
{
    public class TutorialUI : MonoBehaviour
    {
        public GameObject PopTutorial;

        public void Init()
        {
            SetUI(false);
        }

        public void SetUI(bool onOff)
        {
            PopTutorial.SetActive(onOff);
        }

        public void OnClickTutorialEnd()
        {
            GameManager.Instance.StartGame();
        }
    }
}