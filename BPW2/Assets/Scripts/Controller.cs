using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    public static Controller Instance { get; private set; }
    public AudioMixer musicMixer;
    public AudioMixer sfxMixer;
    private string music = null;
    private string musicOld = null;

    private float musicVolume = 1;
    private float soundVolume = 1;

    //public bool loadMusic = false;
    //public GameObject levelController = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex != 0)
        {
            AudioManager.Instance.stopSound("engineRunning");
            SceneManager.LoadScene(0);
        }
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void SetMusicVolume(float volume)
    {
        SetVolume(musicMixer, volume);
    }

    public void SetSoundVolume(float volume)
    {
        SetVolume(sfxMixer, volume);
    }

    public void SetVolume(AudioMixer mixer, float volume)
    {
        mixer.SetFloat("volume", volume);
    }

    public void LoadMusic(string newMusic)
    {
        if (newMusic != music)
        {
            if (music != null)
            {
                AudioManager.Instance.stopSound(music);
            }
            musicOld = music;
            music = newMusic;
            AudioManager.Instance.playSound(music);
        }
    }
}
