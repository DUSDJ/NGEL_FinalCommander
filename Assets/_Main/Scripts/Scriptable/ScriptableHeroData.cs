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
    }
}