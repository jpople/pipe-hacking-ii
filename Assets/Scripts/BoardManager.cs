using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager
{
    public static Tile[,] board;
    public static Tile input;
    public static Tile drain;
    public static Tile selectedTile;
    public static Tile fillingTile;

    public static Tile GetTile(Vector2Int position) {
        return board[position.x, position.y];
    }
}

public class Tile {
    public Vector2Int position;
    public int orientation; // # of 90° rotations CCW from upright position
    public string type;
    public bool[] openings; // starts at top and counts clockwise

    // pipe logic stuff
    public bool isBorder = false;
    public bool isPartOfPipeline = false;

    // flow logic stuff
    public float capacity = 100f;
    public float waterLevel = 0;

    public GameObject baseObject;

    public void rotate(int times) { // counterclockwise 
        bool [] rotated = new bool[openings.Length];
        for(int i = 0; i < openings.Length; i++) {
            rotated[i] = openings[(i + times) % openings.Length];
        }
        this.openings = rotated;
    }
}

public class StraightTile : Tile {
    public StraightTile(Vector2Int position, int orientation) {
        this.type = "straight";
        this.openings = new bool[] {false, true, false, true};
        this.orientation = orientation;
        rotate(orientation);
        this.position = position;
    }
}

public class CurveTile : Tile {
    public CurveTile(Vector2Int position, int orientation) {
        this.type = "curve";
        this.openings = new bool[] {false, false, true, true};
        this.orientation = orientation;
        rotate(orientation);
        this.position = position;
    }
}

public class BlankTile : Tile {
    public BlankTile(Vector2Int position) {
        this.type = "blank";
        this.openings = new bool[] {false, false, false, false};
        this.position = position;
    }
}

public class InputTile : Tile {
    public InputTile(Vector2Int position) {
        this.type = "input";
        this.isBorder = true;
        this.isPartOfPipeline = true;
        this.openings = new bool[] {false, true, false, false};
        this.position = position;
    }
}

public class DrainTile : Tile {
    public DrainTile(Vector2Int position) {
        this.type = "drain";
        this.isBorder = true;
        this.openings = new bool[] {false, true, false, false};
        this.position = position;
    }
}
