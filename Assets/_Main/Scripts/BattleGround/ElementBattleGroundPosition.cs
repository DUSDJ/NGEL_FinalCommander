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


        public void SetElement(ScriptableHeroData data)
        {
            Data = data;

            var prefab = Database.Instance.HeroPrefabDic[data.Key];
            heroPrefab = Instantiate(prefab, HeroPrefabPos, false);
            heroPrefab.gameObject.SetActive(true);
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


        public void OnClickElement()
        {

        }
    }
}