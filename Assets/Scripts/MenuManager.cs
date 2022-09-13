using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject menuScreen;
    public GameObject howToScreen;
    public GameObject creditsScreen;

    public void Start() {
        menuScreen.SetActive(true);
    }

    public void GoToTutorial() {
        menuScreen.SetActive(false);
        howToScreen.SetActive(true);
    }

    public void GoToCredits() {
        menuScreen.SetActive(false);
        creditsScreen.SetActive(true);
    }

    public void GoToMain() {
        menuScreen.SetActive(true);
        howToScreen.SetActive(false);
        creditsScreen.SetActive(false);
    }

    public void Play() {
        SceneManager.LoadScene(1);
    }
}
