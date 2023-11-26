using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FC
{
    public class Database : MonoBehaviour
    {

        #region SingleTone

        private static Database _instance;

        public static Database Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<Database>();

                    if (!_instance)
                    {
                        var prefab = Resources.Load<Database>("Manager/Database");
                        _instance = Instantiate(prefab);

                        if (!_instance)
                        {
                            Debug.LogError("Database is null");
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion

        [Header("머지에 필요한 영웅 수")]
        public int HeroNeedToMerge = 2;

        [Header("크리에이션 비용")]
        public long CreationCost = 100;



        public List<ScriptableHeroData> HeroData = new List<ScriptableHeroData>();


        [Serializable]
        public struct StructTierImage
        {
            public Sprite Tier_1;
            public Sprite Tier_2;
            public Sprite Tier_3;
            public Sprite Tier_4;
            public Sprite Tier_5;
            public Sprite Tier_6;
        }
        public StructTierImage TierImages;


        public Sprite GetTierImage(EnumHeroTier tier)
        {
            switch (tier)
            {
                case EnumHeroTier.Tier_1:
                    return TierImages.Tier_1;
                case EnumHeroTier.Tier_2:
                    return TierImages.Tier_2;
                case EnumHeroTier.Tier_3:
                    return TierImages.Tier_3;
                case EnumHeroTier.Tier_4:
                    return TierImages.Tier_4;
                case EnumHeroTier.Tier_5:
                    return TierImages.Tier_5;
                case EnumHeroTier.Tier_6:
                    return TierImages.Tier_6;
            }

            return TierImages.Tier_1;
        }

        [Serializable]
        public struct StructElementalImage
        {
            public Sprite Elemental_0;
            public Sprite Elemental_1;
            public Sprite Elemental_2;
        }
        public StructElementalImage ElementalImages;


        public Sprite GetElementalImage(int elemental)
        {
            switch (elemental)
            {
                case 0:
                    return ElementalImages.Elemental_0;
                case 1:
                    return ElementalImages.Elemental_1;
                case 2:
                    return ElementalImages.Elemental_2;
            }

            return ElementalImages.Elemental_0;
        }

        [Serializable]
        public struct StructCampImage
        {
            public Sprite Camp_0_Union;
            public Sprite Camp_1_Demic;
            public Sprite Camp_2_Axis;
        }
        public StructCampImage CampImages;


        public Sprite GetCampImage(EnumHeroCamp camp)
        {
            switch (camp)
            {
                case EnumHeroCamp.Union:
                    return CampImages.Camp_0_Union;
                case EnumHeroCamp.Demic:
                    return CampImages.Camp_1_Demic;
                case EnumHeroCamp.Axis:
                    return CampImages.Camp_2_Axis;
            }

            return CampImages.Camp_0_Union;
        }



        [Serializable]
        public struct StructLocationImage
        {
            public Sprite Default;
            public Sprite Factory;
            public Sprite Storage;
            public Sprite Boss;
        }
        public StructLocationImage LocationImages;


        public Sprite GetLocationImage(EnumLocationType type)
        {
            switch (type)
            {
                case EnumLocationType.Default:
                    return LocationImages.Default;                    
                case EnumLocationType.Factory:
                    return LocationImages.Factory;
                case EnumLocationType.Storage:
                    return LocationImages.Storage;
                case EnumLocationType.Boss:
                    return LocationImages.Boss;
            }

            return LocationImages.Default;
        }



        public Dictionary<string, Hero> HeroPrefabDic;
        public Dictionary<string, Monster> MonsterPrefabDic;




        private void Awake()
        {
            #region SingleTon

            if (Instance == this)
            {
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            #endregion
        }


        public IEnumerator InitRoutine()
        {
            yield return SetHeroPrefabDic();
            yield return SetMonsterPrefabDic();


            yield return null;
        }



        public ScriptableHeroData GetHeroDataByTier(EnumHeroTier tier, int level, int elemental)
        {
            var find = HeroData.FindAll((x) => x.Tier == tier).ToList();
            int rand = UnityEngine.Random.Range(0, find.Count);


            var copy = ScriptableObject.CreateInstance<ScriptableHeroData>();
            copy.Copy(find[rand]);
            copy.Level = level;
            copy.Elemental = elemental;
            return copy;
        }


        private IEnumerator SetHeroPrefabDic()
        {
            if (HeroPrefabDic == null)
            {
                Debug.Log("SetHeroPrefabDic");
                HeroPrefabDic = new Dictionary<string, Hero>();

                var prefabs = Resources.LoadAll<Hero>("HeroPrefab");
                for (int i = 0; i < prefabs.Length; i++)
                {
                    var e = prefabs[i];
                    if (!HeroPrefabDic.ContainsKey(e.name))
                    {
                        var name = e.name.Split('_')[1];
                        HeroPrefabDic.Add(name, e);

                        Debug.Log("HeroPrefabDic Add : " + name);
                    }

                }
            }

            yield return null;
        }

        
        private IEnumerator SetMonsterPrefabDic()
        {
            if (MonsterPrefabDic == null)
            {
                Debug.Log("SetMonsterPrefabDic");
                MonsterPrefabDic = new Dictionary<string, Monster>();

                var prefabs = Resources.LoadAll<Monster>("MonsterPrefab");
                for (int i = 0; i < prefabs.Length; i++)
                {
                    var e = prefabs[i];
                    if (!MonsterPrefabDic.ContainsKey(e.name))
                    {
                        //var name = e.name.Split('_')[1];
                        MonsterPrefabDic.Add(e.name, e);

                        Debug.Log("MonsterPrefabDic Add : " + name);
                    }

                }
            }

            yield return null;
        }


    }
}