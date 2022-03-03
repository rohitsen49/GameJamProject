using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Assistant : MonoBehaviour
{
    private Text messageText;

    private void Awake()
    {
        messageText = GetComponent<Text>();
    }

    private void Start()
    {
        messageText.text = "Hello World";
    }
}
