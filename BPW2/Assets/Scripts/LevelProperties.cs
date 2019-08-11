using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProperties : MonoBehaviour
{
    public static LevelProperties Instance { get; private set; }

    public string music = "jaunty";
    public string levelDesc = "";
    public bool levelStarted = false;
    public GameObject infoPanel;
    public GameObject descPanel;
    public GameObject compPanel;

    private GameObject[] PlayerUIs;

    GameObject cameraPivot;
    GameObject plane;
    Camera mainCam;

    public bool levelCompleted = false;
    private bool levelCompActive = false;

    private Coroutine _coroutine;

    private IEnumerator LevelComplete()
    {
        plane.GetComponent<Plane>().disableArrow();
        compPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        AudioManager.Instance.stopSound("engineRunning");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Controller.Instance.LoadMusic(music);
        if (isMainMenu()) return;
        if (PlayerUIs == null)
        {
            PlayerUIs = GameObject.FindGameObjectsWithTag("PlayerUI");
        }

        foreach (GameObject ui in PlayerUIs)
        {
            ui.SetActive(false);
        }

        if (FindObjectOfType<Plane>() != null)
        {
            plane = FindObjectOfType<Plane>().gameObject;
        }
        
        mainCam = Camera.main;

        if (mainCam.transform.parent != null)
        {
            cameraPivot = mainCam.transform.parent.gameObject;
        }

        
    }

    private void Update()
    {
        if (isMainMenu()) return;
        if (Input.GetKeyDown("space") && !levelStarted)
        {
            levelStarted = true;
            descPanel.SetActive(false);
            infoPanel.SetActive(true);
            mainCam.GetComponent<Animator>().SetBool("Zoom", true);
            mainCam.transform.SetParent(plane.transform);
            mainCam.transform.localPosition = new Vector3(0, 6, -18);//0,6,-15
            mainCam.transform.localRotation = new Quaternion(0, 0, 0, 1);
            //Destroy(cameraPivot);

            foreach (GameObject ui in PlayerUIs)
            {
                ui.SetActive(true);
            }
        }
        /*
        if (plane.GetComponent<Plane>().EngineRunning /*&& infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
        }*/

        if (levelCompleted && !levelCompActive)
        {
            levelCompActive = true;
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
            _coroutine = StartCoroutine(LevelComplete());
        }
    }

    private bool isMainMenu()
    {
        return (SceneManager.GetActiveScene().buildIndex == 0);
    }
}
