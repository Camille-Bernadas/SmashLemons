using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void Play(){
        SceneManager.LoadScene(5);
    }
        public void Options(){
        SceneManager.LoadScene(2);
    }

    public void Quit(){
        Debug.Log("This would quit if not in editor.");
        Application.Quit();
    }
}
