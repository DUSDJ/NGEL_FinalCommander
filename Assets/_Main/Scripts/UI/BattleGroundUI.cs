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

        [Header("Monster Spawn Point")]
        public List<Transform> SpawnPoint;


        [Header("로케이션 데이터")]        
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


        private void InitElement()
        {
            if(SelectedLocation == null)
            {
                return;
            }

            for (int i = 0; i < ElementList.Count; i++)
            {
                if(i + 1 > SelectedLocation.NumOfSlots)
                {
                    ElementList[i].Disable();
                }
                else
                {
                    ElementList[i].gameObject.SetActive(true);
                    ElementList[i].Clean();
                }                
            }

            for (int i = 0; i < PositionList.Count; i++)
            {
                if (i + 1 > SelectedLocation.NumOfSlots)
                {
                    PositionList[i].Disable();
                }
                else
                {
                    PositionList[i].gameObject.SetActive(true);
                    PositionList[i].Clean();
                }
            }
        }




        public void SetUI(bool onOff)
        {
            if (onOff)
            {
                CleanElements();

                InitElement();

                UpdateElements();

                UpdateHero();

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

        public void UpdateHero()
        {
            if(SelectedLocation == null)
            {
                return;
            }

            if(SelectedLocation.NowOwner != EnumLocationOwner.Player)
            {
                return;
            }

            if (SelectedLocation.HeroList != null
                && SelectedLocation.HeroList.Count > 0)
            {
                for (int i = 0; i < SelectedLocation.HeroList.Count; i++)
                {
                    if (!SelectedLocation.HeroList[i].IsAlive)
                    {
                        continue;
                    }

                    var bgElem = GetEmptyElement();
                    if (bgElem == null)
                    {
                        // Alert
                        // UIManager.Instance.AlertUI.SetTextMiddleBlue("슬롯이 가득 찼습니다.");
                        return;
                    }

                    bgElem.SetElement(SelectedLocation.HeroList[i]);
                }
            }
        }




        public ElementBattleGroundHero GetEmptyElement()
        {
            for (int i = 0; i < ElementList.Count; i++)
            {
                if(ElementList[i].gameObject.activeSelf == false)
                {
                    continue;
                }

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
                    // Alert
                    UIManager.Instance.AlertUI.SetTextMiddleBlue("슬롯이 가득 찼습니다.");
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


        public void CleanHeroes()
        {
            for (int i = 0; i < PositionList.Count; i++)
            {
                PositionList[i].Clean();
            }
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
        /// 출격하기
        /// </summary>
        public List<Hero> EngageHeroList()
        {
            List<Hero> heroes = new List<Hero>();

            // 하단 인벤토리에서 영웅 제거됨 (이펙트)
            for (int i = 0; i < selectedElements.Count; i++)
            {
                // 상단 배치에 프리팹 생성됨
                var pos = GetEmptyPosition();
                if (pos != null)
                {
                    var hero = pos.SetElement(selectedElements[i].Data);
                    heroes.Add(hero);

                    // Effect
                    EffectManager.Instance.SetEffect("Effect_Teleport", pos.transform.position);
                }

                selectedElements[i].Clean();
            }
            selectedElements.Clear();


            return heroes;
        }


        public List<Hero> StoreHeroList()
        {
            List<Hero> heroes = new List<Hero>();


            // 하단 인벤토리에서 영웅 제거됨
            for (int i = 0; i < selectedElements.Count; i++)
            {
                selectedElements[i].Clean();
            }
            selectedElements.Clear();


            // 상단 슬롯 모두 프리팹 생성
            for (int i = 0; i < ElementList.Count; i++)
            {
                if (ElementList[i].gameObject.activeSelf == false)
                {
                    continue;
                }

                if(ElementList[i].Data != null)
                {
                    // 상단 배치에 프리팹 생성됨
                    var pos = GetEmptyPosition();
                    if (pos != null)
                    {
                        var hero = pos.SetElement(ElementList[i].Data);
                        heroes.Add(hero);
                    }

                    // Effect
                    EffectManager.Instance.SetEffect("Effect_Teleport", ElementList[i].transform.position);
                }                
            }

            return heroes;
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