﻿using System;
using System.Collections;
using UnityEngine;

namespace FC
{
    public class GameManager : MonoBehaviour
    {

        #region SingleTone

        private static GameManager _instance;

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameManager>();

                    if (!_instance)
                    {
                        var prefab = Resources.Load<GameManager>("Manager/GameManager");
                        _instance = Instantiate(prefab);

                        if (!_instance)
                        {
                            Debug.LogError("GameManager is null");
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion



        [Header("인게임 BGM")]
        public string BGMKey;

        private IEnumerator mainRoutine;

        private bool isInit = false;



        [Header("골드")]
        public long gold = 10000;
        public long Gold
        {
            get
            {
                return gold;
            }
            set 
            {
                gold = value;
                UIManager.Instance?.InventoryUI?.UpdateGold(value);
            }
        }

        [Header("초당 획득 골드")]
        public long Income = 30;        


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
                return;
            }

            #endregion

            StartCoroutine(InitRoutine());
        }


        private IEnumerator InitRoutine()
        {
            // 글로벌 화면 즉시 암전
            UIManager.Instance.InstantFade(false);

            // Load Manager
            /* DataBase */
            yield return Database.Instance.InitRoutine();

            /* Audio Manager */
            yield return AudioManager.Instance.InitRoutine();

            /* Effect Manager */
            yield return EffectManager.Instance.InitRoutine();

            /* UI Manager */
            yield return UIManager.Instance.InitRoutine();




            // 글로벌 즉시 암전 해제
            UIManager.Instance.InstantFade(true);



            // Field 
            mainRoutine = MainRoutine();
            StartCoroutine(mainRoutine);

            Debug.Log("Init End!");

            yield return null;
        }

        private IEnumerator MainRoutine()
        {
            var wait = new WaitForSeconds(1.0f);

            while (true)
            {
                Debug.Log(DateTime.Now.ToString("HH:mm:ss"));

                Gold += Income;
                yield return wait;
            }

        }
        
        #endregion




    }
}