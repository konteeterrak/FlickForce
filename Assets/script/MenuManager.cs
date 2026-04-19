using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // เพิ่ม TextMeshPro

public class MenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject creditsPanel; // Optional
    
    [Header("Buttons")]
    public Button startButton;
    public Button optionsButton;
    public Button creditsButton;
    public Button quitButton;
    public Button backButton; // ปุ่มกลับจาก Options
    
    [Header("Settings")]
    public string gameSceneName = "GameScene"; // ชื่อ Scene เกม
    
    [Header("Audio")]
    public AudioSource menuAudioSource;
    public AudioClip buttonClickSound;
    public AudioClip buttonHoverSound;
    
    [Header("Options - Audio")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public TMP_Text masterVolumeText; // เปลี่ยนเป็น TMP_Text
    public TMP_Text musicVolumeText; // เปลี่ยนเป็น TMP_Text
    public TMP_Text sfxVolumeText; // เปลี่ยนเป็น TMP_Text
    
    [Header("Options - Graphics")]
    public TMP_Dropdown qualityDropdown; // เปลี่ยนเป็น TMP_Dropdown
    public TMP_Dropdown resolutionDropdown; // เปลี่ยนเป็น TMP_Dropdown
    public Toggle fullscreenToggle;
    public Toggle vsyncToggle;
    
    private Resolution[] resolutions;
    
    void Start()
    {
        AudioListener.pause = false;
        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        // แสดง Main Menu
        ShowMainMenu();
        
        // Setup Buttons
        if (startButton) startButton.onClick.AddListener(StartGame);
        if (optionsButton) optionsButton.onClick.AddListener(ShowOptions);
        if (creditsButton) creditsButton.onClick.AddListener(ShowCredits);
        if (quitButton) quitButton.onClick.AddListener(QuitGame);
        if (backButton) backButton.onClick.AddListener(ShowMainMenu);
        
        // Setup Audio Sliders
        if (masterVolumeSlider)
        {
            masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }
        
        if (musicVolumeSlider)
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        
        if (sfxVolumeSlider)
        {
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }
        
        // Setup Graphics Settings
        SetupQuality();
        SetupResolution();
        SetupFullscreen();
        SetupVSync();
        
        // เล่นเสียงพื้นหลัง
        if (menuAudioSource && !menuAudioSource.isPlaying)
        {
            menuAudioSource.Play();
        }
    }
    
    // ===================== MENU NAVIGATION =====================
    
    public void ShowMainMenu()
    {
        PlayButtonSound();
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
        if (optionsPanel) optionsPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(false);
    }
    
    public void ShowOptions()
    {
        PlayButtonSound();
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(true);
        if (creditsPanel) creditsPanel.SetActive(false);
    }
    
    public void ShowCredits()
    {
        PlayButtonSound();
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(true);
    }
    
    public void StartGame()
    {
        PlayButtonSound();
        Debug.Log("=== StartGame() ถูกเรียก! ===");
        Debug.Log("กำลังโหลด Scene: [" + gameSceneName + "]"); // ดูชื่อในวงเล็บ
        SceneManager.LoadScene(gameSceneName);
    }
    
    public void QuitGame()
    {
        PlayButtonSound();
        Debug.Log("ออกจากเกม");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    // ===================== AUDIO SETTINGS =====================
    
    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
        if (masterVolumeText) masterVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
        
        // Debug
        Debug.Log("Master Volume: " + volume);
    }
    
    public void SetMusicVolume(float volume)
    {
        if (menuAudioSource) menuAudioSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        if (musicVolumeText) musicVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
    }
    
    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        if (sfxVolumeText) sfxVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
    }
    
    // ===================== GRAPHICS SETTINGS =====================
    
    void SetupQuality()
    {
        if (qualityDropdown)
        {
            qualityDropdown.ClearOptions();
            
            // สร้าง List สำหรับ TMP_Dropdown
            System.Collections.Generic.List<TMP_Dropdown.OptionData> options = new System.Collections.Generic.List<TMP_Dropdown.OptionData>();
            
            foreach (string qualityName in QualitySettings.names)
            {
                options.Add(new TMP_Dropdown.OptionData(qualityName));
            }
            
            qualityDropdown.AddOptions(options);
            qualityDropdown.value = QualitySettings.GetQualityLevel();
            qualityDropdown.onValueChanged.AddListener(SetQuality);
        }
    }
    
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("Quality", qualityIndex);
        Debug.Log("Quality set to: " + QualitySettings.names[qualityIndex]);
    }
    
    void SetupResolution()
    {
        if (resolutionDropdown)
        {
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();
            
            System.Collections.Generic.List<TMP_Dropdown.OptionData> options = new System.Collections.Generic.List<TMP_Dropdown.OptionData>();
            int currentResolutionIndex = 0;
            
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(new TMP_Dropdown.OptionData(option));
                
                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
            
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }
    }
    
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        Debug.Log("Resolution set to: " + resolution.width + "x" + resolution.height);
    }
    
    void SetupFullscreen()
    {
        if (fullscreenToggle)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }
    }
    
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }
    
    void SetupVSync()
    {
        if (vsyncToggle)
        {
            vsyncToggle.isOn = QualitySettings.vSyncCount > 0;
            vsyncToggle.onValueChanged.AddListener(SetVSync);
        }
    }
    
    public void SetVSync(bool enabled)
    {
        QualitySettings.vSyncCount = enabled ? 1 : 0;
        PlayerPrefs.SetInt("VSync", enabled ? 1 : 0);
    }
    
    // ===================== AUDIO EFFECTS =====================
    
    void PlayButtonSound()
    {
        if (menuAudioSource && buttonClickSound)
        {
            menuAudioSource.PlayOneShot(buttonClickSound);
        }
    }
    
    public void PlayHoverSound()
    {
        if (menuAudioSource && buttonHoverSound)
        {
            menuAudioSource.PlayOneShot(buttonHoverSound);
        }
    }
}