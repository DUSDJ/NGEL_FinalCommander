using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

namespace FC
{
    public class AlertUI : MonoBehaviour
    {

        public Transform AlertTransform;

        public TextMeshProUGUI TopText;
        public TextMeshProUGUI BottomText;

        public TextMeshProUGUI MiddleText;



        [Serializable]
        public struct StructAlertColor
        {
            public Color BaseWhite;
            public Color ContentBlue;
            public Color GemPink;
            public Color AlertRed;

        }
        public StructAlertColor AlertColor;


        public float ScailingInTime = 0.3f; // ÆîÃÄÁú ¶§
        public float ScailingOutTime = 0.3f; // Á¢Èú ¶§
        public float WaitTime = 1.0f; // ÆîÃÄÁü -> "´ë±â" -> Á¢Èû


        private WaitForSecondsRealtime wait;
        private IEnumerator routine;
        private Tween tw;



        #region Init

        public void Init()
        {
            AlertTransform.gameObject.SetActive(false);
            TopText.gameObject.SetActive(false);
            BottomText.gameObject.SetActive(false);
            MiddleText.gameObject.SetActive(false);

            wait = new WaitForSecondsRealtime(WaitTime);
        }

        #endregion




        public void SetText(string top, Color topColor, string bottom, Color bottomColor)
        {
            TopText.gameObject.SetActive(true);
            BottomText.gameObject.SetActive(true);
            MiddleText.gameObject.SetActive(false);

            TopText.text = top;
            TopText.color = topColor;

            BottomText.text = bottom;
            BottomText.color = bottomColor;

            StartRoutine();
        }

        public void SetTextMiddle(string middle, Color middleColor)
        {
            TopText.gameObject.SetActive(false);
            BottomText.gameObject.SetActive(false);
            MiddleText.gameObject.SetActive(true);

            MiddleText.text = middle;
            MiddleText.color = middleColor;

            StartRoutine();
        }


        public void SetTextMiddleBlue(string middle)
        {
            TopText.gameObject.SetActive(false);
            BottomText.gameObject.SetActive(false);
            MiddleText.gameObject.SetActive(true);

            MiddleText.text = middle;
            MiddleText.color = AlertColor.ContentBlue;

            StartRoutine();
        }



        public void SetTextMiddleRed(string msg)
        {
            TopText.gameObject.SetActive(false);
            BottomText.gameObject.SetActive(false);
            MiddleText.gameObject.SetActive(true);

            MiddleText.text = msg;
            MiddleText.color = AlertColor.AlertRed;

            StartRoutine();
        }




        private void StartRoutine()
        {
            StopRoutine();


            routine = AlertRoutine();
            StartCoroutine(routine);
        }

        private void StopRoutine()
        {
            if (routine != null)
            {
                StopCoroutine(routine);
            }
            routine = null;
        }




        private IEnumerator AlertRoutine()
        {
            if (tw != null)
            {
                tw.Kill();
            }

            AlertTransform.gameObject.SetActive(true);

            // ÆîÃÄÁü -> "´ë±â" -> Á¢Èû
            AlertTransform.localScale = new Vector2(AlertTransform.localScale.x, 0);
            tw = AlertTransform.DOScaleY(1, ScailingInTime);

            yield return tw.WaitForCompletion();

            yield return wait;

            tw = AlertTransform.DOScaleY(0, ScailingOutTime);
            yield return tw.WaitForCompletion();


            AlertTransform.gameObject.SetActive(false);
        }



    }
}