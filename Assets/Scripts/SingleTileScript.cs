using Match3BaDumtsPuzzleLib.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class SingleTileScript : MonoBehaviour {
    private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
    private static SingleTileScript previousSelected = null;

    public SpriteRenderer render;
    private bool isSelected = false;

    private bool matchFound = false;


    private Transform[] adjacentDirections = new Transform[] { };

    void Update() {
        render = GetComponent<SpriteRenderer>();
    }

    private IEnumerator OnMouseDown() {
        this.matchFound = false;
        if (isSelected) {
            this.deselect();
        } else {
            if (previousSelected == null) {
                this.select();
            } else {
                if (GameplayManager.CheckCloseOrFar(gameObject, previousSelected.gameObject)) {
                    this.swapSprite(previousSelected.render);

                    var previousCount = previousSelected.clearMatch();
                    var count = this.clearMatch();

                    if (previousCount > 4 || count > 4) BoardManagerScript.instance.shagCount += 3;
                    else if (previousCount == 4 || count == 4) BoardManagerScript.instance.shagCount += 2;
                    else BoardManagerScript.instance.shagCount--;

                    BoardManagerScript.instance.totalPoints += previousCount;
                    BoardManagerScript.instance.totalPoints += count;

                    yield return StartCoroutine(BoardManagerScript.instance.findNullTiles());
                    yield return StartCoroutine(BoardManagerScript.instance.checkTilesAfterCleanMatches());
                    yield return StartCoroutine(BoardManagerScript.instance.fillNullTilesByRandom());

                    if (previousSelected != null && !previousSelected.matchFound && !this.matchFound)
                        this.swapSprite(previousSelected.render);

                    BoardManagerScript.instance.updateText();

                    previousSelected?.deselect();
                } else {
                    previousSelected?.deselect();
                    this.select();
                }
            }
        }
    }

    public void select() {
        if (render == null)
            render = gameObject.GetComponent<SpriteRenderer>();

        isSelected = true;
        render.color = selectedColor;
        previousSelected = gameObject.GetComponent<SingleTileScript>();
    }

    public void deselect() {
        if (render == null)
            render = gameObject.GetComponent<SpriteRenderer>();

        isSelected = false;
        render.color = Color.white;
        previousSelected = null;
    }

    public void swapSprite(SpriteRenderer render2) {
        if (render.sprite == render2.sprite) {
            return;
        }

        Sprite tempSprite = render2.sprite;
        render2.sprite = render.sprite;
        render.sprite = tempSprite;
    }

    public int clearMatch() {
        if (render == null) render = GetComponent<SpriteRenderer>();
        if (render.sprite == null) return 0;

        var matchingTilesCount = GameplayManager.ClearMatchAction(gameObject);
        if (matchingTilesCount > 3) matchFound = true;

        return matchingTilesCount;
    }

}
