using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorMovement : MonoBehaviour
{
    public GameManager manager;
    PuzzleGenerator puzzle;

    Queue<LevelAction> actionQueue;
    
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
    bool isAcceptingInput = true;

    public float coverFadeTime = 0.2f;
    bool isRevealing;

    public GameObject selectionPrefab;
    bool isSelecting = true;

    private void Start() {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        puzzle = GameObject.Find("GameManager").GetComponent<PuzzleGenerator>();
        actionQueue = new Queue<LevelAction>();
    }

    private void Update() {
        // handle updating input prompts (does this need to be here? no, right?)
        if (!BoardManager.GetTile(position).isRevealed) {
            manager.interactText.text = "reveal";
        }
        else {
            manager.interactText.text = isSelecting ? "select" : "swap";
        }
        manager.cancelPrompt.SetActive(!isSelecting); 

        // take in inputs and queue appropriate actions
        if(isAcceptingInput) {
            if (Input.GetButtonDown("MoveUp")) {
                actionQueue.Enqueue(new MoveAction(this, Vector2Int.up));
            }
            if (Input.GetButtonDown("MoveDown")) {
                actionQueue.Enqueue(new MoveAction(this, Vector2Int.down));
            }
            if (Input.GetButtonDown("MoveLeft")) {
                actionQueue.Enqueue(new MoveAction(this, Vector2Int.left));
            }
            if (Input.GetButtonDown("MoveRight")) {
                actionQueue.Enqueue(new MoveAction(this, Vector2Int.right));
            }
            if(Input.GetButtonDown("Interact")) {
                actionQueue.Enqueue(new InteractAction(this));
            }
            if(Input.GetButtonDown("Cancel")) {
                actionQueue.Enqueue(new CancelAction(this));
            }
            if(Input.GetButtonDown("Finish")) {
                actionQueue.Enqueue(new FinishAction(this));
            }
        }

        // resolve the first action in the queue
        if(actionQueue.Count != 0 && !isMoving) {
            actionQueue.Dequeue().Execute();
        }
    }

    
    /* ===========================
        MOVEMENT HELPER METHODS
    =========================== */

    // are these better placed somewhere else?  BoardManager maybe?
    // neither of them cares about anything in this class

    public Vector2Int MovementTarget(Vector2Int start, Vector2Int direction) { // "clips" movement to make edges of board loop
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

    public Vector3 GetTransformPosition(Vector2Int coords) { // converts a Vector2Int tile position to a transform position
        return BoardManager.board[coords.x, coords.y].baseObject.transform.position;
    }

    /* ===========================
        ACTION HANDLING METHODS
    ============================ */

    public void HandleInteract() {
        if(!BoardManager.GetTile(position).isRevealed) {
            Reveal();
        }
        else {
            if (BoardManager.GetTile(position).waterLevel == 0) {
                if (isSelecting) {
                    Select();
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

    void SwapTiles() {
        // should probably add a coroutine/animation for this but this'll be fine for now
        // also, maybe some of the board logic here should live in BoardManager?
        Tile hovered = BoardManager.GetTile(position);
        Tile selected = BoardManager.selectedTile;
        Vector2Int selectedPositionCopy = selected.position;
        BoardManager.board[selected.position.x, selected.position.y] = hovered;
        BoardManager.board[hovered.position.x, hovered.position.y] = selected;
        selected.position = hovered.position;
        hovered.position = selectedPositionCopy;

        PipeLogic.TracePipeline(BoardManager.input.position); // don't forget to update this not to take a parameter anymore at some point
        puzzle.DrawBoard();


        swapAudio.Play();
        Destroy(GameObject.Find("Selected(Clone)"));
        BoardManager.selectedTile = null;
        isSelecting = true;
    }

    public void Move(Vector2Int direction) {
        Vector2Int destination = MovementTarget(position, direction);
        Vector3 destinationPosition = GetTransformPosition(destination);
        StartCoroutine(MoveCursor(destinationPosition));
        position = destination;
        if(!BoardManager.GetTile(position).isRevealed) {
            manager.interactText.text = "reveal";
        }
    }

    void Select() {
        selectAudio.Play();
        GameObject selectionIndicator = GameObject.Instantiate(selectionPrefab, transform.position, Quaternion.identity);
        BoardManager.selectedTile = BoardManager.GetTile(position);
        isSelecting = false;
    }

    void Reveal() {
        BoardManager.GetTile(position).isRevealed = true;
        StartCoroutine(DestroyCover());
        PipeLogic.TracePipeline(BoardManager.input.position);
        puzzle.DrawBoard();
    }

    public void CancelSelection() {
        cancelAudio.Play();
        Destroy(GameObject.Find("Selected(Clone)"));
        BoardManager.selectedTile = null;
        isSelecting = true;
    }

    public void Finish() {
        finishAudio.Play();
        puzzle.flowRate = 20;
    }

    /* ==============
        COROUTINES
    ============== */

    private IEnumerator MoveCursor(Vector3 destination) {
        isMoving = true;
        isAcceptingInput = false;

        float elapsedTime = 0f;

        Vector3 startPosition = transform.position;
        
        while(elapsedTime < moveTime) {
            transform.position = Vector3.Lerp(startPosition, destination, (elapsedTime / moveTime));
            elapsedTime += Time.deltaTime;
            if (elapsedTime / moveTime > 0.5) {
                isAcceptingInput = true;
            }
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

        Destroy(cover.gameObject);
        isRevealing = false;

    }
}