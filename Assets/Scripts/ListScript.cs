using Match3BaDumtsPuzzleLib.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ListScript : MonoBehaviour {

    public Text text;
    public TextAsset csvFile;
    public Button backButton;

    // Start is called before the first frame update
    void Start() {
        backButton.onClick.AddListener(back);
#if UNITY_EDITOR
        var lstGames = GameListManager.readCsvFile(AssetDatabase.GetAssetPath(csvFile));
        if (lstGames == null) {
            Debug.LogError("Something go wrong!");
            return;
        }
        lstGames.ForEach(game => {
            this.createNewTextObject(game.FormatLineForRecordsList);
        });
#endif
    }

    void createNewTextObject(string txt) {
        var textCLone = GameObject.Instantiate(text);
        textCLone.text = txt;
        textCLone.transform.localPosition = text.transform.localPosition + new Vector3(0, 10, 0);
        textCLone.transform.parent = text.transform.parent;
        textCLone.font = text.font;
        textCLone.fontSize = text.fontSize;
    }

    public void back() {
        SceneManager.LoadScene("MainScene");
    }

}
