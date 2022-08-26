using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorMovement : MonoBehaviour
{
    public Vector2Int position;

    public float moveTime = 0.1f;
    bool isMoving;

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
        // also also TODO: update isSelected of tiles appropriately
        // TODO: this shit can *super* be refactored into a nicer external Move() method that can also handle the tiles' isSelected
        
        Vector3 destinationPosition;
        Vector2Int destination;

        if (Input.GetKey("w") && !isMoving) {
            destination = MovementTarget(position, Vector2Int.up);
            destinationPosition = GetTransformPosition(destination);
            StartCoroutine(MoveCursor(destinationPosition));
            position = destination;
        }
        if (Input.GetKey("a") && !isMoving) {
            destination = MovementTarget(position, Vector2Int.left);
            destinationPosition = GetTransformPosition(destination);
            StartCoroutine(MoveCursor(destinationPosition));
            position = destination;
        }
        if (Input.GetKey("s") && !isMoving) {
            destination = MovementTarget(position, Vector2Int.down);
            destinationPosition = GetTransformPosition(destination);
            StartCoroutine(MoveCursor(destinationPosition));
            position = destination;
        }
        if (Input.GetKey("d") && !isMoving) {
            destination = MovementTarget(position, Vector2Int.right);
            destinationPosition = GetTransformPosition(destination);
            StartCoroutine(MoveCursor(destinationPosition));
            position = destination;
        }
    }

    Vector2Int MovementTarget(Vector2Int start, Vector2Int direction) { // "clips" movement to make edges of board loop
        Vector2Int target = start + direction;
        if (direction == Vector2Int.up && start.y == puzzle.boardHeight - 1) {
            target.y = 0;
        }
        else if (direction == Vector2Int.down && start.y == 0) {
            target.y = puzzle.boardHeight - 1;
        }
        if (direction == Vector2Int.right && start.x == puzzle.boardWidth - 1) {
            target.x = 0;
        }
        else if (direction == Vector2Int.left && start.x == 0) {
            target.x = puzzle.boardWidth - 1;
        }
        return target;
    }

    Vector3 GetTransformPosition(Vector2Int coords) { // converts a Vector2Int tile position to a transform position
        return BoardManager.board[coords.x, coords.y].baseObject.transform.position;
    }

    private IEnumerator MoveCursor(Vector3 destination) {
        isMoving = true;

        float elapsedTime = 0f;

        Vector3 startPosition = transform.position;
        
        while(elapsedTime < moveTime) {
            transform.position = Vector3.Lerp(startPosition, destination, (elapsedTime / moveTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isMoving = false;
    }
}
