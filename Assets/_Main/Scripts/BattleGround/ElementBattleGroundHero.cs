using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FC
{
    public class ElementBattleGroundHero : MonoBehaviour
    {
        [HideInInspector] public ScriptableHeroData Data = null;

        public GameObject SelectFrame;

        public GameObject ActivationObject;

        public Image PortraitImage;


        [HideInInspector] public ElementInventoryHero MatchedElement = null;



        public void SetElement(ElementInventoryHero elem)
        {
            MatchedElement = elem;

            Data = elem.Data;

            PortraitImage.sprite = Data.PortraitSprite;


            ActivationObject.SetActive(true);
        }




        public void Clean()
        {
            MatchedElement = null;
            Data = null;
            ActivationObject.SetActive(false);
            SelectFrame.SetActive(false);
        }



        public void OnClickElement()
        {
            if(Data != null)
            {
                UIManager.Instance.BattleGroundUI.SubtractHeroFromElement(this);
            }
        }

    }
}