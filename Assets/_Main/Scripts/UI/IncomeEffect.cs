using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FC
{
    public class IncomeEffect : MonoBehaviour
    {
        public Animator anim;
        public TextMeshProUGUI IncomeText;

        public void SetEffect(string msg)
        {
            gameObject.SetActive(true);

            anim.SetTrigger("Action");

            if (IncomeText != null) {
                IncomeText.text = msg;
            }
        }


        public void Clean()
        {
            gameObject.SetActive(false);
        }
    }
}