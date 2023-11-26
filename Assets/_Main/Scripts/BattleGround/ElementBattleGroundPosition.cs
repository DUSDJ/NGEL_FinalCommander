using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FC
{
    public class ElementBattleGroundPosition : MonoBehaviour
    {
        [HideInInspector]public ScriptableHeroData Data;

        public Transform HeroPrefabPos;

        private Hero heroPrefab;


        public Hero SetElement(ScriptableHeroData data)
        {
            gameObject.SetActive(true);

            Data = data;

            var prefab = Database.Instance.HeroPrefabDic[data.Key];
            heroPrefab = Instantiate(prefab, HeroPrefabPos, false);
            heroPrefab.gameObject.SetActive(true);
            heroPrefab.Data = data;
            heroPrefab.SetHero();


            return heroPrefab;
        }


        public void Clean()
        {
            Data = null;

            if (heroPrefab != null)
            {
                heroPrefab.gameObject.SetActive(false);
                heroPrefab = null;
            }
            
        }


        public void Disable()
        {
            Clean();

            gameObject.SetActive(false);
        }


        public void OnClickElement()
        {

        }
    }
}