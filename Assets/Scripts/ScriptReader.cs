using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ScriptReader : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] private float typingSpeed = 0.04f;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject continueIcon;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private Animator portraitAnimator;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    private Story currentStory;

    //from Ryan Scheppler
    public UnityEvent<int> OnEnd = new UnityEvent<int>();
    private int scriptEnding = 0;

    //from Chris
    private RandomContainer randomC;
    public AudioClip[] eshuClips;
    public AudioClip[] astrillClips;

    public bool dialogueIsPlaying { get; private set; }

    private bool canContinueToNextLine = false;

    private Coroutine displayLineCoroutine;

    public bool panelActivate = true;

    private static ScriptReader instance;

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue manager in scene");
        }
        instance = this;
    }

    public static ScriptReader GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(panelActivate);

        //from Chris warning make sure only one random container on the level
        randomC = FindObjectOfType<RandomContainer>();

        //get all of the choices text
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }

    }

    private void Update()
    {
        // return right away if dialogue isnt playing
        if (!dialogueIsPlaying)
        {
            return;
        }

        // handle continuing to the next line in the dialogue when submit is pressed
        if (canContinueToNextLine && currentStory.currentChoices.Count == 0 && InputManager.GetInstance().GetSubmitPressed())
        {
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        randomC.PlaySound(false);
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        SceneManager.LoadScene("Ending");

        //From Ryan Scheppler
        scriptEnding = (int)currentStory.variablesState["scriptEnding"];
        OnEnd.Invoke(scriptEnding);
    }

    private void ContinueStory()
    {
        randomC.PlaySound(false);
        if (currentStory.canContinue)
        {
            //set text for current dialogue line
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }
            displayLineCoroutine = StartCoroutine(DisplayLine(currentStory.Continue()));
            // handle tags
            HandleTags(currentStory.currentTags);
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        //empty the dialogue text
        dialogueText.text = "";
        //hide items while text is typing
        continueIcon.SetActive(false);
        HideChoices();

        canContinueToNextLine = false;

        bool isAddingRichTextTag = false;

        //display each letter one at a time
        foreach (char letter in line.ToCharArray())
        {
            // if the submit button is pressed, finish up displaying the line right away
            if (InputManager.GetInstance().GetSubmitPressed())
            {
                dialogueText.text = line;
                break;
            }

            //check for rich text tag, if found, add it without waiting
            if (letter == '<' || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                dialogueText.text += letter;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            //if not rich text, add the next letter and wait a small time
            else
            {
                dialogueText.text += letter;
                randomC.PlaySound(false);
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        //actions to take after the entire line has finished displaying
        continueIcon.SetActive(true);
        DisplayChoices();

        canContinueToNextLine = true;
    }

    private void HideChoices()
    {
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            //parse the tag
            string[] splitTag = tag.Split(':');
            if(splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be appropriately parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey)
            {
                case SPEAKER_TAG:
                    displayNameText.text = tagValue;
                    if (tagValue == "Eshu")
                    {
                        randomC.clips = eshuClips;
                    }
                    else if (tagValue == "Astrill")
                    {
                        randomC.clips = astrillClips;
                    }
                    break;
                case PORTRAIT_TAG:
                    portraitAnimator.Play(tagValue);
                    break;
                case LAYOUT_TAG:
                    Debug.Log("layout=" + tagValue);
                    break;
                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can supporyt. Number of choices given: " + currentChoices.Count);
        }

        int index = 0;

        foreach(Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        // even system requires we clear it first
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        if (canContinueToNextLine)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);
            InputManager.GetInstance().RegisterSubmitPressed();
            ContinueStory();
        }
    }
}
