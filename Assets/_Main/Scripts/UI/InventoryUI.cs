using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FC
{
    public class InventoryUI : MonoBehaviour
    {
        public TextMeshProUGUI GoldText;

        public void UpdateGold(long value)
        {
            GoldText.text = string.Format("{0}", value);
        }


        public GameObject CreationButton;
        public GameObject HeroInfoButton;
        public GameObject MergeButton;

        public GameObject BGCleanButton;
        public GameObject BGEnageButton;


        public List<ElementInventoryHero> ElementList = new List<ElementInventoryHero>();

        private List<ElementInventoryHero> selectedElements = new List<ElementInventoryHero>();







        public void Init()
        {
            for (int i = 0; i < ElementList.Count; i++)
            {
                ElementList[i].Clean();
            }
        }


        public ElementInventoryHero GetEmptyElement()
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


        private void CleanSelected()
        {
            for (int i = 0; i < selectedElements.Count; i++)
            {
                selectedElements[i].SetSelect(false);
            }
            selectedElements.Clear();
        }

        /// <summary>
        /// 같은 거 한 번 더 누르면 취소됨
        /// </summary>
        public void CleanSelection()
        {
            CreationButton.SetActive(true);
            HeroInfoButton.SetActive(false);
            MergeButton.SetActive(false);

            CleanSelected();
        }

        /// <summary>
        /// Element 한 번 눌렀을 때
        /// </summary>
        /// <param name="baseElem"></param>
        /// <param name="data"></param>
        public void CheckHeroCanThreeMerge(ElementInventoryHero baseElem)
        {
            CreationButton.SetActive(false);
            HeroInfoButton.SetActive(true);
            MergeButton.SetActive(false);

            CleanSelected();

            List<ElementInventoryHero> findList = new List<ElementInventoryHero>();

            // 모든 Element SetSelect Clean 하면서 찾는다
            for (int i = 0; i < ElementList.Count; i++)
            {
                var e = ElementList[i];
                e.SetSelect(false);

                if (e.Data == null)
                {
                    continue;
                }

                if (e == baseElem)
                {
                    continue;
                }

                if (e.Data.Key == baseElem.Data.Key)
                {
                    if (findList.Count < Database.Instance.HeroNeedToMerge - 1)
                    {
                        findList.Add(ElementList[i]);
                    }
                }
            }


            // Base는 일단 Select (selectedElements[0]가 base다)
            baseElem.SetSelect(true);
            selectedElements.Add(baseElem);

            // Base 포함해서 3개 Select Frame On & MergeButton On
            if (findList.Count >= Database.Instance.HeroNeedToMerge - 1)
            {
                MergeButton.SetActive(true);

                for (int i = 0; i < findList.Count; i++)
                {
                    findList[i].SetSelect(true);

                    selectedElements.Add(findList[i]);
                }
            }
        }







        #region OnClick Event

        public void OnClickBtnCreation()
        {
            var e = GetEmptyElement();

            // 빈 칸 있음 : 뽑기 가능
            if (e != null)
            {
                int level = UnityEngine.Random.Range(1, 25 + 1);
                int elemental = UnityEngine.Random.Range(0, 2 + 1);
                var data = Database.Instance.GetHeroDataByTier(EnumHeroTier.Tier_1, level, elemental);                
                e.SetElement(data);

                // 골드 깜
                GameManager.Instance.Gold -= Database.Instance.CreationCost;

                // Effect
                EffectManager.Instance.SetEffect("Effect_LevelUp", e.transform.position);                                
            }
            // 빈 칸 없음 : 봅기 불가
            else
            {
                Debug.LogError("빈 칸 없어서 뽑기 불가!");
                return;
            }
        }

        public void OnClickBtnMerge()
        {
            if (selectedElements.Count < Database.Instance.HeroNeedToMerge)
            {
                Debug.LogError($"{Database.Instance.HeroNeedToMerge}개가 선택되어있지 않음");
                return;
            }

            EnumHeroTier nowTier = EnumHeroTier.Tier_1;
            EnumHeroTier nextTier = EnumHeroTier.Tier_2;

            int selectedElemental = 0;
            int mergeLevel = 0;
            for (int i = 0; i < selectedElements.Count; i++)
            {
                var e = selectedElements[i];

                if(i == 0)
                {
                    selectedElemental = e.Data.Elemental;
                }
                

                mergeLevel += e.Data.Level;
                nowTier = e.Data.Tier;
                e.Clean();
            }

            CleanSelected();

            switch (nowTier)
            {
                case EnumHeroTier.Tier_1:
                    nextTier = EnumHeroTier.Tier_2;
                    break;
                case EnumHeroTier.Tier_2:
                    nextTier = EnumHeroTier.Tier_3;
                    break;
                case EnumHeroTier.Tier_3:
                    nextTier = EnumHeroTier.Tier_4;
                    break;
                case EnumHeroTier.Tier_4:
                    nextTier = EnumHeroTier.Tier_5;
                    break;
                case EnumHeroTier.Tier_5:
                    nextTier = EnumHeroTier.Tier_6;
                    break;
                case EnumHeroTier.Tier_6:
                    nextTier = EnumHeroTier.Tier_6;
                    break;
            }


            var data = Database.Instance.GetHeroDataByTier(nextTier, mergeLevel, selectedElemental);

            var newElement = GetEmptyElement();
            newElement.SetElement(data);

            // 결과물 Element를 단독 Select하기
            newElement.OnClickElement();

            // Effect
            EffectManager.Instance.SetEffect("Effect_LevelUp_2", newElement.transform.position);
        }



        public void OnClickBGCleanButton()
        {
            UIManager.Instance.BattleGroundUI.CleanElements();
        }


        /// <summary>
        /// [출격하기] 버튼
        /// </summary>
        public void OnClickBGEngageButton()
        {
            UIManager.Instance.BattleGroundUI.Engage();
        }

        #endregion



        #region Set State

        public void SetStateWorldMap()
        {
            CreationButton.SetActive(true);
            HeroInfoButton.SetActive(false);
            MergeButton.SetActive(false);

            CleanSelected();

            BGCleanButton.SetActive(false);
            BGEnageButton.SetActive(false);
        }

        public void SetStateBattleGround()
        {
            CreationButton.SetActive(false);
            HeroInfoButton.SetActive(false);
            MergeButton.SetActive(false);

            CleanSelected();

            BGCleanButton.SetActive(true);
            BGEnageButton.SetActive(true);
        }

        #endregion


    }
}