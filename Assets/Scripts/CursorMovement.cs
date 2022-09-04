using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorMovement : MonoBehaviour
{
    GameManager manager;
    PuzzleGenerator puzzle;
    
    public Vector2Int position;

    public AudioSource moveAudio;
    public AudioSource cancelAudio;
    public AudioSource selectAudio;
    public AudioSource swapAudio;
    public AudioSource revealAudio;
    public AudioSource finishAudio;
    public AudioSource forbiddenAudio;

    public float moveTime = 0.15f;
    bool isMoving;

    public float coverFadeTime = 0.2f;
    bool isRevealing;

    public GameObject selectionPrefab;
    bool isSelecting = true;

    private void Start() {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        puzzle = GameObject.Find("GameManager").GetComponent<PuzzleGenerator>();
    }

    private void Update() {
        // also also TODO: update isSelected of tiles appropriately
        // TODO: this shit can *super* be refactored into a nicer external Move() method that can also handle the tiles' isSelected
        if (!BoardManager.GetTile(position).isRevealed) {
            manager.interactText.text = "reveal";
        }
        else {
            manager.interactText.text = isSelecting ? "select" : "swap";
        }
        manager.cancelPrompt.SetActive(!isSelecting); 

        if (Input.GetKeyDown("w") && !isMoving) {
            Move(Vector2Int.up);
        }
        if (Input.GetKeyDown("a") && !isMoving) {
            Move(Vector2Int.left);
        }
        if (Input.GetKeyDown("s") && !isMoving) {
            Move(Vector2Int.down);
        }
        if (Input.GetKeyDown("d") && !isMoving) {
            Move(Vector2Int.right);
        }
        if(Input.GetKeyDown("e") && !isMoving) {
            if(!BoardManager.GetTile(position).isRevealed && !isRevealing) {
                BoardManager.GetTile(position).isRevealed = true;
                PipeLogic.TracePipeline(BoardManager.input.position);
                StartCoroutine(DestroyCover());
            }
            else {
                if (BoardManager.GetTile(position).waterLevel == 0) {
                    if (isSelecting) {
                        selectAudio.Play();
                        GameObject selectionIndicator = GameObject.Instantiate(selectionPrefab, transform.position, Quaternion.identity);
                        BoardManager.selectedTile = BoardManager.GetTile(position);
                        isSelecting = false;
                    }
                    else {
                        SwapTiles();
                    }
                }
                else {
                    forbiddenAudio.Play();
                }
            }
        }
        if(Input.GetKeyDown("q") && !isMoving) {
            cancelAudio.Play();
            Destroy(GameObject.Find("Selected(Clone)"));
            BoardManager.selectedTile = null;
            isSelecting = true;
        }
        if(Input.GetKeyDown("f") && !isMoving) {
            finishAudio.Play();
            puzzle.flowRate = 20;
        }
    }

    Vector2Int MovementTarget(Vector2Int start, Vector2Int direction) { // "clips" movement to make edges of board loop
        Vector2Int target = start + direction;
        if (direction == Vector2Int.up && start.y == puzzle.boardHeight - 2) {
            target.y = 1;
        }
        else if (direction == Vector2Int.down && start.y == 1) {
            target.y = puzzle.boardHeight - 2;
        }
        if (direction == Vector2Int.right && start.x == puzzle.boardWidth - 2) {
            target.x = 1;
        }
        else if (direction == Vector2Int.left && start.x == 1) {
            target.x = puzzle.boardWidth - 2;
        }
        return target;
    }

    Vector3 GetTransformPosition(Vector2Int coords) { // converts a Vector2Int tile position to a transform position
        return BoardManager.board[coords.x, coords.y].baseObject.transform.position;
    }

    void SwapTiles() {
        // should probably add a coroutine/animation for this but this'll be fine for now
        Tile hovered = BoardManager.GetTile(position);
        Tile selected = BoardManager.selectedTile;
        Vector2Int selectedPositionCopy = selected.position;
        BoardManager.board[selected.position.x, selected.position.y] = hovered;
        BoardManager.board[hovered.position.x, hovered.position.y] = selected;
        selected.position = hovered.position;
        hovered.position = selectedPositionCopy;

        PipeLogic.TracePipeline(BoardManager.input.position);
        puzzle.DrawBoard();


        swapAudio.Play();
        Destroy(GameObject.Find("Selected(Clone)"));
        BoardManager.selectedTile = null;
        isSelecting = true;
    }

    void Move(Vector2Int direction) {
        Vector2Int destination = MovementTarget(position, direction);
        Vector3 destinationPosition = GetTransformPosition(destination);
        StartCoroutine(MoveCursor(destinationPosition));
        position = destination;
        if(!BoardManager.GetTile(position).isRevealed) {
            manager.interactText.text = "reveal";
        }
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
        
        transform.position = destination;
        moveAudio.Play();
        isMoving = false;
    }

    private IEnumerator DestroyCover() {
        isRevealing = true;
        revealAudio.Play();
        float elapsedTime = 0f;

        Transform cover = BoardManager.GetTile(position).baseObject.transform.GetChild(0);
        cover.parent = null;
        Vector3 startPosition = cover.position;
        Vector3 destination = startPosition + new Vector3(0, 0.05f, 0);

        while(elapsedTime < coverFadeTime) {
            cover.position = Vector3.Lerp(startPosition, destination, (elapsedTime / moveTime));
            cover.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Mathf.Lerp(1, 0, (elapsedTime / moveTime)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Destroy(cover.gameObject);
        isRevealing = false;

    }
}
