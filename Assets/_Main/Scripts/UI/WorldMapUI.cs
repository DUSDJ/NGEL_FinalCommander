using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FC
{
    public class WorldMapUI : MonoBehaviour
    {
        

        public void Init()
        {

        }


        public void SetUI(bool onOff)
        {
            if (onOff)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }




        /// <summary>
        /// 그냥 전장으로 이동됨
        /// </summary>
        public void OnClickLocation()
        {
            if(UIManager.Instance.NowState == EnumUIState.WorldMap)
            {
                UIManager.Instance.SetState(EnumUIState.BattleGround);
            }            
        }


    }
}