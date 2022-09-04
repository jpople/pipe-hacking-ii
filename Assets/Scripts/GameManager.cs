﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public PuzzleGenerator puzzleGenerator;
    public Canvas resultsCanvas;
    public Image resultScreen;
    public Text resultText;
    public Text interactText;
    public GameObject inputScreen;
    public GameObject cancelPrompt;
    public AudioSource victoryAudio;
    public AudioSource defeatAudio;

    bool gameOver = false;

    void Start() {
        puzzleGenerator = GetComponent<PuzzleGenerator>();

        puzzleGenerator.Initialize();
        puzzleGenerator.GeneratePuzzle();
        puzzleGenerator.DrawBoard();
        puzzleGenerator.SpawnCursor();

        resultScreen.enabled = false;
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
                if(!gameOver) {
                    victoryAudio.Play();
                }
                resultScreen.enabled = true;
                inputScreen.SetActive(false);
                resultText.text = "Victory is yours!";
                gameOver = true;
            }
            else {
                if(!gameOver) {
                    defeatAudio.Play();
                }
                resultScreen.enabled = true;
                inputScreen.SetActive(false);
                resultText.text = "You have been defeated!";
                gameOver = true;
            }
        }
    }
}
