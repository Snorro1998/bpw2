using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelDescription : MonoBehaviour
{

    void Start()
    {
        string description = LevelProperties.Instance.levelDesc;

        TextMeshProUGUI textPro = (TextMeshProUGUI)GetComponent(typeof(TextMeshProUGUI));
        Text textDefault = (Text)GetComponent(typeof(Text));

        if (textPro != null)
        {
            textPro.text = description;
            //return;
        }
        else
        {
            textDefault.text = description;
        }
    }
}
