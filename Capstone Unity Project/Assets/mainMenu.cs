using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainMenu : MonoBehaviour
{
    public string sceneName;
    public string animationsName;

    public Image black;
    public Animator anim;

    public void loadingScene()
    {
        //Put our main gameplay scene will go after this within our build.
        StartCoroutine(fadingTransition(sceneName));

    }

    public void loadingAnimations()
    {
        //Put our main gameplay scene will go after this within our build.
        StartCoroutine(fadingTransition(animationsName));

    }

    public void QuitGame ()
    {
        Debug.Log("Quit.");
        Application.Quit();
    }

    IEnumerator fadingTransition(string newScene)
    {
        anim.SetBool("Fade", true);
        yield return new WaitUntil(() => black.color.a == 1);
        SceneManager.LoadScene(newScene);
    }

}
