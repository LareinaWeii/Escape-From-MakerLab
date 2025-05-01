using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("-------------Audio Source-------------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    public static AudioManager Instance; // 单例模式

    [Header("-------------Audio clips-------------")]
    public AudioClip gameBgm; // 游戏场景背景音乐
    public AudioClip openingBgm; // Bgm for opening screen
    public AudioClip openingEggBgm; // Bgm for opening screen of Egg
    public AudioClip clickClip; // 点击音效
    public AudioClip FairyClip; // 仙女音效
    public AudioClip OpenDoorClip; // 开场音效
    public AudioClip ShootingClip; // 射击音效
    // public AudioSource musicSource; // 用于播放背景音乐的 AudioSource

    [Header("-------------Others-------------")]
    public GameObject menuCanvas;
    public MenuManage menuManage;
    private GameObject openingScreen;
    private MenuManage MenuManageScript;
    private SceneSwitcher SceneManagerScript; //Careful that SceneManager.cs's class name is SceneSwitcher, not SceneManager

    [Header("-------------Flags-------------")]
    private bool playOnceFlag = true; // Flag to control the play once of opening screen BGM
    private bool hasPlayedFairyClip = false; // Flag to ensure FairyClip plays only once
    private bool hasPlayedDoorClip = false;
    private bool hasPlayedShootingClip = false; // Flag to ensure shooting sound plays only once

    private void Awake()
    {
        // 如果已经存在一个 AudioManager 实例，则销毁当前的
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 设置为当前实例并防止销毁
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {   
        MenuManageScript = GameObject.Find("Menu Manager").GetComponent<MenuManage>();
        SceneManagerScript = GameObject.Find("SceneManager").GetComponent<SceneSwitcher>();
        openingScreen = MenuManageScript.openingScreen;
        
    }

    private void Update()
    {
        if(MenuManageScript.isOpeningScreen)
        {
            if(playOnceFlag)
            {
                if (openingScreen.activeSelf && !menuManage.easterEggerIsTriggered)
                {
                    // PlayBgm(openingBgm);
                    PlaySfx(openingBgm); // 播放点击音效
                }

                else if (openingScreen.activeSelf && menuManage.easterEggerIsTriggered)
                {
                    PlayBgm(openingEggBgm);
                }

                playOnceFlag = false; // Reset the flag to prevent repeated calls
            }

        }
        else
        {
            PlayBgm(gameBgm); // 播放游戏场景背景音乐
        }


        if(MenuManageScript.isTutScreenOpenedFlag)
        {
            if(!hasPlayedFairyClip)
            {
                PlaySfx(FairyClip); // 播放游戏场景背景音乐
                hasPlayedFairyClip = true; // Set the flag to true to prevent repeated calls
            }
        }
        else
        {
            hasPlayedFairyClip = false; // Reset the flag when the tutorial screen is closed
        }

        if(SceneManagerScript.isOpeningScreenPassed && !hasPlayedDoorClip)
        {
            PlaySfx(OpenDoorClip); // 播放游戏场景背景音乐
            hasPlayedDoorClip = true; // Set the flag to true to prevent repeated calls
        }

        if(MenuManageScript.isStoryScreen2OpenedFlag && !hasPlayedShootingClip)
        {
            PlaySfx(ShootingClip); // 播放游戏场景背景音乐
            hasPlayedShootingClip = true; // Set the flag to true to prevent repeated calls
        }
    }


    public void PlayBgm(AudioClip clip)
    {
        if (musicSource.clip == clip) return; // 如果正在播放同一首音乐则不切换
        musicSource.clip = clip;
        musicSource.Play();
    }


    public void PlaySfx(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    public void SetVolume(float volume)
    {
        musicSource.volume = volume; // 调节背景音乐音量
    }
}
