using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class mainMenu : MonoBehaviour
{
    public int playerLives;
    public int numRounds;

    public Animator animator;


    public void NewGame()
    {
        SceneManager.LoadScene(1);
        PlayerPrefs.SetInt("NumRounds", numRounds);
        PlayerPrefs.SetInt("roundResults", 0);
    }

    public void playGame ()
    {
        //Put our main gameplay scene will go after this within our build.

        //animator.SetTrigger("FadeOut");
        SceneManager.LoadScene(1);

    }

    public void QuitGame ()
    {
        Debug.Log("Quit.");
        Application.Quit();
    }

}
