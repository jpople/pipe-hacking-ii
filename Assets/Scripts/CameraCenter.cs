using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCenter : MonoBehaviour
{
    public GameManager manager;
    bool centered = false;

    void LateUpdate() {
        if (!centered) {
            Center();
        }
    }

    void Center()
    {
        PuzzleGenerator generator = manager.GetComponent<PuzzleGenerator>();
        Transform lowerLeft = BoardManager.board[0,0].baseObject.transform;
        Transform upperRight = BoardManager.board[generator.boardWidth - 1, generator.boardHeight - 1].baseObject.transform;
        transform.position = new Vector3((lowerLeft.position.x + upperRight.position.x) / 2, (lowerLeft.position.y + upperRight.position.y) / 2, -10f);
        centered = true;
    }
}
