using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MinecraftTutorial
{
    
    public class UIItemSlot : MonoBehaviour
    {
        [Header("Usefull Game Object")]
        public ItemSlot itemSlot;
        public WorldGenerator world;

        [Header("Important Variables")]
        public bool isLinked = false;
        public Image slotImage;
        public Image slotIcon;
        public Text slotAmount;
        public GameObject conditionPlacement;
        RectTransform slotConditionRect;
        public Image condition;

        /* Explanation :
         * 
         * Class item slot is doing that's called "behind the scene" holder of the perticular slot
         * what ever item in this slot will remain in this slot like your inventory or tool belt or
         * the container/saving cheast or where ever location of the slot is
         * this still can hold the data
         * it will not be triggered if we dont look at this inventory
         * 
         * why we divide the UI item set and item slot ? 
         * because we have an array of the item data that we stored inside the inventory we want to make 
         * it flexible to use
         * 
         */

        private void Awake()
        {

            if (world == null)
            {
                world = GameObject.FindGameObjectWithTag("GameManager").GetComponent<WorldGenerator>();
            }

            slotConditionRect = conditionPlacement.gameObject.GetComponent<RectTransform>();

        }

        public bool HasItem
        {
            get
            {

                if (itemSlot == null)
                {
                    return false;
                }
                else
                {
                    return itemSlot.HasItem;
                }

            }
        }

        public void Link(ItemSlot _itemSlot)
        {

            itemSlot = _itemSlot;
            isLinked = true;
            itemSlot.LinkUISlot(this);

            UpdateSlot();

        }

        public void UnLink()
        {

            itemSlot.UnLinkUISlot();
            itemSlot = null;

            UpdateSlot();

        }

        public void UpdateSlot()
        {

            if (itemSlot != null && itemSlot.HasItem)
            {
                //this will be Update all element that we have in Inventory
                slotIcon.sprite = world.blockTypes[itemSlot.stack.blockID].icon;
                slotAmount.text = itemSlot.stack.amountOfItem.ToString();
                slotIcon.enabled = true;
                slotAmount.enabled = true;

            }
            else
            {
                //we'll be clear it 
                Clear();

            }

        }

        public void Clear()
        {

            slotIcon.sprite = null;
            slotAmount.text = "";
            slotIcon.enabled = false;
            slotAmount.enabled = false;

        }

        private void OnDestroy()
        {

            if (isLinked)
            {
                itemSlot.UnLinkUISlot();
            }

        }
        
    }

}