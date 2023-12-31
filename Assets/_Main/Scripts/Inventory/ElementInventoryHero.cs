﻿using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace FC
{
    public class ElementInventoryHero : MonoBehaviour
    {
        [HideInInspector]public ScriptableHeroData Data = null;

        public GameObject SelectFrame;

        public GameObject ActivationObject;

        public Image PortraitImage;
        public Image TierImage;
        public TextMeshProUGUI NameText;

        public TextMeshProUGUI LevelText;
        public Image ElementalImage;
        
        public Image CampImage;
        


        public void SetElement(ScriptableHeroData heroData)
        {
            Data = heroData;

            var db = Database.Instance;

            PortraitImage.sprite = heroData.PortraitSprite;
            TierImage.sprite = db.GetTierImage(heroData.Tier);

            // NameText.text = heroData.Name;

            LevelText.text = string.Format("Lv.{0}", heroData.Level);
            ElementalImage.sprite = db.GetElementalImage(heroData.Elemental);

            CampImage.sprite = db.GetCampImage(heroData.Camp);

            ActivationObject.SetActive(true);            
        }


        public void SetSelect(bool onOff)
        {
            if (onOff)
            {
                SelectFrame.SetActive(true);
            }
            else
            {
                SelectFrame.SetActive(false);
            }
        }


        public void Clean()
        {
            Data = null;
            ActivationObject.SetActive(false);
            SelectFrame.SetActive(false);
        }



        public void OnClickElement()
        {
            if(Data == null)
            {
                return;
            }


            if (UIManager.Instance.NowState == EnumUIState.WorldMap)
            {
                var parent = UIManager.Instance.InventoryUI;

                if (SelectFrame.activeSelf == true)
                {
                    parent.CleanSelection();
                }
                else
                {
                    parent.CheckHeroCanThreeMerge(this);
                }


            }
            else if(UIManager.Instance.NowState == EnumUIState.BattleGround)
            {
                if (GameManager.Instance.NowGameState != EnumGameState.OutGame)
                {
                    return;
                }

                var parent = UIManager.Instance.BattleGroundUI;
                parent.AddHeroToElement(this);
            }            
        }
    }
}