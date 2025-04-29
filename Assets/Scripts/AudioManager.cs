using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
    // public AudioSource musicSource; // 用于播放背景音乐的 AudioSource

    [Header("-------------Others-------------")]
    public GameObject menuCanvas;
    public MenuManage menuManage;
    public GameObject openingScreen;
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
        
        // // 默认播放菜单背景音乐
        // if (gameBgm != null)
        // {
        //     PlayBgm(gameBgm);
        // }
    }

    private void Update()
    {
        if(openingScreen != null)
        {

            if (openingScreen.activeSelf && !menuManage.easterEggerIsTriggered)
            {
                PlayBgm(openingBgm);
            }

            else if (openingScreen.activeSelf && menuManage.easterEggerIsTriggered)
            {
                PlayBgm(openingEggBgm);
            }
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
