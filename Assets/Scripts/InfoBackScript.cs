using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InfoBackScript : MonoBehaviour {

    public Button backButton;

    void Start() {
        backButton.onClick.AddListener(back);
    }

    void back() {
        SceneManager.LoadScene("MainScene");
    }
}
