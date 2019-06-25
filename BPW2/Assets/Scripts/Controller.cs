using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    public static Controller Instance { get; private set; }
    public AudioMixer musicMixer;
    public AudioMixer sfxMixer;

    private float musicVolume = 1;
    private float soundVolume = 1;

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
            AudioManager.Instance.stopSound("jaunty");
            AudioManager.Instance.stopSound("engineRunning");
            SceneManager.LoadScene(0);
        }
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
}
