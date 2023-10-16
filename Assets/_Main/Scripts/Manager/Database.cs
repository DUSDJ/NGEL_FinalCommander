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



        public List<ScriptableHeroData> HeroData = new List<ScriptableHeroData>();

        public struct StructTierImage
        {
            public Sprite Tier_1;
            public Sprite Tier_2;
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
            }

            return TierImages.Tier_1;
        }



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
            yield return null;
        }



        public ScriptableHeroData GetHeroDataByTier(EnumHeroTier tier)
        {
            var find = HeroData.FindAll((x) => x.Tier == tier).ToList();
            int rand = UnityEngine.Random.Range(0, find.Count);

            return find[rand];
        }


    }
}