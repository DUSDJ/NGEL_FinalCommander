using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace FC
{
    public class ElementLocationSlot : MonoBehaviour
    {
        public Hero Hero;

        [HideInInspector] public ScriptableHeroData Data = null;

        public GameObject SelectFrame;

        public GameObject ActivationObject;

        public Image PortraitImage;
        public Image TierImage;
        public TextMeshProUGUI NameText;

        public TextMeshProUGUI LevelText;
        public Image ElementalImage;

        public Image CampImage;



        public void SetElement(Hero hero)
        {
            if (hero == null || hero.IsAlive == false)
            {
                Clean();
                return;
            }


            Data = hero.Data;

            var db = Database.Instance;

            PortraitImage.sprite = Data.PortraitSprite;
            TierImage.sprite = db.GetTierImage(Data.Tier);

            // NameText.text = heroData.Name;

            LevelText.text = string.Format("Lv.{0}", Data.Level);
            ElementalImage.sprite = db.GetElementalImage(Data.Elemental);

            CampImage.sprite = db.GetCampImage(Data.Camp);

            ActivationObject.SetActive(true);
        }

        public void Clean()
        {
            Hero = null;
            Data = null;
            ActivationObject.SetActive(false);
        }


    }
}