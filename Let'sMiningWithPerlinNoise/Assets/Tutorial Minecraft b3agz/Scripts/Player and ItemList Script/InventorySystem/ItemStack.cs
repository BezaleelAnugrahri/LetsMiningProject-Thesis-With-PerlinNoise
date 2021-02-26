using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftTutorial
{
    //just holding perticular item ID and amount of item
    public class ItemStack
    {
        [Header("Item List Variable")]
        public byte blockID;
        public int amountOfItem;

        public ItemStack(byte _id, int _amount)
        {

            blockID = _id;
            amountOfItem = _amount;

        }

    }

}