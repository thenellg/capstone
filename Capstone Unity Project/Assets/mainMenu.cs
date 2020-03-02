using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class mainMenu : MonoBehaviour
{
    public int sceneNum;

    public Animator animator;

    public void playGame()
    {
        //Put our main gameplay scene will go after this within our build.

        //animator.SetTrigger("FadeOut");
        SceneManager.LoadScene(1);

    }

    public void playAnims()
    {
        //Put our main gameplay scene will go after this within our build.

        //animator.SetTrigger("FadeOut");
        SceneManager.LoadScene(2);

    }

    public void changeScene(int sceneNum)
    {
        //Put our main gameplay scene will go after this within our build.

        //animator.SetTrigger("FadeOut");
        SceneManager.LoadScene(sceneNum);

    }

    public void QuitGame ()
    {
        Debug.Log("Quit.");
        Application.Quit();
    }

}
