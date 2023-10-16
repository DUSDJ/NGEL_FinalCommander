﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FC
{
    public class InventoryUI : MonoBehaviour
    {
        public GameObject CreationButton;
        public GameObject HeroInfoButton;
        public GameObject MergeButton;


        public List<ElementInventoryHero> ElementList = new List<ElementInventoryHero>();

        private List<ElementInventoryHero> selectedElements = new List<ElementInventoryHero>();
        



        public void Init()
        {
            for (int i = 0; i < ElementList.Count; i++)
            {
                ElementList[i].Clean();
            }

            CreationButton.SetActive(true);
            HeroInfoButton.SetActive(false);
            MergeButton.SetActive(false);
        }


        public ElementInventoryHero GetEmptyElement()
        {
            for (int i = 0; i < ElementList.Count; i++)
            {
                if(ElementList[i].Data == null)
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

                if(e.Data == null)
                {
                    continue;
                }

                if(e == baseElem)
                {
                    continue;
                }

                if(e.Data.Key == baseElem.Data.Key)
                {
                    if (findList.Count < Database.Instance.HeroNeedToMerge - 1)
                    {
                        findList.Add(ElementList[i]);
                    }                    
                }
            }


            // Base는 일단 Select
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
            if(e != null)
            {
                var data = Database.Instance.GetHeroDataByTier(EnumHeroTier.Tier_1);

                e.SetElement(data);
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
            if(selectedElements.Count < Database.Instance.HeroNeedToMerge)
            {
                Debug.LogError($"{Database.Instance.HeroNeedToMerge}개가 선택되어있지 않음");
                return;
            }

            EnumHeroTier nowTier = EnumHeroTier.Tier_1;
            EnumHeroTier nextTier = EnumHeroTier.Tier_2;

            for (int i = 0; i < selectedElements.Count; i++)
            {
                var e = selectedElements[i];

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
                    nextTier = EnumHeroTier.Tier_2;
                    break;
            }


            var data = Database.Instance.GetHeroDataByTier(nextTier);

            var newElement = GetEmptyElement();
            newElement.SetElement(data);

            // 결과물 Element를 단독 Select하기
            newElement.OnClickElement();
        }

        #endregion


    }
}