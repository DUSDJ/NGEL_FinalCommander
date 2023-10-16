using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FC
{
    public class BattleGroundUI : MonoBehaviour
    {
        public List<ElementBattleGroundHero> ElementList = new List<ElementBattleGroundHero>();
        
        public List<ElementBattleGroundPosition> PositionList = new List<ElementBattleGroundPosition>();




        public void Init()
        {
            for (int i = 0; i < ElementList.Count; i++)
            {
                ElementList[i].Clean();
            }

            for (int i = 0; i < PositionList.Count; i++)
            {
                PositionList[i].Clean();
            }


            SetUI(false);
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







        public void OnClickBackButton()
        {
            if(UIManager.Instance.NowState == EnumUIState.BattleGround)
            {
                UIManager.Instance.SetState(EnumUIState.WorldMap);
                UIManager.Instance.WorldMapUI.SetUI(true);


                SetUI(false);
            }
        }


    }
}