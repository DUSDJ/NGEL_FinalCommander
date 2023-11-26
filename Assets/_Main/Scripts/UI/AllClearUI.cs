using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace FC
{
    public class AllClearUI : MonoBehaviour
    {
        public Transform Frame;

        public void Init()
        {
            SetUI(false);
        }

        public void SetUI(bool onOff)
        {
            if (onOff)
            {
                gameObject.SetActive(true);
                StartCoroutine(Routine());
            }
            else
            {
                gameObject.SetActive(false);
            }
        }


        private IEnumerator Routine()
        {
            Frame.transform.localPosition = new Vector2(0, 1920f);
            var tw = Frame.transform.DOLocalMoveY(0, 2.0f).SetEase(Ease.OutSine).SetUpdate(true);
            yield return tw.WaitForCompletion();

        }



        public void OnClickQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}