using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Match3BaDumtsPuzzleLib.Managers {
    public static class GameplayManager {
        public static List<Vector2> vector2s = new List<Vector2> { Vector2.right, Vector2.left, Vector2.up, Vector2.down };

        // удаляем совпадающие элементы
        public static int ClearMatchAction(GameObject gameObject) {
            try {
                List<GameObject> matchingTiles = new List<GameObject>();

                matchingTiles.AddRange(CheckCount(gameObject, new List<GameObject>()));
                if (matchingTiles.Count < 3)
                    return 0;

                matchingTiles.ForEach(match => {
                    match.GetComponent<SpriteRenderer>().sprite = null;
                });


                return matchingTiles.Count;
            } catch (Exception ex) {
                Debug.LogError("ClearMatchAction error: " + ex.Message);
                return -1;
            }
        }


        // проверяем, что не менее 2-х аналогичных пересекается
        public static List<GameObject> CheckCount(GameObject tile, List<GameObject> commonMatchTiles) {
            try {
                while (true) {
                    var matchingTiles = FindMatch(tile);

                    matchingTiles.ForEach(tileFound => {
                        if (!commonMatchTiles.Contains(tileFound)) {
                            commonMatchTiles.Add(tileFound);
                            commonMatchTiles = CheckCount(tileFound, commonMatchTiles);
                        }
                    });
                    break;
                }
            } catch (Exception ex) {
                Debug.LogError("checkCount error: " + ex.Message);
            }

            return commonMatchTiles;
        }

        // проверяем, что кликнули на близлежащую фишку
        public static bool CheckCloseOrFar(GameObject gameObject, GameObject previousSelected) {
            List<GameObject> gameObjects = new List<GameObject>();

            var hit = Physics2D.Raycast(previousSelected.transform.localPosition, Vector2.right);
            var hit1 = Physics2D.Raycast(previousSelected.transform.localPosition, Vector2.left);
            var hit2 = Physics2D.Raycast(previousSelected.transform.localPosition, Vector2.up);
            var hit3 = Physics2D.Raycast(previousSelected.transform.localPosition, Vector2.down);


            if (hit.collider != null) gameObjects.Add(hit.collider.gameObject);
            if (hit1.collider != null) gameObjects.Add(hit1.collider.gameObject);
            if (hit2.collider != null) gameObjects.Add(hit2.collider.gameObject);
            if (hit3.collider != null) gameObjects.Add(hit3.collider.gameObject);

            if (gameObjects.Contains(gameObject)) return true;

            return false;
        }

        // собирает все элементы такого типа вокруг
        public static List<GameObject> FindMatch(GameObject startGameObject) {
            List<GameObject> matchingTiles = new List<GameObject>();

            try {
                vector2s.ForEach(vector => {
                    RaycastHit2D hit = Physics2D.Raycast(startGameObject.transform.position, vector);

                    if (hit.collider != null &&
                        hit.collider.GetComponent<SpriteRenderer>().sprite == startGameObject.GetComponent<SpriteRenderer>().sprite) {
                        matchingTiles.Add(hit.collider.gameObject);
                    }

                });
            } catch (Exception ex) {
                Debug.LogError("FindMatch error: " + ex.Message);
            }

            return matchingTiles;
        }

        // перемешивание элементов после создания доски
        public static GameObject[,] MixElements(GameObject[,] tiles, int xSize, int ySize, List<Sprite> characters) {
            try {
                for (int x = 0; x < xSize; x++) {
                    for (int y = 0; y < ySize; y++) {

                        if (y == 0 && x < 4) continue;
                        if (CheckCount(tiles[x, y], new List<GameObject>()).Count >= 3) {
                            // чтобы случайно не зарандомить такой же элемент
                            while (true) {
                                Sprite newSprite = characters[UnityEngine.Random.Range(0, characters.Count)];
                                if (!newSprite.Equals(tiles[x, y].GetComponent<SpriteRenderer>().sprite)) {
                                    tiles[x, y].GetComponent<SpriteRenderer>().sprite = newSprite;
                                    if (CheckCount(tiles[x, y], new List<GameObject>()).Count < 2) break;
                                }
                            }

                        }

                    }
                }
            } catch (Exception ex) {
                Debug.LogError("MixElements error: " + ex.Message);
            }

            return tiles;
        }

        public static GameObject[,] CreateExamplePartOfTiles(float startX,
            float startY,
            Vector2 offset,
            GameObject[,] tiles,
            List<Sprite> characters,
            GameObject tile) {
            var newSpriteForExample = characters[0];
            var newSpriteForExampleDiff = characters[1];
            try {
                for (int x = 0; x < 4; x++) {
                    var newTileForExample = GameObject.Instantiate(tile,
                            new Vector3(startX + (offset.x * x), startY + (offset.y * 0), 0),
                            tile.transform.rotation);

                    if (x != 2) newTileForExample.GetComponent<SpriteRenderer>().sprite = newSpriteForExample;
                    else newTileForExample.GetComponent<SpriteRenderer>().sprite = newSpriteForExampleDiff;

                    newTileForExample.transform.parent = tile.transform.parent;
                    newTileForExample.name = "TileClone" + x + 0;
                    newTileForExample.SetActive(true);
                    tiles[x, 0] = newTileForExample;
                }
            } catch (Exception ex) {
                Debug.LogError("CreateExamplePartOfTiles error: " + ex.Message);
            }
            return tiles;
        }
    }
}
