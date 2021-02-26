using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;


namespace MinecraftTutorial
{
    public class MenuManager : MonoBehaviour
    {

        [Header("Important GameObject and Reference")]
        public GameObject mainMenu;
        public GameObject settingsMenu;

        [Header("MainMenu UI Elements")]
        public TextMeshProUGUI seedField = null;
        public TextMeshProUGUI playerNameField;
        public TextMeshProUGUI errorTextField;
        public string namePlayer;
        public string inGameScene;
        string worldName;


        [Header("Settings UI Elements")]
        Settings settings;
        public Slider viewDstSlider;
        public TextMeshProUGUI viewDstText;
        public Slider mouseSensitive;
        public TextMeshProUGUI mouseSensitiveSlider;
        public Toggle threadingToggle;
        public Toggle chunkAnimToggle;
        public TMP_Dropdown cloudsType;
        public Toggle survivalModeToggle;

        private void Awake()
        {

            LoadSettings();
            PlayerPrefs.SetString("AppVer", Application.version.ToString());

        }

        void Start()
        {

            BackToMainMenu();

        }

        void Update()
        {
            namePlayer = playerNameField.text.ToString();
            PlayerPrefs.SetString("PlayerName", namePlayer);
        }

        public void StartGame()
        {
            if (string.IsNullOrEmpty(seedField.text.ToString()))
            {
                errorTextField.text = "Error WorldName field can't be empty, please enter the world name !";
            }
            else if (seedField.text == "New World")
            {
                Debug.Log("Default world has been loaded");
                worldName = "New World";
                SendData(worldName);
            }
            else
            {
                worldName = seedField.text;
                SendData(worldName);
            }

        }

        void SendData(string worldName)
        {
            int seed = Mathf.Abs(worldName.GetHashCode()) / VoxelData.worldSizeInChunks;

            PlayerPrefs.SetInt("Seed", seed);
            PlayerPrefs.SetString("WorldName", worldName);

            VoxelData.seed = seed;
            VoxelData.worldName = worldName;

            settings.playerName = PlayerPrefs.GetString("PlayerName");
            Debug.Log("Welcome " + PlayerPrefs.GetString("PlayerName"));

            LoadScenes(inGameScene);
        }

        public void LoadScenes(string scene)
        {

            Debug.Log("Load Scene = " + scene);
            SceneManager.LoadScene(scene);

        }

        public void OpenSettings()
        {

            LoadSettings();

            viewDstSlider.value = settings.viewDistance;
            UpdateViewDistSlider();
            mouseSensitive.value = settings.mouseSensitivity;
            UpdateMouseSlider();

            threadingToggle.isOn = settings.enableThreading;
            chunkAnimToggle.isOn = settings.enableChunkLoadAnimation;
            survivalModeToggle.isOn = settings.enableSurvivalMode;
            cloudsType.value = (int)settings.cloudStyle;

            mainMenu.gameObject.SetActive(false);
            settingsMenu.gameObject.SetActive(true);

        }

        public void BackToMainMenu()
        {

            mainMenu.gameObject.SetActive(true);
            settingsMenu.gameObject.SetActive(false);

        }

        public void SaveSettings()
        {

            settings.playerName = PlayerPrefs.GetString("PlayerName");
            settings.version = PlayerPrefs.GetString("AppVer");
            settings.viewDistance = (int)viewDstSlider.value;
            settings.mouseSensitivity = mouseSensitive.value;
            settings.enableThreading = threadingToggle.isOn;
            settings.enableChunkLoadAnimation = chunkAnimToggle.isOn;
            settings.enableSurvivalMode = survivalModeToggle.isOn;
            settings.cloudStyle = (CloudStyle)cloudsType.value;

            string JsonExport = JsonUtility.ToJson(settings);
            File.WriteAllText(Application.dataPath + "/" + namePlayer + "_Settings.SAVDAT", JsonExport);
            Debug.Log("Data has been saved, detail : " + JsonExport);

        }

        public void LoadSettings()
        {
            
            if (!File.Exists(Application.dataPath + "/" + namePlayer + "_Settings.SAVDAT"))
            {

                Debug.Log("Sorry no settings file found, creating new one");

                settings = new Settings();
                string JsonExport = JsonUtility.ToJson(settings);
                File.WriteAllText(Application.dataPath + "/" + namePlayer + "_Settings.SAVDAT", JsonExport);

            }
            else
            {

                Debug.Log("Settings file found, loading settings." + "\n Welcome : " + namePlayer);

                string JsonImport = File.ReadAllText(Application.dataPath + "/" + namePlayer + "_Settings.SAVDAT");
                settings = JsonUtility.FromJson<Settings>(JsonImport);

            }

        }

        public void QuitGame()
        {

            Debug.Log("Quitting Game Now....");
            Application.Quit();

        }


        public void UpdateViewDistSlider()
        {

            viewDstText.text = "View Distance : " + viewDstSlider.value;

        }

        public void UpdateMouseSlider()
        {

            mouseSensitiveSlider.text = "Mouse Sensitivity : " + mouseSensitive.value.ToString("F1");

        }

    }

}