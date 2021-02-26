using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MinecraftTutorial
{

    public class DragAndDropItem : MonoBehaviour
    {
        [Header("Important Game Object")]
        [SerializeField]
        private UIItemSlot cursorSlot = null;
        private ItemSlot cursorItemSlot;
        public Player player;

        [Header("Other Important Things")]
        [SerializeField]
        private GraphicRaycaster m_Raycaster = null;
        private PointerEventData m_PointerEventData;
        [SerializeField]
        private EventSystem m_EventSystem = null;

        private void Start()
        {

            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            }

            cursorItemSlot = new ItemSlot(cursorSlot);
                       
        }

        private void Update()
        {

            KeyBinding();
        
        }

        void KeyBinding()
        {
            
            if (!player.InUI)
            {
                return;
            }

            cursorSlot.transform.position = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                
                //enable survival mode or creative mode
                if (!player.settings.enableSurvivalMode)
                {   
                    HandleSlotClick(CheckForSlot());
                }
                else
                {
                    HandleSurvivalSlotClick(CheckForSlot());
                }

            }

            if (Input.GetMouseButtonDown(1))
            {
                //enable survival mode or creative mode
                if (player.settings.enableSurvivalMode)
                {

                    HandleSlotRightClick(CheckForSlot());

                }
                
            }

        }

        private void HandleSlotRightClick(UIItemSlot clickedSlot)
        {
            //for taking the item 1 by 1 
            if (!cursorSlot.HasItem && clickedSlot.HasItem)
            {
                
                //Debug.Log("We got : " + clickedSlot.itemSlot.stack.amountOfItem + " Item : " + clickedSlot.itemSlot.stack.blockID);
                cursorItemSlot.InsertStack(clickedSlot.itemSlot.TakeAmount(1));
                return;

            }
            
            if (cursorSlot.HasItem && clickedSlot.HasItem)
            {
                
                if (cursorItemSlot.stack.blockID == clickedSlot.itemSlot.stack.blockID)
                {
                    cursorItemSlot.InsertStack(clickedSlot.itemSlot.TakeAmount(1));
                }

            }
            
        }

        private void HandleSlotClick(UIItemSlot clickedSlot)
        {

            if (clickedSlot == null)
            {
                return;
            }

            if (!cursorSlot.HasItem && !clickedSlot.HasItem)
            {
                return;
            }
            
            //if item slot is creative mode 
            //then copy all of item inside cursor slot
            if (clickedSlot.itemSlot.isCreative)
            {

                cursorItemSlot.EmptySlot();
                cursorItemSlot.InsertStack(clickedSlot.itemSlot.stack);

            }

            if (!cursorSlot.HasItem && clickedSlot.HasItem)
            {

                cursorItemSlot.InsertStack(clickedSlot.itemSlot.TakeAll());
                return;

            }

            if (cursorSlot.HasItem && !clickedSlot.HasItem)
            {

                clickedSlot.itemSlot.InsertStack(cursorItemSlot.TakeAll());
                return;

            }

            if (cursorSlot.HasItem && clickedSlot.HasItem)
            {

                if (cursorSlot.itemSlot.stack.blockID != clickedSlot.itemSlot.stack.blockID)
                {
                    /*
                     * so pick up item from slot and then replace it to slot but if it has an item in it
                     * it will swap it into the item that inside the slot
                     * and put the previous item that dragged into the slot
                     */
                    ItemStack oldCursorItemEmplacement = cursorSlot.itemSlot.TakeAll();
                    ItemStack oldSlot = clickedSlot.itemSlot.TakeAll();

                    clickedSlot.itemSlot.InsertStack(oldCursorItemEmplacement);
                    cursorSlot.itemSlot.InsertStack(oldSlot);

                }

            }

        }
        
        private void HandleSurvivalSlotClick(UIItemSlot clickedSlot)
        {
            if (clickedSlot == null)
            {
                return;
            }

            if (!cursorSlot.HasItem && !clickedSlot.HasItem)
            {
                return;
            }

            //if item slot is Adventure/Survival mode 
            //then Take all of item that was clicked to cursor slot
            if (clickedSlot.itemSlot.isCreative && !cursorSlot.HasItem)
            {

                cursorItemSlot.InsertStack(clickedSlot.itemSlot.TakeAll());
                return;

            }

            if (!cursorSlot.HasItem && clickedSlot.HasItem)
            {

                cursorItemSlot.InsertStack(clickedSlot.itemSlot.TakeAll());
                return;

            }

            if (cursorSlot.HasItem && !clickedSlot.HasItem)
            {

                clickedSlot.itemSlot.InsertStack(cursorItemSlot.TakeAll());
                return;

            }

            if (cursorSlot.HasItem && clickedSlot.HasItem)
            {

                if (cursorSlot.itemSlot.stack.blockID != clickedSlot.itemSlot.stack.blockID)
                {
                    /*
                     * so pick up item from slot and then replace it to slot but if it has an item in it
                     * it will swap it into the item that inside the slot
                     * and put the previous item that dragged into the slot
                     */
                    ItemStack oldCursorItemEmplacement = cursorSlot.itemSlot.TakeAll();
                    ItemStack oldSlot = clickedSlot.itemSlot.TakeAll();

                    clickedSlot.itemSlot.InsertStack(oldCursorItemEmplacement);
                    cursorSlot.itemSlot.InsertStack(oldSlot);

                }
                else
                {
                    //if the item was same add it, else replace it 
                    clickedSlot.itemSlot.InsertStack(cursorSlot.itemSlot.TakeAll());

                }

            }

        }

        private UIItemSlot CheckForSlot()
        {
            /*we want to cast array out into mouse position, where ever mouse position on the screen
             * and it will be returning the list containing all of UI element that on the mouse in that position
             * example : if we got an inventory slots, and that inventory slots has an icon in it
             * so, both of inventory slots and the icon that at the top of the window like toolbar or inventory window
             * will going to return all of the object in this list 
             */
            m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            m_Raycaster.Raycast(m_PointerEventData, results);

            //choosing the item that we're interesting in it 
            foreach (RaycastResult result in results)
            {

                if (result.gameObject.tag == "UIItemSlot")
                {
                    return result.gameObject.GetComponent<UIItemSlot>();
                }
                
            }

            return null;

        }

    }

}