using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FC
{

    public class TitleManager : MonoBehaviour
    {

        #region SingleTone

        private static TitleManager _instance;

        public static TitleManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<TitleManager>();

                    if (!_instance)
                    {
                        Debug.LogError("TitleManager is null");
                    }
                }

                return _instance;
            }
        }

        #endregion



        [Header("BGM")]
        public string BGMKey;


        [Serializable]
        public struct StructTitleScene
        {
            public Image LoginCompleteImage;

            public GameObject TitleTextObject;
            public TextMeshProUGUI TitleText;


            public GameObject BtnGameStart;
        }
        public StructTitleScene TitleSceneObjects;





        public static bool loadingEndTrigger = false;




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


        private void Start()
        {
            // 타이틀 씬에서만 작동
            Init();
        }


        private void Init()
        {
            StartCoroutine(InitRoutine());
        }


        #endregion



        #region Load

        private void SetLoadingText(string str)
        {
            TitleSceneObjects.TitleText.text = str;
        }




        private IEnumerator InitRoutine()
        {
            

            // 기기 자동 절전모드 Off
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            // Text Init
            // 타이틀텍스트 On
            TitleSceneObjects.TitleTextObject.SetActive(true);
            TitleSceneObjects.TitleText.alpha = 0f;
            TitleSceneObjects.TitleText.DOFade(1f, 1.0f);
            SetLoadingText(ConstText.TitleLoading);


            // Load Manager (Not Login Data)
            yield return LoadManager();


            // 파칭! & BGM 재생
            TitleSceneObjects.LoginCompleteImage.gameObject.SetActive(true);
            TitleSceneObjects.LoginCompleteImage.DOFade(1.0f, 0.05f).OnComplete(() => {
                TitleSceneObjects.LoginCompleteImage.DOFade(0f, 0.2f).OnComplete(() => {
                    TitleSceneObjects.LoginCompleteImage.gameObject.SetActive(false);
                });
            });
            
            AudioManager.Instance.SetBGM(BGMKey);


            // 게임 시작 버튼 On            
            TitleSceneObjects.BtnGameStart.SetActive(true);

            // 타이틀텍스트 Off
            SetLoadingText(string.Empty);
            TitleSceneObjects.TitleTextObject.SetActive(false);

            // 타이틀씬 모든 작업 끝
            loadingEndTrigger = true;
        }




        public IEnumerator LoadManager()
        {

            /* DataBase  */
            yield return Database.Instance.InitRoutine();

            /* Audio Manager */
            yield return AudioManager.Instance.InitRoutine();

            /* Effect Manager */
            yield return EffectManager.Instance.InitRoutine();


            // Sound Setting Local Load
            // LocalLoadSoundSetting();
            AudioManager.Instance.SetBGMVolume(0.5f);
        }




        #endregion



        #region OnClick


        public void OnClickSceneLoad()
        {
            if (!loadingEndTrigger)
            {
                return;
            }

            DOTween.Clear();

            // 인게임으로 이동 (로딩씬 경유)
            LoadingManager.LoadScene(2);

        }



        #endregion

    }
}