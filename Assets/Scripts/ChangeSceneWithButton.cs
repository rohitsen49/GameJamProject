using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneWithButton : MonoBehaviour
{
    public bool gamePaused = false;
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);

        /* if(Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene("PauseMenu");
            gamePaused = true;

            Input.GetKeyDown(KeyCode.Escape);
            {
                SceneManager.LoadScene(sceneName);
                gamePaused = false;
            }
        } */
    }
}
