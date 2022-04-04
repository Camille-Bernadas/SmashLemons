using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer mainMixer;

    public void SetFullscreen(bool isFullscreen){
        Screen.fullScreen = isFullscreen;
    }
    public void SetQuality(int qualityIndex){
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void SetVolume(float volume){
        mainMixer.SetFloat("volume", volume);
    }
    public void back(){
        SceneManager.LoadScene(0);
    }

    public void Quit(){
        Debug.Log("This would quit if not in editor.");
        Application.Quit();
    }
}
