using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

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
        [HideInInspector] public WorldMapUI WorldMapUI;
        [HideInInspector] public InventoryUI InventoryUI;  
        [HideInInspector] public BattleGroundUI BattleGroundUI;




        #region UI State

        public EnumUIState NowState = EnumUIState.WorldMap;

        public void SetState(EnumUIState nextState)
        {
            ExitState(NowState);

            NowState = nextState;

            InitState(nextState);
        }

        private void InitState(EnumUIState state)
        {
            switch (state)
            {
                case EnumUIState.WorldMap:
                    WorldMapUI.SetUI(true);
                    BattleGroundUI.SetUI(false);

                    InventoryUI.SetStateWorldMap();
                    break;


                case EnumUIState.BattleGround:
                    WorldMapUI.SetUI(false);
                    BattleGroundUI.SetUI(true);

                    InventoryUI.SetStateBattleGround();
                    break;
            }
        }

        private void ExitState(EnumUIState state)
        {

        }



        #endregion










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

            WorldMapUI = FindObjectOfType<WorldMapUI>(true);
            WorldMapUI.Init();
            
            InventoryUI = FindObjectOfType<InventoryUI>(true);
            InventoryUI.Init();


            BattleGroundUI = FindObjectOfType<BattleGroundUI>(true);
            BattleGroundUI.Init();


            SetState(EnumUIState.WorldMap);

            yield return null;
        }


        #endregion
        

    }
}