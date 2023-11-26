﻿using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace FC
{
    

    public class ElementLocation : MonoBehaviour
    {
        [HideInInspector] public List<Hero> HeroList;


        [Header("데이터")]
        public string LocationName;
        public EnumLocationType LocationType = EnumLocationType.Default;

        [Header("기본 적 수")]
        public int BaseMonsterCount = 20;

        [Header("적 생성 규칙")]        
        public int MonsterAddValue = 1;
        public List<Monster> MonsterPrefabs;

        public EnumElemental Elemental = EnumElemental.Fire;


        [Header("소유자")] public EnumLocationOwner BaseOwner = EnumLocationOwner.Enemy;

        [Header("정복 보상")]
        public int RewardGold;


        [Header("슬롯 수")]
        public int NumOfSlots = 2;



        [Header("UI")]
        public TextMeshProUGUI LocationNameText;
        public Image LocationIcon;
        public Image ElementalIcon;
        public TextMeshProUGUI NumOfMonsterText;

        public Image ClearLocationIcon;

        public IncomeEffect IncomeEffect;

        
        [HideInInspector] public int NowMonsterCount = 0;
        [HideInInspector] public EnumLocationOwner NowOwner = EnumLocationOwner.Enemy;





        public void Init()
        {
            HeroList = new List<Hero>();

            NowMonsterCount = BaseMonsterCount;
            NowOwner = BaseOwner;

            UpdateElement();
        }


        public void UpdateElement()
        {
            if(NowOwner == EnumLocationOwner.Player)
            {
                ClearLocationIcon.sprite = Database.Instance.GetLocationImage(LocationType);
                ClearLocationIcon.gameObject.SetActive(true);
            }
            else
            {
                ClearLocationIcon.gameObject.SetActive(false);
            }


            LocationNameText.text = string.Format("{0}", LocationName);
            NumOfMonsterText.text = string.Format("{0}", NowMonsterCount);

            LocationIcon.sprite = Database.Instance.GetLocationImage(LocationType);
            ElementalIcon.sprite = Database.Instance.GetElementalImage((int)Elemental);
        }



        /// <summary>
        /// 매 턴마다 적용되는 내용
        /// </summary>
        public void UpdateRoutine()
        {
            if(NowOwner == EnumLocationOwner.Player)
            {
                return;
            }


            // 몬스터 수 증가            
            NowMonsterCount += MonsterAddValue;
            if(NowMonsterCount > 999)
            {
                NowMonsterCount = 999;
            }

            IncomeEffect.SetEffect(string.Format("+{0}", MonsterAddValue));

            UpdateElement();
            UIManager.Instance.BattleGroundUI.UpdateLocationIfSelected(this);
        }



        public void SetHero(List<Hero> list)
        {
            HeroList = new List<Hero>(list);
        }


        public void DecreaseMonster(int value)
        {
            NowMonsterCount -= value;
            if (NowMonsterCount < 0)
            {
                NowMonsterCount = 0;
            }

            UpdateElement();
            UIManager.Instance.BattleGroundUI.UpdateLocationIfSelected(this);
        }


        public void OnClickLocation()
        {
            UIManager.Instance.WorldMapUI.OnClickLocation(this);
        }
    }
}