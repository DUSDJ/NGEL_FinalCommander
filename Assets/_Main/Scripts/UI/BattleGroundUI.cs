using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FC
{
    public class BattleGroundUI : MonoBehaviour
    {
        public List<ElementBattleGroundHero> ElementList = new List<ElementBattleGroundHero>();
        
        public List<ElementBattleGroundPosition> PositionList = new List<ElementBattleGroundPosition>();


        private List<ElementInventoryHero> selectedElements = new List<ElementInventoryHero>();





        [Header("�����̼� ������")]        
        public ElementLocation SelectedLocation;

        [Header("UI")]
        public TextMeshProUGUI NumOfMonsterText;

        public TextMeshProUGUI LocationNameText;
        public Image LocationTypeIcon;
        public Image LocationElementalIcon;



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

                UpdateElements();

                gameObject.SetActive(true);
            }
            else
            {
                CleanElements();

                gameObject.SetActive(false);
            }
        }




        public void UpdateLocationIfSelected(ElementLocation e)
        {
            if(SelectedLocation != null
                && SelectedLocation == e)
            {
                UpdateElements();
            }
        }


        private void UpdateElements()
        {
            if(SelectedLocation == null)
            {
                return;
            }


            NumOfMonsterText.text = string.Format("{0}", SelectedLocation.NowMonsterCount);
            LocationNameText.text = SelectedLocation.LocationName;
            LocationTypeIcon.sprite = Database.Instance.GetLocationImage(SelectedLocation.LocationType);
            LocationElementalIcon.sprite = Database.Instance.GetElementalImage((int)SelectedLocation.Elemental);
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
            // ����
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
            // ���ϱ�
            else
            {
                var bgElem = GetEmptyElement();
                if (bgElem == null)
                {
                    Debug.LogError("��� �߰��� ĭ�� ����");
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
            for (int i = 0; i < PositionList.Count; i++)
            {
                PositionList[i].Clean();
            }

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



        /// <summary>
        /// ����ϱ�
        /// </summary>
        public void Engage()
        {
            // �ϴ� �κ��丮���� ���� ���ŵ� (����Ʈ)
            for (int i = 0; i < selectedElements.Count; i++)
            {
                // ��� ��ġ�� ������ ������
                var pos = GetEmptyPosition();
                if (pos != null)
                {
                    pos.SetElement(selectedElements[i].Data);

                    // Effect
                    EffectManager.Instance.SetEffect("Effect_Teleport", pos.transform.position);
                }

                selectedElements[i].Clean();
            }
            selectedElements.Clear();            
        }


        private ElementBattleGroundPosition GetEmptyPosition()
        {
            var list = PositionList;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Data == null)
                {
                    return list[i];
                }
            }

            return null;
        }

    }
}