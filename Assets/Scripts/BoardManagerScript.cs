﻿using Match3BaDumtsPuzzleLib.Managers;
using Match3BaDumtsPuzzleLib.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoardManagerScript : MonoBehaviour {

    public TextAsset csvFile;

    public static BoardManagerScript instance;
    public List<Sprite> characters = new List<Sprite>();
    public List<Vector2> vector2s = new List<Vector2> { Vector2.right, Vector2.left, Vector2.up, Vector2.down };

    public GameObject tile;

    public Button surrenderButton;
    public Button sureExitButton;
    public Button stayIntoGameButton;
    public Button refreshButton;

    public Button okButton;

    public Text shagCountText;
    public Text totalPointsText;

    public int shagCount;
    public int totalPoints = 0;
    public int totalPointsToWin = 0;

    public Button exitResultButton;
    public Text resultText;

    public GameObject attantionPanel;
    public GameObject resultPanel;
    public GameObject okPanel;

    public int xSize, ySize;

    public static GameObject[,] tiles;

    IEnumerator Start() {
        instance = GetComponent<BoardManagerScript>();

        okPanel.gameObject.SetActive(false);
        okButton.onClick.AddListener(okStart);
        surrenderButton.onClick.AddListener(surrenderGame);
        sureExitButton.onClick.AddListener(sureExitGame);
        stayIntoGameButton.onClick.AddListener(stayIntoGame);
        refreshButton.onClick.AddListener(restartGame);
        exitResultButton.onClick.AddListener(sureExitGame);

        this.updateText();

        if (Screen.width <= Screen.height) {
            var buff = this.xSize;
            this.xSize = this.ySize;
            this.ySize = buff;
        }
        tiles = new GameObject[xSize, ySize];
        yield return StartCoroutine(this.createTiles());

        this.okPanel.SetActive(true);
        this.turnPaneAndTiles(false);

    }
   

    void okStart() {
        this.okPanel.SetActive(false);
        this.turnPaneAndTiles(true);

    }

    void surrenderGame() {
        attantionPanel.SetActive(true);
        this.turnPaneAndTiles(false);
    }

    void stayIntoGame() {
        attantionPanel.SetActive(false);
        this.turnPaneAndTiles(true);
    }

    void sureExitGame() {
        SceneManager.LoadScene("MainScene");
    }

    void gameOver(bool win) {
        this.resultPanel.SetActive(true);
        if (win) resultText.text = "Поздравляем, вы победили!";
        else resultText.text = "К сожалению, вы проиграли... Попробуйте еще раз!";
        // сохранить в рейтинг

        for (int x = 0; x < this.xSize; x++) {
            for (int y = 0; y < this.ySize; y++) {
                tiles[x, y].SetActive(false);
            }
        }
        this.shagCountText.gameObject.SetActive(false);
        this.totalPointsText.gameObject.SetActive(false);
        this.surrenderButton.gameObject.SetActive(false);

#if UNITY_EDITOR
        var lstGames = GameListManager.readCsvFile(AssetDatabase.GetAssetPath(csvFile));
        GameListManager.writeLine(new GameHistoryModel(lstGames.Count, System.DateTime.Now, this.totalPoints, win),
            AssetDatabase.GetAssetPath(csvFile));
#endif
    }

    void restartGame() {
        attantionPanel.SetActive(false);
        this.turnPaneAndTiles(true);
        for (int x = 0; x < tiles.GetLength(0); x++) {
            for (int y = 0; y < tiles.GetLength(1); y++) {
                Sprite newSprite = characters[Random.Range(0, characters.Count)];
                tiles[x, y].GetComponent<SpriteRenderer>().sprite = newSprite;
                tiles[x, y].GetComponent<SingleTileScript>().deselect();
            }
        }

        this.mixElements();
    }

    void turnPaneAndTiles(bool state) {
        for (int x = 0; x < this.xSize; x++) {
            for (int y = 0; y < this.ySize; y++) {
                tiles[x, y].SetActive(state);
            }
        }
        this.shagCountText.gameObject.SetActive(state);
        this.totalPointsText.gameObject.SetActive(state);
        this.surrenderButton.gameObject.SetActive(state);
    }

    public void updateText() {
        this.shagCountText.text = "Осталось шагов: " + this.shagCount;
        this.totalPointsText.text = this.totalPoints + " / " + this.totalPointsToWin;

        if (this.shagCount <= 0) {

            if (totalPoints >= totalPointsToWin) this.gameOver(true);
            else this.gameOver(false);

        }

        if (totalPoints >= totalPointsToWin) this.gameOver(true);
    }


    void createExamplePartOfTiles(float startX, float startY, Vector2 offset) {
        Sprite newSpriteForExample = characters[0];
        Sprite newSpriteForExampleDiff = characters[1];

        for (int x = 0; x < 4; x++) {
            GameObject newTileForExample = Instantiate(tile,
                    new Vector3(startX + (offset.x * x), startY + (offset.y * 0), 0),
                    tile.transform.rotation);

            if (x != 2) newTileForExample.GetComponent<SpriteRenderer>().sprite = newSpriteForExample;
            else newTileForExample.GetComponent<SpriteRenderer>().sprite = newSpriteForExampleDiff;

            newTileForExample.transform.parent = tile.transform.parent;
            newTileForExample.name = "TileClone" + x + 0;
            newTileForExample.SetActive(true);
            tiles[x, 0] = newTileForExample;
        }
    }

    IEnumerator showExample() {
        tiles[3, 0].GetComponent<SingleTileScript>().select();
        yield return new WaitForSeconds(0.2f);
        tiles[2, 0].GetComponent<SingleTileScript>()
            .swapSprite(tiles[3, 0].GetComponent<SingleTileScript>().GetComponent<SpriteRenderer>());

        yield return new WaitForSeconds(0.2f);
        tiles[2, 0].GetComponent<SingleTileScript>().clearMatch();
        yield return StartCoroutine(findNullTiles());
        yield return StartCoroutine(checkTilesAfterCleanMatches());
        yield return StartCoroutine(fillNullTilesByRandom());

        tiles[3, 0].GetComponent<SingleTileScript>().deselect();
    }


    IEnumerator createTiles() {
        Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size;

        float startX = transform.position.x;
        float startY = transform.position.y;

        this.createExamplePartOfTiles(startX, startY, offset);
        
        for (int x = 0; x < this.xSize; x++) {
            for (int y = 0; y < this.ySize; y++) {
                if (y == 0 && x < 4) continue;
                GameObject newTile = Instantiate(tile,
                    new Vector3(startX + (offset.x * x), startY + (offset.y * y), 0),
                    tile.transform.rotation);

                Sprite newSprite = characters[Random.Range(0, characters.Count)];
                newTile.GetComponent<SpriteRenderer>().sprite = newSprite;

                newTile.transform.parent = tile.transform.parent;
                newTile.name = "TileClone" + x + y;
                newTile.SetActive(true);
                tiles[x, y] = newTile;
            }
        }

        this.mixElements();

        yield return StartCoroutine(showExample());
    }

    public void mixElements() {
        for (int x = 0; x < this.xSize; x++) {
            for (int y = 0; y < this.ySize; y++) {

                if (y == 0 && x < 4) continue;
                if (checkCount(tiles[x, y], new List<GameObject>()).Count >= 3) {
                    // чтобы случайно не зарандомить такой же элемент
                    while (true) {
                        Sprite newSprite = characters[Random.Range(0, characters.Count)];
                        if (!newSprite.Equals(tiles[x, y].GetComponent<SpriteRenderer>().sprite)) {
                            tiles[x, y].GetComponent<SpriteRenderer>().sprite = newSprite;
                            if (checkCount(tiles[x, y], new List<GameObject>()).Count < 2) break;
                        }
                    }

                }

            }
        }
    }

    // проверяем, что не менее 2-х аналогичных пересекается
    public List<GameObject> checkCount(GameObject tile, List<GameObject> commonMatchTiles) {

        while (true) {
            var matchingTiles = this.findMatch(tile);

            matchingTiles.ForEach(tileFound => {
                if (!commonMatchTiles.Contains(tileFound)) {
                    commonMatchTiles.Add(tileFound);
                    commonMatchTiles = checkCount(tileFound, commonMatchTiles);
                }
            });
            break;
        }

        return commonMatchTiles;

    }



    public IEnumerator checkTilesAfterCleanMatches() {
        var buff = true;

        while (buff) {
            buff = false;
            for (int x = 0; x < xSize; x++) {
                for (int y = 0; y < ySize; y++) {

                    if (tiles[x, y].GetComponent<SpriteRenderer>().sprite != null
                        && checkCount(tiles[x, y], new List<GameObject>()).Count >= 3) {
                        this.totalPoints += tiles[x, y].GetComponent<SingleTileScript>().clearMatch();
                        buff = true;
                        break;
                    }

                }
                if (buff) {
                    yield return StartCoroutine(findNullTiles());
                    break;
                }
            }
        }

    }

    public IEnumerator fillNullTilesByRandom() {
        for (int y = ySize - 1; y >= 0; y--) {
            for (int x = 0; x < xSize; x++) {
                if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null) {

                    while (true) {
                        Sprite newSprite = characters[Random.Range(0, characters.Count)];
                        if (!newSprite.Equals(tiles[x, y].GetComponent<SpriteRenderer>().sprite)) {
                            tiles[x, y].GetComponent<SpriteRenderer>().sprite = newSprite;
                            if (checkCount(tiles[x, y], new List<GameObject>()).Count < 2) {
                                yield return new WaitForSeconds(0.1f);
                                break;
                            }
                        }
                    }

                }
            }
        }
    }

    // собирает все элементы такого типа вокруг
    public List<GameObject> findMatch(GameObject startGameObject) {
        List<GameObject> matchingTiles = new List<GameObject>();

        this.vector2s.ForEach(vector => {
            RaycastHit2D hit = Physics2D.Raycast(startGameObject.transform.position, vector);

            if (hit.collider != null &&
                hit.collider.GetComponent<SpriteRenderer>().sprite == startGameObject.GetComponent<SpriteRenderer>().sprite) {
                matchingTiles.Add(hit.collider.gameObject);
            }

        });

        return matchingTiles;
    }

    public IEnumerator findNullTiles() {
        for (int x = 0; x < xSize; x++) {
            for (int y = 0; y < ySize; y++) {
                if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null) {
                    yield return StartCoroutine(shiftTilesDown(x, y));
                    break;
                }
            }
        }
    }

    IEnumerator shiftTilesDown(int x, int yStart, float shiftDelay = .03f) {

        var tilesList = new List<GameObject>();

        for (int y = yStart; y < ySize; y++) {
            tilesList.Add(tiles[x, y]);
        }

        if (!tilesList.Any(a => a.GetComponent<SpriteRenderer>().sprite != null) ||
            !tilesList.Any(a => a.GetComponent<SpriteRenderer>().sprite == null))
            yield return null;

        var isNessShifting = true;

        while (isNessShifting) {
            isNessShifting = false;
            for (int y = yStart; y < ySize - 1; y++) {
                if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null) {

                    for (int yInternal = y + 1; yInternal < ySize; yInternal++) {
                        if (tiles[x, yInternal].GetComponent<SpriteRenderer>().sprite != null) {
                            yield return new WaitForSeconds(shiftDelay);
                            tiles[x, y].GetComponent<SpriteRenderer>().sprite = tiles[x, yInternal].GetComponent<SpriteRenderer>().sprite;
                            tiles[x, yInternal].GetComponent<SpriteRenderer>().sprite = null;
                            isNessShifting = true;
                            break;
                        }
                    }

                }
            }
        }

    }
}
