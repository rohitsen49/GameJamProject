using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;

public class ScriptReader : MonoBehaviour
{
    [SerializeField] private float typingSpeed = 0.04f;
    [SerializeField] private GameObject continueIcon;
    private TextAsset _inkJsonFile;
    private Story _StoryScript;

    private Coroutine displayLineCoroutine;
    private bool canContinueToNextLine = false;

    public TMP_Text dialougueBox;
    public TMP_Text nameTag;
    void Start()
    {
        LoadStory();
    }

    private void Update()
    {
        if (canContinueToNextLine && Input.GetKeyDown(KeyCode.Mouse0))
        {
            DisplayNextLine();
        }
    }

    void LoadStory()
    {
        _StoryScript = new Story(_inkJsonFile.text);

        _StoryScript.BindExternalFunction("Name", (string charName) => ChangeName(charName));
    }

    public void DisplayNextLine()
    {
        if (_StoryScript.canContinue) // Checking if there is content to go through
        {
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }
            displayLineCoroutine = StartCoroutine(DisplayLine(_StoryScript.Continue()));
            string text = _StoryScript.Continue(); //get next line
            text = text?.Trim(); //Removes white space from text
        }
        else
        {
            dialougueBox.text = "END";
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        //empty the dialouge text
        dialougueBox.text = "";
        //hide items while text is typing
        continueIcon.SetActive(false);

        canContinueToNextLine = false;

        //display each letter one at a time
        foreach (char letter in line.ToCharArray())
        {
            //if the submit button is pressed, finish up displaying the line right away
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                dialougueBox.text = line;
                break;
            }

            dialougueBox.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        //actions to take after the entire line has finished displaying
        continueIcon.SetActive(true);

        canContinueToNextLine = true;
    }

    public void ChangeName(string name)
    {
        string SpeakerName = name;

        nameTag.text = SpeakerName;
    }
}
