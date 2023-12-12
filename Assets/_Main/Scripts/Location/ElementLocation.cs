using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace FC
{
    

    public class ElementLocation : MonoBehaviour
    {
        [HideInInspector] public List<Hero> HeroList;


        [Header("해금 조건")]
        public List<ElementLocation> UnlockLocation = new List<ElementLocation>();
        
        [Header("해금시 On되는 오브젝트들")]
        public List<GameObject> UnlockActiveObject = new List<GameObject>();


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
        public Image BattleLocationIcon;

        public Image LockIcon;

        public IncomeEffect IncomeEffect;


        #region 월드맵에서 보이는 영웅 슬롯

        
        public List<ElementLocationSlot> Slots = new List<ElementLocationSlot>();

        public void InitSlots()
        {
            Slots = GetComponentsInChildren<ElementLocationSlot>(true).ToList();

            for (int i = 0; i < Slots.Count; i++)
            {
                Slots[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < NumOfSlots; i++)
            {
                Slots[i].gameObject.SetActive(true);
            }

            CleanSlots();
        }

        public void CleanSlots()
        {
            for (int i = 0; i < Slots.Count; i++)
            {
                Slots[i].Clean();
            }
        }

        public void SetSlots(int index, Hero hero)
        {
            if(index >= NumOfSlots)
            {
                return;
            }

            Slots[index].SetElement(hero);
        }

        public void UpdateSlots()
        {
            for (int i = 0; i < HeroList.Count; i++)
            {                
                Slots[i].SetElement(HeroList[i]);
            }
            
        }

        #endregion



        [HideInInspector] public int NowMonsterCount = 0;
        [HideInInspector] public EnumLocationOwner NowOwner = EnumLocationOwner.Enemy;





        public void Init()
        {
            InitSlots();

            HeroList = new List<Hero>();

            NowMonsterCount = BaseMonsterCount;
            NowOwner = BaseOwner;

            UpdateElement();            
        }


        public void UpdateElement()
        {
            LockIcon.gameObject.SetActive(false);

            for (int u = 0; u < UnlockActiveObject.Count; u++)
            {
                UnlockActiveObject[u].SetActive(true);
            }

            if (UnlockLocation != null && UnlockLocation.Count > 0)
            {
                for (int i = 0; i < UnlockLocation.Count; i++)
                {
                    if (UnlockLocation[i].NowOwner != EnumLocationOwner.Player)
                    {
                        LockIcon.gameObject.SetActive(true);

                        for (int u = 0; u < UnlockActiveObject.Count; u++)
                        {
                            UnlockActiveObject[u].SetActive(false);
                        }

                        break;
                    }
                }
            }            


            BattleLocationIcon.gameObject.SetActive(false);

            if (NowOwner == EnumLocationOwner.Player)
            {
                ClearLocationIcon.sprite = Database.Instance.GetLocationImage(LocationType);
                ClearLocationIcon.gameObject.SetActive(true);
            }
            else
            {
                ClearLocationIcon.gameObject.SetActive(false);

                if(GameManager.Instance.BattleLocation == this)
                {
                    BattleLocationIcon.gameObject.SetActive(true);
                }
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

            CleanSlots();
            for (int i = 0; i < HeroList.Count; i++)
            {
                SetSlots(i, HeroList[i]);
            }            
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
            // 해금
            if(UnlockLocation != null && UnlockLocation.Count > 0)
            {
                for (int i = 0; i < UnlockLocation.Count; i++)
                {
                    if(UnlockLocation[i].NowOwner != EnumLocationOwner.Player)
                    {
                        UIManager.Instance.AlertUI.SetText("잠겨있습니다.",
                            UIManager.Instance.AlertUI.AlertColor.AlertRed,
                            "다른 지역을 먼저 공략하세요.",
                            UIManager.Instance.AlertUI.AlertColor.AlertRed);
                        return;
                    }
                }
            }

            for (int i = 0; i < UnlockActiveObject.Count; i++)
            {
                UnlockActiveObject[i].SetActive(true);
            }

            UIManager.Instance.WorldMapUI.OnClickLocation(this);
        }
    }
}