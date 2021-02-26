using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MinecraftTutorial
{

    public class ToolBar : MonoBehaviour
    {
        [Header("GameObject Reference")]
        public GameObject toolbar;
        public WorldGenerator world;
        public Player player;

        [Header("UI Element")]
        public RectTransform highlight;
        public UIItemSlot[] slots;

        [Header("Variable")]
        public int slotIndex = 0;

        private void Awake()
        {

            if (player.settings.enableSurvivalMode)
            {
                //survival mode
                GetItemIconSurvivalMode();

            }
            else
            {
                //creative mode 
                GetItemIconCreativeMode();
                
            }
                                    
        }

        private void Update()
        {

            ScrollAbleMenuItem("Mouse ScrollWheel");

        }

        

        void ScrollAbleMenuItem(string keyName)
        {

            float scroll = Input.GetAxis(keyName);
            if (scroll != 0)
            {

                if (scroll > 0)
                {
                    slotIndex--;
                }
                else
                {
                    slotIndex++;
                }

                if (slotIndex > slots.Length - 1)
                {
                    slotIndex = 0;
                } 
                if (slotIndex < 0)
                {
                    slotIndex = slots.Length - 1;
                }

                //we want to moving highlight image position 
                highlight.position = slots[slotIndex].slotIcon.transform.position;
                //player.selectedBlockIndex = slots[slotIndex].itemSlot.stack.blockID;

            }

        }

        void GetItemIconCreativeMode()
        {

            foreach (UIItemSlot ss in slots)
            {
                ItemSlot slot = new ItemSlot(ss);
                //slot.isCreative = true;
            }

        }

        void GetItemIconSurvivalMode()
        {

            foreach (UIItemSlot ss in slots)
            {
                ItemSlot slot = new ItemSlot(ss);
                slot.isCreative = true;
            }

            //later on
            //if it dont have any item inside inventoryData, then all empty
            //else if it have we need to send some item into toolbar 
            //then we send the other inventoryData inside bag.


        }


    }

}