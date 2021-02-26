using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace MinecraftTutorial
{
    public class Player : MonoBehaviour
    {   
        /*Reference*/
        [Tooltip("Getting Important GameObject")]
        [Header("GameObject Reference")]
        public Transform playerCam;
        public WorldGenerator world;
        public Transform highLightBlock;
        public Transform placeBlock;
        public ToolBar toolbar;
        public GameObject InventoryWindow;
        public GameObject InventoryParent;
        public GameObject cursorSlot;

        /*Player Movement*/
        [Header("Player Movement")]
        private float horizontal;
        private float vertical;
        private float mouseHorizontal;
        private float mouseVertical;
        private Vector3 velocity;
        private float minAngle = -90f;
        private float maxAngle = 90f;

        /*player Characteristic or physical apperance*/
        [Header("Character Physical Apperance")]
        public float playerHeight = -50f;
        public float playerWidth = 0.15f;
        public Settings settings;

        /*PlayerTransformPositionInWorld*/
        [Header("PlayerTransformPositionInWorld")]
        public Vector3 spawnPosition;
        public float walkSpeed = 5f;
        public float sprintSpeed = 7f;
        public float jumpForce = 6f;
        [SerializeField]
        private bool jumpRequest = false;
        public bool isGrounded = false;
        public bool isSprinting = false;

        /*World Physics*/
        [Header("World Physics")]
        [SerializeField]
        private float verticalMomentum = 0;
        public float gravityValue = -9.807f;
        /*-9.807 is represent if we remove 1 block every second
        * earth grafity, in maincraft one block is represent meter
        * in our game 1 unit(float) is a sise of our block. 
        * that's mean to be meter.*/

        /*Another Variable*/
        [Header("Another Variable")]
        public float checkIncrement = 0.1f;
        public float reach = 8f;
        public int bagSlotAmount = 36;

        /*Player UI*/
        [Header("Player UI")]
        public bool _inUI = false;
                

        //Dictionary<InventoryData, int> itemDictionary = new Dictionary<InventoryData, int>();

        private void Awake()
        {

            Load_Data();
            settings.version = PlayerPrefs.GetString("AppVer");

        }

        private void Start()
        {
                        
            //Save_Data();
            MouseVisibility();

            InUI = false;
            
        }

        private void Update()
        {

            OpenInventory();
            
            if (!InUI)
            {
                GetPlayerInput();

                PlaceCursorBlocks();
            }

        }

        private void FixedUpdate()
        {

            if (!InUI)
            {
                PlayerMovement();
            }

        }

        #region Save Data With Unity IO
        public void Save_Data()
        {

            string JsonExport = JsonUtility.ToJson(settings);
            Debug.Log(JsonExport);
            //this code will take settings class converting into json formating and stick it into string

            File.WriteAllText(Application.dataPath + "/" + settings.playerName + "_Settings.SAVDAT", JsonExport);

        }

        public void Load_Data()
        {

            settings.playerName = PlayerPrefs.GetString("PlayerName");
            string JsonImport = File.ReadAllText(Application.dataPath + "/" + settings.playerName + "_Settings.SAVDAT");
            settings = JsonUtility.FromJson<Settings>(JsonImport);

        }

        #endregion

        #region Player Movement
        //note : keybindings is at the Project Settings/Input Manager
        private void GetPlayerInput()
        {
            //because we don't have UI yet
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Save_Data();
                Application.Quit();
            }

            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            //mouseHorizontal = Input.GetAxis("Mouse X");
            //mouseVertical = Input.GetAxis("Mouse Y");

            SprintingMode("Sprint");

            JumpingMode("Jump");

            DestroyOrAddCube();

            if (Input.GetKeyDown(KeyCode.F1))
            {

                SaveSystem.SaveWorld(WorldGenerator.Instance.worldData);
                
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                Save_Data();                 
            }

        }

        void PlayerMovement()
        {

            CalculateVelocity();

            if (jumpRequest)
            {
                Jump();
            }
                        
            //rotate horizontal(look left and right)
            mouseHorizontal = Input.GetAxis("Mouse X") * settings.mouseSensitivity;
            transform.Rotate(Vector3.up * mouseHorizontal);
            //rotate vertical(look up and down)
            mouseVertical += Input.GetAxis("Mouse Y") * settings.mouseSensitivity;

            mouseVertical = Mathf.Clamp(mouseVertical, minAngle, maxAngle);
            playerCam.transform.localRotation = Quaternion.Euler(-mouseVertical, 0f, 0f);

            //player Transform (Player Movement in world)
            transform.Translate(velocity, Space.World);
            
        }

        void Jump()
        {
            verticalMomentum = jumpForce;
            isGrounded = false;
            jumpRequest = false;
        }
        
        private void JumpingMode(string keyName)
        {

            if (Input.GetButtonDown(keyName))
            {
                jumpRequest = true;
            }
            if (Input.GetButtonUp(keyName))
            {
                jumpRequest = false;
            }

        }

        private void SprintingMode(string keyName)
        {

            if (Input.GetButtonDown(keyName))
            {
                isSprinting = true;
            }
            if (Input.GetButtonUp(keyName))
            {
                isSprinting = false;
            }

        }
        #endregion

        #region Calculate Velocity
        private void CalculateVelocity()
        {
            //Gravity end Velocity
            /*Affect vertical momentum with gravity*/
            if (verticalMomentum > gravityValue)
            {
                verticalMomentum += Time.fixedDeltaTime * gravityValue;
            }

            //if we're sprinting, use sprinting multiplier
            if (isSprinting)
            {
                velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * sprintSpeed;
            }
            else
            {
                velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * walkSpeed;
            }

            /*Time.deltaTime is the time interval between current frame and the last one, which is the same than the time interval between two call to the Update() method. This means that Update() is going to be called a different number of times depending on the performance of your game.
              On the other hand you have the method FixedUpdate() which works essentially as Update() but with the difference that it is going to be called a fixed amount of times per second. Naturally Time.fixedDeltaTime is the difference between two calls of FixedUpdate() and it always holds the same value.
              unity source : https://docs.unity3d.com/Manual/ExecutionOrder.html?_ga=2.193592618.68571605.1600005531-1189868289.1600005531 */
            
            //Applying vertical momentum (falling/jumping)
            velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;
            
            if ((velocity.z > 0 && frontCollider) || (velocity.z < 0 && backCollider))
            {
                velocity.z = 0;
            }
            if ((velocity.x > 0 && rightCollider) || (velocity.x < 0 && leftCollider))
            {
                velocity.x = 0;
            }

            if (velocity.y < 0)
            {
                velocity.y = CheckDownSpeed(velocity.y);
            }
            else if(velocity.y > 0)
            {
                velocity.y = CheckUpSpeed(velocity.y, 2f);
            }

        }

        #endregion
        
        #region Mouse Visibility
        private void MouseVisibility()
        {

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            InventoryParent.gameObject.SetActive(false);
            InventoryWindow.gameObject.SetActive(false);
            cursorSlot.gameObject.SetActive(false);
                
        }
        #endregion

        #region Editing World(Player Action)


        private void DestroyOrAddCube()
        {

            if (highLightBlock.gameObject.activeSelf)
            {
                //Destroy block
                if (Input.GetMouseButtonDown(0))
                {

                    if (!settings.enableSurvivalMode)
                    {
                        world.GetChunkFromVector3(highLightBlock.position).EditVoxel(highLightBlock.position, 0);
                    }
                    else
                    {

                        byte blockID = world.GetChunkFromVector3(highLightBlock.position).GetVoxelFromGlobalVector3(highLightBlock.position).id;

                        AddItemToPlayerInventory(blockID);

                        world.GetChunkFromVector3(highLightBlock.position).EditVoxel(highLightBlock.position, 0);
                    
                    }
                    
                }

                //Add block
                if (Input.GetMouseButtonDown(1))
                {

                    if (toolbar.slots[toolbar.slotIndex].HasItem)
                    {

                        if (!settings.enableSurvivalMode)
                        {
                            world.GetChunkFromVector3(placeBlock.position).EditVoxel(placeBlock.position, toolbar.slots[toolbar.slotIndex].itemSlot.stack.blockID);
                            toolbar.slots[toolbar.slotIndex].itemSlot.Take(1);
                        }
                        else
                        {

                            byte blockID = toolbar.slots[toolbar.slotIndex].itemSlot.stack.blockID;

                            world.GetChunkFromVector3(placeBlock.position).EditVoxel(placeBlock.position, toolbar.slots[toolbar.slotIndex].itemSlot.stack.blockID);
                            toolbar.slots[toolbar.slotIndex].itemSlot.Take(1);
                            
                            SubtractItemFromPlayerInventory(blockID);

                        }

                    }
                    
                }

            }

        }

        #region Player_Inventory_Data_System
        public void AddItemToPlayerInventory(byte id)
        {
            //add to list inventoryData and block amount of it so we can save it in PlayerInventoryData
            if (!settings.inventoryData.Contains(id))
            {

                settings.inventoryData.Add(id);
                settings.amount.Add(1);

            }
            else
            {

                for (int i = 0; i < settings.inventoryData.Count; i++)
                {

                    if (id == settings.inventoryData[i])
                    {
                        settings.amount[i]++;
                    }

                }

            }

            //put id and amount of it to the Toolbar first ?
        
        }

        public void SubtractItemFromPlayerInventory(byte id)
        {
            //check if it has that item from inventory
            if (settings.inventoryData.Contains(id))
            {

                for (int i = 0; i < settings.inventoryData.Count; i++)
                {

                    if (id == settings.inventoryData[i])
                    {

                        settings.amount[i]--;

                        if (settings.amount[i] == 0)
                        {
                            settings.inventoryData.Remove(id);
                            settings.amount.RemoveAt(i);
                        }

                    }

                }

            }
            
        }
        #endregion

        private void PlaceCursorBlocks()
        {

            float step = checkIncrement;
            Vector3 lastPos = new Vector3();

            while (step < reach)
            {

                Vector3 pos = playerCam.position + (playerCam.forward * step);

                if (world.CheckForVoxel(pos))
                {

                    highLightBlock.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
                    placeBlock.position = lastPos;

                    highLightBlock.gameObject.SetActive(true);
                    placeBlock.gameObject.SetActive(true);

                    return;

                }

                lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));

                step += checkIncrement;

            }

            highLightBlock.gameObject.SetActive(false);
            placeBlock.gameObject.SetActive(false);

        }

        #endregion

        #region Inventory System

        public bool InUI
        {

            get
            {
                return _inUI;
            }

            set
            {

                _inUI = value;
                if (_inUI)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;


                    InventoryParent.gameObject.SetActive(true);
                    InventoryWindow.gameObject.SetActive(true);
                    cursorSlot.gameObject.SetActive(true);
                    
                }
                else
                {
                    MouseVisibility();
                }

            }

        }

        private void OpenInventory()
        {

            if (Input.GetKeyDown(KeyCode.I))
            {

                InUI = !InUI;

            }

        }

        #endregion

        #region ManualMinecraftBoxCollider
        private float CheckDownSpeed(float downSpeed)
        {

            if (
                world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) || 
                world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) || 
                world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth)) || 
                world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth))  
               )
            {

                isGrounded = true;
                return 0;
            
            }
            else
            {

                isGrounded = false;
                return downSpeed;

            }

        }

        private float CheckUpSpeed(float upSpeed, float jumpHight)
        {

            if (
                world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + jumpHight + upSpeed, transform.position.z - playerWidth)) ||
                world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + jumpHight + upSpeed, transform.position.z - playerWidth)) ||
                world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + jumpHight + upSpeed, transform.position.z + playerWidth)) ||
                world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + jumpHight + upSpeed, transform.position.z + playerWidth))
               )
            {

                return 0;

            }
            else
            {
                
                return upSpeed;

            }

        }

        public bool frontCollider
        {
            
            get
            {
                if (
                     world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z + playerWidth)) ||
                     world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth))
                    )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public bool backCollider
        {

            get
            {
                if (
                     world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z - playerWidth)) ||
                     world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth))
                    )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public bool leftCollider
        {

            get
            {
                if (
                     world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y, transform.position.z)) ||
                     world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z))
                    )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public bool rightCollider
        {

            get
            {
                if (
                     world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y, transform.position.z)) ||
                     world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z))
                    )
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


    [System.Serializable]
    public class Settings
    {
        [Header("Game Data")]
        public string version;
        public string playerName;

        [Header("Performance")]
        public int loadDistance = 16;
        public int viewDistance = 4;
        public bool enableThreading = true;
        public CloudStyle cloudStyle = CloudStyle.Fast;
        public bool enableChunkLoadAnimation = false;
        public bool enableSurvivalMode = false;

        [Header("Control")]
        [Range(0.1f, 10f)]
        public float mouseSensitivity = 2.0f;

        [Header("InventoryData")]
        public List<byte> inventoryData = new List<byte>();
        public List<int> amount = new List<int>();

    }


}