using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FC
{
    public class LoadingManager : MonoBehaviour
    {
        public static int nextSceneNumber;
        // public static string LoadingText;

        public Image ProgressBar;
        public TextMeshProUGUI TextBox;


        #region Title Camera Manager

        public Camera LoadingMainCamera;

        void OnPreCull() => GL.Clear(true, true, Color.black);


        public void CheckCanvas()
        {
            // Odin Button으로 에디터에서 접근하면 버그있음 주의

            Rect rect = LoadingMainCamera.rect;

            // (가로 / 세로)
            float scaleHeight = ((float)Screen.width / Screen.height) / ((float)1080 / 1920);
            float scaleWidth = 1f / scaleHeight;

            Debug.Log($"Screen.width = {Screen.width}");
            Debug.Log($"Screen.height = {Screen.height}");

            Debug.Log($"scaleHeight = {scaleHeight}");
            Debug.Log($"scaleWidth = {scaleWidth}");

            if (scaleHeight == 1)
            {
                Debug.Log("scaleHeight = 1");
                return;
            }

            if (scaleHeight < 1)
            {
                Debug.Log($"rect.y = {(1f - scaleHeight) / 2f}");

                rect.width = 1f;
                rect.x = 0f;

                rect.height = scaleHeight;
                rect.y = (1f - scaleHeight) / 2f;
            }
            else
            {
                rect.width = scaleWidth;
                rect.x = (1f - scaleWidth) / 2f;

                rect.height = 1f;
                rect.y = 0f;
            }
            LoadingMainCamera.rect = rect;

            //var UIRect = TitleMainCamera.rect;
            //UIRect.width = scaleWidth;
            //UIRect.x = (1f - scaleWidth) / 2f;

            //UIRect.height = scaleHeight;

            //TitleMainCamera.rect = UIRect;
        }


        #endregion



        public static void LoadScene(int sceneNumber, string loadingText)
        {
            // LoadingText = loadingText;
            nextSceneNumber = sceneNumber;

            SceneManager.LoadScene(1);
        }

        public static void LoadScene(int sceneNumber)
        {
            // LoadingText = "Now Loading...";
            nextSceneNumber = sceneNumber;

            SceneManager.LoadScene(1);
        }




        private void Start()
        {
            CheckCanvas();

            StartCoroutine(LoadScene());
        }



        public IEnumerator LoadScene()
        {
            ProgressBar.fillAmount = 0f;
            TextBox.text = "0%";


            AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneNumber);
            op.allowSceneActivation = false;

            float timer = 0.0f;
            while (!op.isDone)
            {
                yield return null;

                timer += Time.deltaTime;

                if (op.progress < 0.9f)
                {
                    ProgressBar.fillAmount = Mathf.Lerp(ProgressBar.fillAmount, op.progress, timer);
                    TextBox.text = string.Format("{0:0.##}%", ProgressBar.fillAmount * 100);
                    if (ProgressBar.fillAmount >= op.progress)
                    {
                        timer = 0f;
                    }
                }
                else
                {
                    ProgressBar.fillAmount = Mathf.Lerp(ProgressBar.fillAmount, 1f, timer);
                    TextBox.text = string.Format("{0:0.##}%", ProgressBar.fillAmount * 100);
                    if (ProgressBar.fillAmount == 1.0f)
                    {
                        op.allowSceneActivation = true;
                        yield break;
                    }
                }
            }

        }




    }
}

