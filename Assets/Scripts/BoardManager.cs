using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager
{
    public static Tile[,] board;
}

public class Tile {
    public Vector2Int position; // "held" tile has position (-1, -1)
    public int orientation; // # of rotations CCW from upright position
    public string type;
    public bool[] openings; // starts at top and counts clockwise

    public bool isSelected;

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
