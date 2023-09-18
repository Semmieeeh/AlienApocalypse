using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPopup : MonoBehaviour
{
    public TextMeshProUGUI topText, mainText;

    public bool IsActive => gameObject.activeInHierarchy;

    public void Popup(string cause, string description)
    {
        this.topText.text = cause;
        this.mainText.text = description;

        gameObject.SetActive(true);
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false);
    }
}
