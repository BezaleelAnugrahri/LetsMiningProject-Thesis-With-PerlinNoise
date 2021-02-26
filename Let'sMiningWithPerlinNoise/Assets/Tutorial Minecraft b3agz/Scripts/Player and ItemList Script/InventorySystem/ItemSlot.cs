using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftTutorial
{
    public class ItemSlot
    {
        #region Survival mode Inventory system ...
        
        #endregion 

        #region Creative mode inventory system ...
        [Header("Creative mode inventory")]
        //and this is where we can know which item that have amount of that item
        public ItemStack stack = null;
        private UIItemSlot uiItemSlot = null;

        public bool isCreative;

        public ItemSlot(UIItemSlot _uiItemSlot)
        {

            stack = null;
            uiItemSlot = _uiItemSlot;
            uiItemSlot.Link(this);

        }

        public ItemSlot(UIItemSlot _uiItemSlot, ItemStack _stack)
        {

            stack = _stack;
            uiItemSlot = _uiItemSlot;
            uiItemSlot.Link(this);

        }

        public void LinkUISlot(UIItemSlot uiSlot)
        {

            uiItemSlot = uiSlot;

        }

        public void UnLinkUISlot()
        {

            uiItemSlot = null;

        }

        public void EmptySlot()
        {

            stack = null;
            if (uiItemSlot != null)
            {
                uiItemSlot.UpdateSlot();
            }

        }

        public int Take(int amt)
        {
            int _amt;

            //trake an ammount from EmptySlot() function
            if (amt > stack.amountOfItem)
            {

                _amt = stack.amountOfItem;
                EmptySlot();
                return _amt;

            }
            else if (amt < stack.amountOfItem)
            {

                stack.amountOfItem -= amt;
                uiItemSlot.UpdateSlot();
                return amt;

            }
            else
            {

                EmptySlot();
                return amt;

            }

        }

        public ItemStack TakeAll()
        {
            ItemStack handOver = new ItemStack(stack.blockID, stack.amountOfItem);
            EmptySlot();
            return handOver;
        }

        public ItemStack TakeAmount(int amnt)
        {
           
            int _amt;

            //trake an ammount from EmptySlot() function
            if (amnt > stack.amountOfItem)
            {
                _amt = stack.amountOfItem;
                ItemStack handOver = new ItemStack(stack.blockID, _amt);

                EmptySlot();
                return handOver;

            }
            else if (amnt < stack.amountOfItem)
            {

                stack.amountOfItem -= amnt;
                ItemStack handOver = new ItemStack(stack.blockID, amnt);

                uiItemSlot.UpdateSlot();
                return handOver;

            }
            else
            {

                EmptySlot();
                ItemStack handOver = new ItemStack(0, 0);
                return handOver;

            }

        }

        public void InsertStack(ItemStack _stack)
        {
            stack = _stack;
            uiItemSlot.UpdateSlot();
        }

        public bool HasItem
        {

            get
            {

                if (stack != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

        }

        #endregion

    }

}
