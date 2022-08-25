using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorMovement : MonoBehaviour
{
    public Vector2Int position;

    public float moveTime = 0.1f;
    bool isMoving;
    Vector3 startPosition, endPosition;

    GameManager manager;
    PuzzleGenerator puzzle;
    // Transform sprite;

    private void Start() {
        // manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        puzzle = GameObject.Find("GameManager").GetComponent<PuzzleGenerator>();
        transform.position = Vector3.zero;
        // sprite = transform.Find("Cursor").transform
    }

    private void Update() {
        // TODO: instead of multiplying by tileScaling, find the precise center of the tile in that position and set movement destination to that
        // also TODO: implement logic to keep cursor in board
        // also TODO: update isSelected of tiles appropriately
        if (Input.GetKey("w") && !isMoving) {
            StartCoroutine(MoveCursor(Vector3.up * puzzle.tileScaling));
        }
        if (Input.GetKey("a") && !isMoving) {
            StartCoroutine(MoveCursor(Vector3.left * puzzle.tileScaling));
        }
        if (Input.GetKey("s") && !isMoving) {
            StartCoroutine(MoveCursor(Vector3.down * puzzle.tileScaling));
        }
        if (Input.GetKey("d") && !isMoving) {
            StartCoroutine(MoveCursor(Vector3.right * puzzle.tileScaling));
        }
    }

    private IEnumerator MoveCursor(Vector3 direction) {
        isMoving = true;

        float elapsedTime = 0f;

        startPosition = transform.position;
        endPosition = startPosition + direction;
        
        while(elapsedTime < moveTime) {
            transform.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime / moveTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isMoving = false;
    }
}
