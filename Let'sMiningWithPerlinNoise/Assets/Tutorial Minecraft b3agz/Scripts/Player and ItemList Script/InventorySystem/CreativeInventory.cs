using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftTutorial
{
    public class CreativeInventory : MonoBehaviour
    {

        [Header("GameObject Reference")]
        public GameObject slotPrefabs;
        public Player player;
        public WorldGenerator world;
        public GameObject creativeInventory;
        [SerializeField]
        GameObject[] bag;

        [Header("List and Queue")]
        List<ItemSlot> slots = new List<ItemSlot>();



        private void Awake()
        {

            if (!player.settings.enableSurvivalMode)
            {
                CreativeInventorySystem();
            }
            else
            {

                bag = new GameObject[player.bagSlotAmount];
                SurvivalInventorySystem();
                /*for (int i = 0; i < player.settings.inventoryData.Count; i++)
                {
                    
                    byte indexBlock = player.settings.inventoryData[i];
                    int amountOfItem = player.settings.amount[i];

                    ItemStack stack = new ItemStack(indexBlock, amountOfItem);

                    slots.Add(new ItemSlot(new UIItemSlot(), stack));
    
                }*/

            }

        }

        private void Start()
        {

            

        }

        private void Update()
        {

            /*if (player.settings.enableSurvivalMode)
            {

                if (player.InUI)
                {
                    //OpenSurvivalInventory(slots);
                }
                else
                {
                    //CloseSurvivalInventory();
                }

            }*/
               
        }


        public void CreativeInventorySystem()
        {

            //creative mode
            //block id index, starting by 1 because 0 is Air block
            for (int i = 1; i < world.blockTypes.Length; i++)
            {

                GameObject newSlot = Instantiate(slotPrefabs, creativeInventory.gameObject.transform);

                ItemStack stack = new ItemStack((byte)i, 99);
                ItemSlot slot = new ItemSlot(newSlot.GetComponent<UIItemSlot>(), stack);

                slot.isCreative = true;

            }


        }


        private void SurvivalInventorySystem()
        {
            //survival mode

            for (int i = 0; i < player.bagSlotAmount; i++)
            {

                bag[i] = Instantiate(slotPrefabs, creativeInventory.gameObject.transform);

                if (i < player.settings.inventoryData.Count)
                {

                    ItemStack stack = new ItemStack((byte)player.settings.inventoryData[i], player.settings.amount[i]);
                    ItemSlot slot = new ItemSlot(bag[i].GetComponent<UIItemSlot>(), stack);

                    slot.isCreative = true;

                }
                else
                {

                    ItemSlot slot = new ItemSlot(bag[i].GetComponent<UIItemSlot>());

                    slot.isCreative = true;

                }

            }

            

        }


        private void OpenSurvivalInventory(List<ItemSlot> equipmentSlot)
        {



        }

        private void CloseSurvivalInventory()
        {



        }

    }

}