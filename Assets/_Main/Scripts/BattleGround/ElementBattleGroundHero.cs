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


        public void SetElement(Hero hero)
        {
            MatchedElement = null;

            Data = hero.Data;

            PortraitImage.sprite = Data.PortraitSprite;

            ActivationObject.SetActive(true);
            gameObject.SetActive(true);
        }


        public void SetElement(ElementInventoryHero elem)
        {
            MatchedElement = elem;

            Data = elem.Data;

            PortraitImage.sprite = Data.PortraitSprite;


            ActivationObject.SetActive(true);
            gameObject.SetActive(true);
        }




        public void Clean()
        {
            MatchedElement = null;
            Data = null;
            ActivationObject.SetActive(false);
            SelectFrame.SetActive(false);
        }

        public void Disable()
        {
            Clean();

            gameObject.SetActive(false);
        }



        public void OnClickElement()
        {
            if(GameManager.Instance.NowGameState == EnumGameState.Battle)
            {
                return;
            }

            if(Data != null)
            {
                if(MatchedElement != null)
                {
                    UIManager.Instance.BattleGroundUI.SubtractHeroFromElement(this);
                }
                else
                {
                    // 슬롯 빈 자리 있으면 Add
                    var e = UIManager.Instance.InventoryUI.GetEmptyElement();
                    if(e != null)
                    {
                        UIManager.Instance.InventoryUI.AddHero(Data);

                        // Add 후 이 오브젝트는 Clean
                        Clean();

                        // 해당 스토리지 저장
                        UIManager.Instance.BattleGroundUI.CleanHeroes();

                        UIManager.Instance.BattleGroundUI.SelectedLocation.SetHero(
                            UIManager.Instance.BattleGroundUI.StoreHeroList()
                            );

                        UIManager.Instance.BattleGroundUI.CleanElements();
                        UIManager.Instance.BattleGroundUI.UpdateHero();
                    }
                    // 없으면 Alert
                    else
                    {
                        UIManager.Instance.AlertUI.SetTextMiddleRed("슬롯에 빈 자리가 없습니다.");
                    }                    
                }
                
            }
        }

    }
}