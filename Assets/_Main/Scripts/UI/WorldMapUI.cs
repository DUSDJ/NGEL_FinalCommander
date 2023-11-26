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
        /// ���� �����͸� �ݿ��Ͽ�
        /// �������� �̵�
        /// </summary>
        public void OnClickLocation(ElementLocation e)
        {
            if(UIManager.Instance.NowState == EnumUIState.WorldMap)
            {
                if(GameManager.Instance.NowGameState == EnumGameState.Battle)
                {
                    if(GameManager.Instance.BattleLocation != e)
                    {
                        UIManager.Instance.AlertUI.SetTextMiddleRed("�ٸ� ������ �������Դϴ�.");
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