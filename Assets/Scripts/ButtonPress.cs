using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonPress : MonoBehaviour
{
    public string levelName1;
    public string levelName2;

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ChangeScene1()
    {
        SceneManager.LoadScene(levelName1);
    }

    public void ChangeScene2()
    {
        SceneManager.LoadScene(levelName2);
    }
}
