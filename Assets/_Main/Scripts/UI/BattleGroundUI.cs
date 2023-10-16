using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FC
{
    public class BattleGroundUI : MonoBehaviour
    {
        public List<ElementBattleGroundHero> ElementList = new List<ElementBattleGroundHero>();
        
        public List<ElementBattleGroundPosition> PositionList = new List<ElementBattleGroundPosition>();


        private List<ElementInventoryHero> selectedElements = new List<ElementInventoryHero>();



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

        }





        public void SetUI(bool onOff)
        {
            if (onOff)
            {
                CleanElements();

                gameObject.SetActive(true);
            }
            else
            {
                CleanElements();

                gameObject.SetActive(false);
            }
        }


        public ElementBattleGroundHero GetEmptyElement()
        {
            for (int i = 0; i < ElementList.Count; i++)
            {
                if (ElementList[i].Data == null)
                {
                    return ElementList[i];
                }
            }

            return null;
        }


        public void AddHeroToElement(ElementInventoryHero element)
        {
            // 빼기
            if (selectedElements.Contains(element))
            {
                element.SetSelect(false);

                if (selectedElements.Contains(element))
                {
                    selectedElements.Remove(element);
                }


                for (int i = 0; i < ElementList.Count; i++)
                {
                    if(ElementList[i].MatchedElement == element)
                    {
                        ElementList[i].Clean();
                    }
                }

                return;
            }
            // 더하기
            else
            {
                var bgElem = GetEmptyElement();
                if (bgElem == null)
                {
                    Debug.LogError("출격 추가할 칸이 없음");
                    return;
                }

                element.SetSelect(true);

                selectedElements.Add(element);

                bgElem.SetElement(element);
            }            

        }

        public void SubtractHeroFromElement(ElementBattleGroundHero elem)
        {
            elem.MatchedElement.SetSelect(false);
            if (selectedElements.Contains(elem.MatchedElement))
            {
                selectedElements.Remove(elem.MatchedElement);
            }
            elem.Clean();
        }


        public void CleanElements()
        {
            for (int i = 0; i < ElementList.Count; i++)
            {
                ElementList[i].Clean();
            }

            for (int i = 0; i < selectedElements.Count; i++)
            {
                selectedElements[i].SetSelect(false);
            }
            selectedElements.Clear();
        }




        public void OnClickBackButton()
        {
            if(UIManager.Instance.NowState == EnumUIState.BattleGround)
            {
                UIManager.Instance.SetState(EnumUIState.WorldMap);
            }
        }


    }
}