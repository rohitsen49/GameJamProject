using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Assistant : MonoBehaviour
{
    [SerializeField] private TextWriter textWriter;
    private Text messageText;

    private void Awake()
    {
        messageText = GetComponent<Text>();
    }

    private void Start()
    {
        textWriter.AddWriter(messageText, "This is the assisstant speaking, hello and goodbye, see you next time!", .1f, true);
    }
}
