using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PuzzleGenerator puzzleGenerator;

    void Start() {
        puzzleGenerator = GetComponent<PuzzleGenerator>();

        puzzleGenerator.Initialize();
        puzzleGenerator.GeneratePuzzle();
        puzzleGenerator.DrawBoard();
        puzzleGenerator.SpawnCursor();
    }

    void FixedUpdate() {
        if (BoardManager.fillingTile.waterLevel < BoardManager.fillingTile.capacity) {
            BoardManager.fillingTile.waterLevel += puzzleGenerator.flowRate;
            puzzleGenerator.RefreshLabels();
        }
        else {
            int index = PipeLogic.pipeline.IndexOf(BoardManager.fillingTile);
            if (PipeLogic.pipeline.Count > index + 1) {
                BoardManager.fillingTile = PipeLogic.pipeline[index + 1];
            }
            else if (BoardManager.fillingTile == BoardManager.drain) {
                Debug.Log("you win!");
            }
            else {
                Debug.Log("you lose!");
            }
        }
    }
}
