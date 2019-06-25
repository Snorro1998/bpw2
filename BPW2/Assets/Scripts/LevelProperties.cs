using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProperties : MonoBehaviour
{
    public string Music;
    public string levelDesc;
    public bool levelStarted = false;
    public GameObject infoPanel;
    public GameObject descPanel;
    public GameObject compPanel;
    GameObject cameraPivot;
    GameObject plane;
    Camera mainCam;

    public bool levelComplete = false;

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
        //AudioManager.Instance.playSound(Music);
        plane = FindObjectOfType<Plane>().gameObject;
        mainCam = Camera.main;
        cameraPivot = mainCam.transform.parent.gameObject;
    }

    private void Update()
    {
        if (Input.GetKeyDown("space") && !levelStarted)
        {
            levelStarted = true;
            descPanel.SetActive(false);
            infoPanel.SetActive(true);
            mainCam.GetComponent<Animator>().SetBool("Zoom", true);
            mainCam.transform.SetParent(plane.transform);
            mainCam.transform.localPosition = new Vector3(0, 6, -18);//0,6,-15
            mainCam.transform.localRotation = new Quaternion(0, 0, 0, 1);
            Destroy(cameraPivot);
        }

        if (plane.GetComponent<Plane>().EngineRunning && infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
        }

        if (levelComplete)
        {
            levelComplete = false;
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
            _coroutine = StartCoroutine(LevelComplete());
        }
    }
}
