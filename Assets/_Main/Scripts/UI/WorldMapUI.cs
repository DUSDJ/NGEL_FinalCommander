using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FC
{
    public class WorldMapUI : MonoBehaviour
    {
        [HideInInspector] public List<ElementLocation> LocationList;



        public void Init()
        {
            LocationList = new List<ElementLocation>();
            LocationList = FindObjectsOfType<ElementLocation>(false).ToList();

            for (int i = 0; i < LocationList.Count; i++)
            {
                LocationList[i].Init();
            }
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



        public void UpdateLocation()
        {
            for (int i = 0; i < LocationList.Count; i++)
            {
                LocationList[i].UpdateRoutine();
            }

        }



        /// <summary>
        /// 지역 데이터를 반영하여
        /// 전장으로 이동
        /// </summary>
        public void OnClickLocation(ElementLocation e)
        {
            if(UIManager.Instance.NowState == EnumUIState.WorldMap)
            {
                if(GameManager.Instance.NowGameState == EnumGameState.Battle)
                {
                    if(GameManager.Instance.BattleLocation != e)
                    {
                        UIManager.Instance.AlertUI.SetTextMiddleRed("다른 지역이 전투중입니다.");
                        return;
                    }
                    else
                    {
                        UIManager.Instance.BattleGroundUI.SelectedLocation = e;
                        UIManager.Instance.SetState(EnumUIState.BattleGround);
                    }
                }
                else
                {
                    UIManager.Instance.BattleGroundUI.SelectedLocation = e;
                    UIManager.Instance.SetState(EnumUIState.BattleGround);
                }                
            }
        }


    }
}