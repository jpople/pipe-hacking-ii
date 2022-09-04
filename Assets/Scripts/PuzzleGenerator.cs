using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleGenerator : MonoBehaviour
{
    public int boardWidth;
    public int boardHeight;

    public GameObject cursorPrefab;

    // sprites
    public Sprite[] straight;
    public Sprite[] curve;
    public Sprite blank;
    public Sprite[] input;
    public Sprite[] drain;
    public Sprite cover;

    // random tile spawn rates (proportional, e.g. 2/2/1 yields 40%/40%/20% spawn chance)
    public int straightChance;
    public int curveChance;
    public int blankChance;

    // flow rate
    public float flowRate;

    public float tileScaling = 0.16f;

    public void Initialize() {
        BoardManager.board = new Tile[boardWidth, boardHeight];
    }

    public void GeneratePuzzle() {
        GenerateGoals();
        for (int x = 0; x < boardWidth; x++) {
            for (int y = 0; y < boardHeight; y++) {
                if (BoardManager.board[x, y] == null) {
                    Tile newTile = null;
                    if (x == 0 || y == 0 || x == boardWidth - 1 || y == boardHeight -1) {
                        newTile = new BlankTile(new Vector2Int(x, y));
                        newTile.isBorder = true;
                    }
                    else {
                        newTile = GenerateRandomTile(new Vector2Int(x, y));
                    }
                    BoardManager.board[x, y] = newTile;
                }
            }
        }
        PipeLogic.TracePipeline(BoardManager.input.position);
    }

    public Tile GenerateRandomTile(Vector2Int position) {
        Tile newTile = null;
        int sumOfRates = straightChance + curveChance + blankChance;
        int roll = Random.Range(0, sumOfRates);
        if (roll < straightChance) {
            newTile = new StraightTile(position, Random.Range(0, 4));
        }
        else if ((roll -= straightChance) < curveChance) {
            newTile = new CurveTile(position, Random.Range(0, 4));
        }
        else {
            newTile = new BlankTile(position);
        }
        return newTile;
    }

    public void GenerateGoals() {
        // probably this doesn't need to be one method with two parts
        // place input tile
        Vector2Int inputPosition = new Vector2Int();
        int wallRoll = Random.Range(0, 4); // top, then count clockwise
        int wallSize = wallRoll % 2 == 0 ? boardWidth : boardHeight;
        int positionOnWall = Random.Range(1, wallSize - 1);
        switch(wallRoll) {
            case 0:
                inputPosition = new Vector2Int(positionOnWall, boardHeight - 1);
                break;
            case 1:
                inputPosition = new Vector2Int(boardWidth - 1, positionOnWall);
                break;
            case 2:
                inputPosition = new Vector2Int(positionOnWall, 0);
                break;
            case 3:
                inputPosition = new Vector2Int(0, positionOnWall);
                break;
        }
        Tile inputTile = new InputTile(inputPosition);
        inputTile.orientation = 3 - wallRoll;
        inputTile.rotate(inputTile.orientation);
        BoardManager.board[inputPosition.x, inputPosition.y] = inputTile;
        BoardManager.input = inputTile;
        BoardManager.fillingTile = inputTile;

        // place output tile
        Vector2Int drainPosition = new Vector2Int();
        // we have to do a little extra work here to make sure the input and output aren't right next to each other to prevent the solution from being trivial
        bool isAcceptable = false;
        while (!isAcceptable) {
            wallRoll = Random.Range(0, 4);
            wallSize = wallRoll % 2 == 0 ? boardWidth : boardHeight;
            positionOnWall = Random.Range(1, wallSize - 1);
            switch(wallRoll) {
            case 0:
                drainPosition = new Vector2Int(positionOnWall, boardHeight - 1);
                break;
            case 1:
                drainPosition = new Vector2Int(boardWidth - 1, positionOnWall);
                break;
            case 2:
                drainPosition = new Vector2Int(positionOnWall, 0);
                break;
            case 3:
                drainPosition = new Vector2Int(0, positionOnWall);
                break;
            }
            if (Vector2Int.Distance(inputPosition, drainPosition) >= 3) {
                // this logic can eventually be more interesting and allow for, e.g., input and drain that are right next to each other but have paths that go opposite directions for a bit
                // that's a lot of work though, and this works for now
                isAcceptable = true;
            }
        }
        Tile drainTile = new DrainTile(drainPosition);
        drainTile.orientation = (3 - wallRoll) % 4;
        drainTile.rotate(drainTile.orientation);
        BoardManager.board[drainPosition.x, drainPosition.y] = drainTile;
        BoardManager.drain = drainTile;

    }

    public void DrawBoard() {
        GameObject tileHolder = GameObject.Find("TileHolder");
        foreach (Transform child in tileHolder.transform) {
            GameObject.Destroy(child.gameObject);
        }

        for (int y = 0; y < boardHeight; y++) {
            for (int x = 0; x < boardWidth; x++) {
                Tile tile = BoardManager.board[x, y];
                if (tile != null) {
                    tile.hasBeenFlipped = false; // this is an ugly hack that results in weird off-frames; figure out if there's a way to properly preserve already-flipped tiles in between redraws
                    GameObject newTileObject = new GameObject();

                    newTileObject.AddComponent<SpriteRenderer>();
                    newTileObject.GetComponent<SpriteRenderer>().sortingLayerName = "Pipes";
                    int spriteNumber = 0;
                    switch(tile.type) {
                        case "straight":
                            spriteNumber = (int) Mathf.Clamp(Mathf.FloorToInt((tile.waterLevel / tile.capacity) * straight.Length), 0, straight.Length - 1);
                            newTileObject.GetComponent<SpriteRenderer>().sprite = straight[spriteNumber];
                            break;
                        case "curve":
                            spriteNumber = (int) Mathf.Clamp(Mathf.FloorToInt((tile.waterLevel / tile.capacity) * curve.Length), 0, curve.Length - 1);
                            newTileObject.GetComponent<SpriteRenderer>().sprite = curve[spriteNumber];
                            break;
                        case "blank":
                            if(!tile.isBorder) {
                                newTileObject.GetComponent<SpriteRenderer>().sprite = blank;
                            }
                            break;
                        case "input":
                            spriteNumber = (int) Mathf.Clamp(Mathf.FloorToInt((tile.waterLevel / tile.capacity) * input.Length), 0, input.Length - 1);
                            newTileObject.GetComponent<SpriteRenderer>().sprite = input[spriteNumber];
                            break;
                        case "drain":
                            spriteNumber = (int) Mathf.Clamp(Mathf.FloorToInt((tile.waterLevel / tile.capacity) * drain.Length), 0, drain.Length - 1);
                            newTileObject.GetComponent<SpriteRenderer>().sprite = drain[spriteNumber];
                            break;
                    }

                    newTileObject.transform.position = new Vector3(x * tileScaling, y * tileScaling, 0);
                    newTileObject.transform.Rotate(new Vector3(0, 0, tile.orientation * 90));
                    newTileObject.name = $"{tile.type} @ {tile.position.x}, {tile.position.y}";

                    newTileObject.transform.parent = tileHolder.transform;
                    if (!tile.isBorder && !tile.isPartOfPipeline) {
                        newTileObject.transform.localScale = newTileObject.transform.localScale * 0.75f;
                        newTileObject.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f);
                    }
                    
                    if(!tile.isBorder && !tile.isRevealed) {
                        GameObject tileCover = new GameObject();
                        tileCover.AddComponent<SpriteRenderer>();
                        tileCover.GetComponent<SpriteRenderer>().sprite = cover;
                        tileCover.GetComponent<SpriteRenderer>().sortingLayerName = "Covers";
                        tileCover.transform.position = new Vector3(x * tileScaling, y * tileScaling, 1);
                        tileCover.transform.parent = newTileObject.transform;
                    }

                    tile.baseObject = newTileObject;
                }
            }
        }
    }

    public void SpriteUpdate () {
        Tile filling = BoardManager.fillingTile;
        SpriteRenderer renderer = BoardManager.fillingTile.baseObject.GetComponent<SpriteRenderer>();
        Tile prev = null;
        Vector2Int flowDirection = new Vector2Int(0, 0); // direction from which liquid is flowing into fillingTile

        if(PipeLogic.pipeline.Count >= 2) {
            int prevIndex = 0;
            if (PipeLogic.pipeline.IndexOf(filling) > 0) {
                prevIndex = PipeLogic.pipeline.IndexOf(filling) - 1;
            }
            prev = PipeLogic.pipeline[prevIndex];
            flowDirection = prev.position - filling.position;
        }

        Sprite[] sheet = null;

        // gotta do a bunch of work here to make sure flow comes in from correct side of sprite
        if (filling.type == "straight") {
            sheet = straight;
            
            bool flip = false;

            switch(flowDirection) {
                case Vector2Int v when v.Equals(Vector2Int.right):
                    flip = filling.orientation == 0;
                    break;
                case Vector2Int v when v.Equals(Vector2Int.up):
                    flip = filling.orientation == 1;
                    break;
                case Vector2Int v when v.Equals(Vector2Int.left):
                    flip = filling.orientation == 2;
                    break;
                case Vector2Int v when v.Equals(Vector2Int.down):
                    flip = filling.orientation == 3;
                    break;
            }
            if(flip && !filling.hasBeenFlipped) {
                filling.baseObject.transform.Rotate(0, 0, 180);
                filling.hasBeenFlipped = true;
            }
        }
        else if (filling.type == "curve") {
            sheet = curve;

            bool flip = false;

            switch(flowDirection) {
                case Vector2Int v when v.Equals(Vector2Int.down):
                    flip = filling.orientation == 0;
                    break;
                case Vector2Int v when v.Equals(Vector2Int.right):
                    flip = filling.orientation == 1;
                    break;
                case Vector2Int v when v.Equals(Vector2Int.up):
                    flip = filling.orientation == 2;
                    break;
                case Vector2Int v when v.Equals(Vector2Int.left):
                    flip = filling.orientation == 3;
                    break;
            }
            if(flip && !filling.hasBeenFlipped) {
                if (filling.orientation == 0 || filling.orientation == 2) {
                    filling.baseObject.transform.Rotate(180, 0, -90);
                }
                else {
                    filling.baseObject.transform.Rotate(0, 180, 90);
                }
                filling.hasBeenFlipped = true;
            }
        }
        else if (filling.type == "input") {
            sheet = input;
        }
        else if (filling.type == "drain") {
            sheet = drain;
        }
        if(sheet != null) {
            int spriteNumber = (int) Mathf.Clamp(Mathf.FloorToInt((filling.waterLevel / filling.capacity) * sheet.Length), 0, (sheet.Length - 1));
            renderer.sprite = sheet[spriteNumber];
        }
    }

    public void SpawnCursor() {
        GameObject cursor = GameObject.Instantiate(cursorPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        CursorMovement movement = cursor.GetComponent<CursorMovement>();
        movement.position = new Vector2Int (Random.Range(1, boardWidth - 2), Random.Range(1, boardHeight - 2));
        cursor.transform.position = new Vector3(movement.position.x * tileScaling, movement.position.y * tileScaling, 1);
    }
}
