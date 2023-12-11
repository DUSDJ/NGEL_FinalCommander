using UnityEditor;
using UnityEngine;

namespace FC
{
    public class StageUI : MonoBehaviour
    {
        public void OnClickStage0()
        {
            if(GameManager.Instance.IsInit == false)
            {
                return;
            }


            if (GameManager.Instance.IsTutorialClear)
            {
                GameManager.Instance.StartGame();

                UIManager.Instance.TutorialUI.SetUI(false);
            }
            else
            {
                UIManager.Instance.TutorialUI.SetUI(true);
            }
        }
    }
}