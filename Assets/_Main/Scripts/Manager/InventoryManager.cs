using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace FC
{
    public class InventoryManager : MonoBehaviour
    {

        #region SingleTone

        private static InventoryManager _instance;

        public static InventoryManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<InventoryManager>();

                    if (!_instance)
                    {
                        var prefab = Resources.Load<InventoryManager>("Manager/InventoryManager");
                        _instance = Instantiate(prefab);

                        if (!_instance)
                        {
                            Debug.LogError("InventoryManager is null");
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion




        #region Values

        public Dictionary<int, int> Inventory = new Dictionary<int, int>();


        #endregion











        #region Awake & Init


        private void Awake()
        {
            #region SingleTon

            if (Instance == this)
            {

            }
            else
            {
                Destroy(gameObject);
            }

            #endregion

        }


        public IEnumerator InitRoutine()
        {
            Inventory = new Dictionary<int, int>();

            yield return null;
        }


        #endregion


    }
}