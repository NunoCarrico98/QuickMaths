using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    private Canvas canvas;
    private TextMeshProUGUI coins;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        foreach (Transform t in canvas.transform)
        {
            if (t.name == "Coins")
                coins = t.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        }
    }

    private void Update()
    {
        coins.text = $"{GameManager.Instance.Coins}";
    }
}
