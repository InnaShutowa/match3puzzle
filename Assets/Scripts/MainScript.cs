using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScript : MonoBehaviour {

    public Button infoButton;
    public Button listButton;
    public Button exitButton;
    public Button playButton;
    public GameObject attantionPanel;
    public Button closeAttantionPanelButton;
    public Button sureExitButton;
    // Start is called before the first frame update
    void Start() {
        infoButton.onClick.AddListener(info);
        listButton.onClick.AddListener(list);
        exitButton.onClick.AddListener(exit);
        playButton.onClick.AddListener(play);
        closeAttantionPanelButton.onClick.AddListener(closeAttantionPanel);
        sureExitButton.onClick.AddListener(sureExit);
    }

    void info() {
        SceneManager.LoadScene("InfoScene");
    }

    void list() {
        SceneManager.LoadScene("ListScene");
    }

    void exit() {
        attantionPanel.SetActive(true);
    }

    void play() {
        SceneManager.LoadScene("GameplayScene");
    }

    void closeAttantionPanel() {
        attantionPanel.SetActive(false);
    }

    void sureExit() {
        Application.Quit();
    }
}
