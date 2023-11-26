using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FC
{
    public class MonsterManager : MonoBehaviour
    {

        #region SingleTone

        private static MonsterManager _instance;

        public static MonsterManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<MonsterManager>();

                    if (!_instance)
                    {
                        var prefab = Resources.Load<MonsterManager>("Manager/MonsterManager");
                        _instance = Instantiate(prefab);

                        if (!_instance)
                        {
                            Debug.LogError("MonsterManager is null");
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion




        #region Pooling

        public Dictionary<string, List<Monster>> ListDic;



        public void IncreasePool(string key, int num)
        {
            for (int i = 0; i < num; i++)
            {
                var e = Instantiate(Database.Instance.MonsterPrefabDic[key]);
                e.transform.SetParent(UIManager.Instance.BattleGroundUI.MonsterParent, false);
                ListDic[key].Add(e);
                e.gameObject.SetActive(false);
            }
        }


        public Monster SetMonster(string msg, Vector3 position)
        {
            if (Database.Instance.MonsterPrefabDic.ContainsKey(msg) == false)
            {
                Debug.LogWarning(string.Format("없는 이름으로 Monster 생성 : {0}", msg));
                return null;
            }


            while (true)
            {
                for (int i = 0; i < ListDic[msg].Count; i++)
                {
                    var e = ListDic[msg][i];

                    if (e.gameObject.activeSelf == false)
                    {
                        e.SetMonster(position);
                        return e;
                    }

                }

                IncreasePool(msg, 10);
            }
        }


        #endregion






        #region Awake & Init


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
            }

            #endregion

        }


        public IEnumerator InitRoutine()
        {
            ListDic = new Dictionary<string, List<Monster>>();


            var dic = Database.Instance.MonsterPrefabDic;
            foreach (var item in dic)
            {
                ListDic.Add(item.Key, new List<Monster>());
                IncreasePool(item.Key, 5);
            }
            
            yield return null;
        }


        #endregion

        


        public List<Monster> GetActiveMonster()
        {
            List<Monster> list = new List<Monster>();

            foreach (var item in ListDic)
            {
                foreach (var m in item.Value)
                {
                    if (m.IsAlive)
                    {
                        list.Add(m);
                    }
                }
            }


            return list;
        }


    }
}