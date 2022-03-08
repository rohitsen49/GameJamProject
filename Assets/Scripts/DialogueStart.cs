using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueStart : MonoBehaviour
{
    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;
    
    void Start()
    {
        //ScriptReader.GetInstance().EnterDialogueMode(inkJSON);
    }

    private void Update()
    {
        if (!ScriptReader.GetInstance().dialogueIsPlaying)
        {
           
            if (InputManager.GetInstance().GetSubmitPressed())
            {
                ScriptReader.GetInstance().EnterDialogueMode(inkJSON);
                print("Starting Dialogue");
            }
        }
    }
}
