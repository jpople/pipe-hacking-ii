using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGenerator : MonoBehaviour
{
    public int boardWidth;
    public int boardHeight;

    public GameObject cursorPrefab;

    // sprites
    public Sprite straight;
    public Sprite curve;
    public Sprite blank;
    public Sprite input;
    public Sprite drain;

    // random tile spawn rates (proportional, e.g. 2/2/1 yields 40%/40%/20% spawn chance)
    public int straightChance;
    public int curveChance;
    public int blankChance;

    public float tileScaling;

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
        drainTile.orientation = 3 - wallRoll;
        BoardManager.board[drainPosition.x, drainPosition.y] = drainTile;

    }

    public void DrawBoard() {
        GameObject tileHolder = GameObject.Find("TileHolder");

        for (int y = 0; y < boardHeight; y++) {
            for (int x = 0; x < boardWidth; x++) {
                Tile tile = BoardManager.board[x, y];
                if (tile != null) {
                    GameObject newTileObject = new GameObject();

                    newTileObject.AddComponent<SpriteRenderer>();
                    switch(tile.type) {
                        case "straight":
                            newTileObject.GetComponent<SpriteRenderer>().sprite = straight;
                            break;
                        case "curve":
                            newTileObject.GetComponent<SpriteRenderer>().sprite = curve;
                            break;
                        case "blank":
                            newTileObject.GetComponent<SpriteRenderer>().sprite = blank;
                            break;
                        case "input":
                            newTileObject.GetComponent<SpriteRenderer>().sprite = input;
                            break;
                        case "drain":
                            newTileObject.GetComponent<SpriteRenderer>().sprite = drain;
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

                    tile.baseObject = newTileObject;
                }
            }
        }
    }

    public void SpawnCursor() {
        GameObject cursor = GameObject.Instantiate(cursorPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        CursorMovement movement = cursor.GetComponent<CursorMovement>();
        movement.position = new Vector2Int (Random.Range(1, boardWidth - 2), Random.Range(1, boardHeight - 2));
        cursor.transform.position = new Vector3(movement.position.x * tileScaling, movement.position.y * tileScaling, 1);
    }
}
