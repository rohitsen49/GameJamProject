using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextWriter : MonoBehaviour
{
    private List<TextWriterSingle> textWriterSingleList;

    private void Awake()
    {
        textWriterSingleList = new List<TextWriterSingle>();
    }
    public void AddWriter(TextWriter uiText, string textToWrite, float timePerCharacter, bool invisibleCharacters)
    {
        textWriterSingleList.Add(new TextWriterSingle(uiText, textToWrite, timePerCharacter, invisibleCharacters));
    }

    private void Update()
    {
        for (int i = 0; i < textWriterSingleList.Count; i++)
        {

        }
        if (textWriterSingle != null)
        {
            textWriterSingle.update();
        }
    }
    
    public class TextWriterSingle
    {
        private Text uiText;
        private string textToWrite;
        private int characterIndex;
        private float timePerCharacter;
        private float timer;
        private bool invisibleCharacters;

        public TextWriterSingle(Text uiText, string textToWrite, float timePerCharacter, bool invisibleCharacters)
        {
            this.uiText = uiText;
            this.textToWrite = textToWrite;
            this.timePerCharacter = timePerCharacter;
            this.invisibleCharacters = invisibleCharacters;
            characterIndex = 0;
        }

        public void Update()
        {
            if (uiText != null)
            {
                timer -= Time.deltaTime;
                while (timer <= 0f)
                {
                    //display next character
                    timer += timePerCharacter;
                    characterIndex++;
                    string text = textToWrite.Substring(0, characterIndex);
                    if (invisibleCharacters)
                    {
                        text += "<color=#00000000>" + textToWrite.Substring(characterIndex) + "</color>";
                    }
                    uiText.text = text;

                    if (characterIndex >= textToWrite.Length)
                    {
                        //entire string displayed
                        uiText = null;
                        return;
                    }
                }
            }
        }
    }
}
