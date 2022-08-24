using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGenerator : MonoBehaviour
{
    public int boardWidth;
    public int boardHeight;

    public GameObject cursorPrefab;

    public Sprite straight;
    public Sprite curve;
    public float tileScaling;

    public void Initialize() {
        BoardManager.board = new Tile[boardWidth, boardHeight];
    }

    public void GeneratePuzzle() {
        for (int x = 0; x < boardWidth; x++) {
            for (int y = 0; y < boardHeight; y++) {
                Tile newTile = null;
                if (Random.Range(0, 2) == 1) {
                    newTile = new StraightTile(new Vector2Int(x, y), Random.Range(0, 4));
                }
                else {
                    newTile = new CurveTile(new Vector2Int(x, y), Random.Range(0, 4));
                }
                BoardManager.board[x, y] = newTile;
            }
        }
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
                    }
                    newTileObject.transform.position = new Vector3(x * tileScaling, y * tileScaling, 0);
                    newTileObject.transform.Rotate(new Vector3(0, 0, tile.orientation * 90));
                    newTileObject.transform.parent = tileHolder.transform;
                    newTileObject.name = $"{tile.type} @ {tile.position.x}, {tile.position.y}";

                    tile.baseObject = newTileObject;
                }
            }
        }
        Debug.Log(BoardManager.board[0, 0].orientation);
        for (int i = 0; i < 4; i++) {
            Debug.Log($"{i}: {BoardManager.board[0, 0].openings[i]}");
        }
    }
}
