using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FC
{

    public class EffectManager : MonoBehaviour
    {


        #region SingleTon
        /* SingleTon */
        private static EffectManager _instance;
        public static EffectManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<EffectManager>();

                    if (!_instance)
                    {
                        var prefab = Resources.Load<EffectManager>("Manager/EffectManager");
                        _instance = Instantiate(prefab);

                        if (!_instance)
                        {
                            Debug.LogError("EffectManager is null");
                        }
                    }
                }

                return _instance;
            }

        }

        #endregion

        public Dictionary<string, Effect> EffectDic;
        public Dictionary<string, List<Effect>> ListDic;



        private bool isInit = false;

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
            if (isInit)
            {
                yield break;
            }

            Debug.Log("=====EffectManager Init=====");

            EffectDic = new Dictionary<string, Effect>();
            ListDic = new Dictionary<string, List<Effect>>();

            #region Resources Load

            Effect[] effects = Resources.LoadAll<Effect>("Effect");
            for (int i = 0; i < effects.Length; i++)
            {
                Debug.LogFormat("EffectDic Add : {0}", effects[i].name);
                EffectDic.Add(effects[i].name, effects[i]);
                ListDic.Add(effects[i].name, new List<Effect>());
            }

            #endregion

            isInit = true;
        }


        public void IncreasePool(string key, int num)
        {
            for (int i = 0; i < num; i++)
            {
                Effect e = Instantiate(EffectDic[key]);
                e.transform.SetParent(transform, false);
                ListDic[key].Add(e);
                e.gameObject.SetActive(false);
            }
        }

        public Effect SetEffect(string msg, Vector3 position)
        {
            if (EffectDic.ContainsKey(msg) == false)
            {
                Debug.LogWarning(string.Format("없는 이름으로 Effect 생성 : {0}", msg));
                return null;
            }


            while (true)
            {
                for (int i = 0; i < ListDic[msg].Count; i++)
                {
                    Effect e = ListDic[msg][i];

                    if (e.gameObject.activeSelf == false)
                    {
                        e.gameObject.SetActive(true);
                        e.SetEffect(position);

                        return e;
                    }

                }

                IncreasePool(msg, 3);
            }
        }

        public Effect SetEffect(string msg, Vector3 position, Quaternion rotation)
        {
            if (EffectDic.ContainsKey(msg) == false)
            {
                Debug.LogWarning(string.Format("없는 이름으로 Effect 생성 : {0}", msg));
                return null;
            }

            while (true)
            {
                for (int i = 0; i < ListDic[msg].Count; i++)
                {
                    Effect e = ListDic[msg][i];

                    if (e.gameObject.activeSelf == false)
                    {
                        e.gameObject.SetActive(true);
                        e.SetEffect(position, rotation);

                        return e;
                    }

                }

                IncreasePool(msg, 6);
            }
        }


        #region Clean

        public void CleanGame()
        {
            foreach (var item in ListDic)
            {
                var list = item.Value;
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Clean();
                }
            }
        }

        #endregion
    }

}
