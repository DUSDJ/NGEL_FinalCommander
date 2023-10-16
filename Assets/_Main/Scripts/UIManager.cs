using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace FC
{
    public class UIManager : MonoBehaviour
    {

        #region SingleTone

        private static UIManager _instance;

        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UIManager>();

                    if (!_instance)
                    {
                        var prefab = Resources.Load<UIManager>("Manager/UIManager");
                        _instance = Instantiate(prefab);

                        if (!_instance)
                        {
                            Debug.LogError("UIManager is null");
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion


        // Main UI
        // [Header("Sub UIs")]
        [HideInInspector] public InventoryUI InventoryUI;







        #region Global Fade



        [Header("Global Fade")]
        public Image FadeImage;


        public void InstantFade(bool trueIsIn)
        {
            Color c = FadeImage.color;
            // FadeIn (1 -> 0)
            if (trueIsIn)
            {
                c.a = 0;
                FadeImage.color = c;

                FadeImage.gameObject.SetActive(false);
            }
            // FadeOut (0 -> 1)
            else
            {
                c.a = 1;
                FadeImage.color = c;
                FadeImage.gameObject.SetActive(true);
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

            InventoryUI = FindObjectOfType<InventoryUI>(true);
            InventoryUI.Init();



            yield return null;
        }


        #endregion


    }
}