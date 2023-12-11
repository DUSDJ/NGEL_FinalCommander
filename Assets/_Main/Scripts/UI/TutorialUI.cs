using System.Collections;
using UnityEngine;

namespace FC
{
    public class TutorialUI : MonoBehaviour
    {

        public void OnClickTutorialEnd()
        {
            GameManager.Instance.StartGame();
        }
    }
}