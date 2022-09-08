using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject menuScreen;
    public GameObject howToScreen;

    public void Start() {
        menuScreen.SetActive(true);
        // howToScreen.SetActive(false);
    }
    public void GoToTutorial() {
        menuScreen.SetActive(false);
        howToScreen.SetActive(true);
    }

    public void GoToMain() {
        menuScreen.SetActive(true);
        howToScreen.SetActive(false);
    }

    public void Play() {
        SceneManager.LoadScene(0);
    }
}
