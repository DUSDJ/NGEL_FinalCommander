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
            // Ÿ��Ʋ �������� �۵�
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
            

            // ��� �ڵ� ������� Off
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            // Text Init
            // Ÿ��Ʋ�ؽ�Ʈ On
            TitleSceneObjects.TitleTextObject.SetActive(true);
            TitleSceneObjects.TitleText.alpha = 0f;
            TitleSceneObjects.TitleText.DOFade(1f, 1.0f);
            SetLoadingText(ConstText.TitleLoading);


            // Load Manager (Not Login Data)
            yield return LoadManager();


            // ��Ī! & BGM ���
            TitleSceneObjects.LoginCompleteImage.gameObject.SetActive(true);
            TitleSceneObjects.LoginCompleteImage.DOFade(1.0f, 0.05f).OnComplete(() => {
                TitleSceneObjects.LoginCompleteImage.DOFade(0f, 0.2f).OnComplete(() => {
                    TitleSceneObjects.LoginCompleteImage.gameObject.SetActive(false);
                });
            });
            
            AudioManager.Instance.SetBGM(BGMKey);


            // ���� ���� ��ư On            
            TitleSceneObjects.BtnGameStart.SetActive(true);

            // Ÿ��Ʋ�ؽ�Ʈ Off
            SetLoadingText(string.Empty);
            TitleSceneObjects.TitleTextObject.SetActive(false);

            // Ÿ��Ʋ�� ��� �۾� ��
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

            // �ΰ������� �̵� (�ε��� ����)
            LoadingManager.LoadScene(2);

        }



        #endregion

    }
}