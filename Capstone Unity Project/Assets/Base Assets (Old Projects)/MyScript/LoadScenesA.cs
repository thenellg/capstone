// SceneA.
// SceneA is given the sceneName which will
// load SceneB from the Build Settings

using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenesA : MonoBehaviour
{

    public int sceneNum;

    void Start()
    {
        Debug.Log("LoadSceneA");
    }

    public void LoadA(int sceneNum)
    {
        Debug.Log("sceneNum to load: " + sceneNum);
        SceneManager.LoadScene(sceneNum);
    }

    public void LoadLastScene(int sceneNum)
    {
        sceneNum = sceneNum - 2;
        Debug.Log("sceneNum to load: " + sceneNum);
        SceneManager.LoadScene(sceneNum);
    }


    public void LoadAnimMenu()
    {
        SceneManager.LoadScene(2);
    }
}