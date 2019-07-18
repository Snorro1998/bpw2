using System.Collections;
using UnityEngine;

public class Blinking : MonoBehaviour
{
    public float interval = 1f;
    public GameObject obj;

    private Coroutine _coroutine;
    private IEnumerator toggle()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            obj.SetActive(!obj.activeSelf);
        }
    }

    void Start()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        _coroutine = StartCoroutine(toggle());
    }
}
