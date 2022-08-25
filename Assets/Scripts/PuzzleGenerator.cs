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

    // random tile spawn rates (relative)
    public int straightChance;
    public int curveChance;
    public int blankChance;

    public float tileScaling;

    public void Initialize() {
        BoardManager.board = new Tile[boardWidth, boardHeight];
    }

    public void GeneratePuzzle() {
        for (int x = 0; x < boardWidth; x++) {
            for (int y = 0; y < boardHeight; y++) {
                Tile newTile = GenerateRandomTile(new Vector2Int(x, y));
                BoardManager.board[x, y] = newTile;
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
                    }
                    newTileObject.transform.position = new Vector3(x * tileScaling, y * tileScaling, 0);
                    newTileObject.transform.Rotate(new Vector3(0, 0, tile.orientation * 90));
                    newTileObject.transform.parent = tileHolder.transform;
                    newTileObject.name = $"{tile.type} @ {tile.position.x}, {tile.position.y}";

                    tile.baseObject = newTileObject;
                }
            }
        }
    }
}
