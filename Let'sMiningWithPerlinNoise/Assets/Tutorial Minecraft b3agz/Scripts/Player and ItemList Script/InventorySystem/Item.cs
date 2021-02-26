using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftTutorial
{
    /* note : 
     * This script is changing BlockTypes class, but all of it basically same
     */
    [CreateAssetMenu(fileName = "New Item", menuName = "MinecraftTutorial/Inventory/New Item")]
    public class Item : ScriptableObject
    {

        [Header("Block Description")]
        // Item name, can't be called "name" as clashes with built in Object.name variable.
        public string blockName;
        public bool isSolid;
        public bool renderNeighborFaces;
        public bool isLiquidType;
        [Range(0, 1)]
        public float transparency;
        // The image that will show up in inventory slots.
        public Sprite icon;
        // Description of item, uses TextArea tag to give us more space in the inspector to write in.
        [TextArea]
        public string blockDescription;
        // The maximum of this item that can be placed in one slot.
        public int maxStack;
        // The maximum condition this item can be in. If the item does not degrade, set this value to -1.
        public int maxCondition;
        //The Damage for combat system
        public float weaponDamage;
        //The amount of defense that this item can hold for combat system
        public float defenseAmount;

        // Quick ways of checking if the item is stackable or degradable.
        public bool isStackable { get { return (maxStack > 1); } }
        public bool isDegradable { get { return (maxCondition > -1); } }

        [Header("Texture Values")]
        public int frontFaceTexture;
        public int backFaceTexture;
        public int topFaceTexture;
        public int bottomFaceTexture;
        public int leftFaceTexture;
        public int rightFaceTexture;

        public int getTextureID(int faceIndex)
        {
            switch (faceIndex)
            {
                case 0:
                    {
                        return frontFaceTexture;
                    }
                case 1:
                    {
                        return backFaceTexture;
                    }
                case 2:
                    {
                        return topFaceTexture;
                    }
                case 3:
                    {
                        return bottomFaceTexture;
                    }
                case 4:
                    {
                        return leftFaceTexture;
                    }
                case 5:
                    {
                        return rightFaceTexture;
                    }
                default:
                    {
                        Debug.Log("Error in GetTextureID; Invalid face Index");
                        return 0;
                    }

            }
        }

    }


}