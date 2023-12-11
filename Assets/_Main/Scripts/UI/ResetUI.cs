using System.Collections;
using UnityEngine;

namespace FC
{
    public class ResetUI : MonoBehaviour
    {
       
        public void OnClickReset()
        {
            GameManager.Instance.ResetGame();
        }
    }
}