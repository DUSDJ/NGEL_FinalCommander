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
        }

    }
}