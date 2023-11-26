using System.Collections;
using UnityEngine;

namespace FC
{

    [CreateAssetMenu(fileName = "HeroData", menuName = "ScriptableObjects/HeroData", order = 1)]
    public class ScriptableHeroData : ScriptableObject
    {
        public EnumHeroTier Tier;

        public string Key;

        public string Name;

        public Sprite PortraitSprite;


        [HideInInspector] public int Level = 1;
        [HideInInspector] public int Elemental = 0;

        public EnumHeroCamp Camp;



        [Header("1레벨 기본 능력치")]
        public int BaseHP = 10;
        public int BaseAtk = 10;
        public float BaseAtkSpeed = 1;

        public float SearchRange;

        [Header("레벨당 추가 능력치")]
        public int LevelUpAddHP = 1;
        public int LevelUpAddAtk = 1;


        public int GetHP(int Level)
        {
            return BaseHP + (LevelUpAddHP * Level);
        }

        public int GetAtk(int Level)
        {
            return BaseAtk + (LevelUpAddAtk * Level);
        }




        public ScriptableHeroData()
        {

        }

        public ScriptableHeroData(ScriptableHeroData copy)
        {
            Tier = copy.Tier;
            Key = copy.Key;
            Name = copy.Name;
            PortraitSprite = copy.PortraitSprite;
            
            Level = 1;
            Elemental = 0;
            Camp = copy.Camp;

            BaseHP = copy.BaseHP;
            BaseAtk = copy.BaseAtk;
            BaseAtkSpeed = copy.BaseAtkSpeed;

            SearchRange = copy.SearchRange;
            LevelUpAddHP = copy.LevelUpAddHP;
            LevelUpAddAtk = copy.LevelUpAddHP;
        }

        public void Copy(ScriptableHeroData copy)
        {
            Tier = copy.Tier;
            Key = copy.Key;
            Name = copy.Name;
            PortraitSprite = copy.PortraitSprite;

            Level = 1;
            Elemental = 0;
            Camp = copy.Camp;


            BaseHP = copy.BaseHP;
            BaseAtk = copy.BaseAtk;
            BaseAtkSpeed = copy.BaseAtkSpeed;

            SearchRange = copy.SearchRange;
            LevelUpAddHP = copy.LevelUpAddHP;
            LevelUpAddAtk = copy.LevelUpAddHP;
    }

    }
}