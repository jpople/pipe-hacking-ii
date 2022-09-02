using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public PuzzleGenerator puzzleGenerator;
    public Canvas resultsCanvas;
    GameObject resultsText;

    void Start() {
        puzzleGenerator = GetComponent<PuzzleGenerator>();

        puzzleGenerator.Initialize();
        puzzleGenerator.GeneratePuzzle();
        puzzleGenerator.DrawBoard();
        puzzleGenerator.SpawnCursor();

        resultsCanvas.enabled = false;
        resultsText = GameObject.Find("ResultText");
    }

    void FixedUpdate() {
        if (BoardManager.fillingTile.waterLevel < BoardManager.fillingTile.capacity) {
            BoardManager.fillingTile.waterLevel += puzzleGenerator.flowRate;
            puzzleGenerator.SpriteUpdate();
        }
        else {
            int index = PipeLogic.pipeline.IndexOf(BoardManager.fillingTile);
            if (PipeLogic.pipeline.Count > index + 1) {
                BoardManager.fillingTile = PipeLogic.pipeline[index + 1];
            }
            else if (BoardManager.fillingTile == BoardManager.drain) {
                resultsCanvas.enabled = true;
                resultsText.GetComponent<Text>().text = "Victory is yours!";
            }
            else {
                resultsCanvas.enabled = true;
                resultsText.GetComponent<Text>().text = "You have been defeated!";
            }
        }
    }
}
