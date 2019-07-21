using System.Collections;
using UnityEngine;
using TMPro;

public class Blinking : MonoBehaviour
{
    public float interval = 1f;
    public string infoText = "Press 'M' to start the engines";
    private TextMeshProUGUI infoTxt;

    public GameObject infoBox;
    private bool hidden = false;

    private Coroutine _coroutine;
    private IEnumerator toggle()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            infoBox.SetActive(!infoBox.activeSelf);
        }
    }

    void Start()
    {
        infoBox = transform.GetChild(0).gameObject;
        infoTxt = (TextMeshProUGUI)infoBox.transform.GetChild(0).GetComponent(typeof(TextMeshProUGUI));
        infoTxt.text = infoText;
        showInfo();
    }

    public void setInfo(string newText)
    {
        infoTxt.text = newText;
        showInfo();
    }

    public void showInfo()
    {
        hidden = false;
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        _coroutine = StartCoroutine(toggle());
    }

    public void hideInfo()
    {
        if (!hidden)
        {
            hidden = true;
            StopCoroutine(_coroutine);
            infoBox.SetActive(false);
        }
    }
}
