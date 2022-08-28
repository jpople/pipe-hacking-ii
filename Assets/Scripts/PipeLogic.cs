using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeLogic
{
    public static List<Tile> pipeline;

    public static bool IsDirectlyConnected(Tile a, Tile b) {
        Vector2Int step = b.position - a.position;
        if(step.magnitude != 1 || a.isBorder) {
            return false;
        }
        switch (step) {
            case Vector2Int v when v.Equals(Vector2Int.up):
                return a.openings[0] && b.openings[2];
            case Vector2Int v when v.Equals(Vector2Int.down):
                return a.openings[2] && b.openings[0];
            case Vector2Int v when v.Equals(Vector2Int.left):
                return a.openings[3] && b.openings[1];
            case Vector2Int v when v.Equals(Vector2Int.right):
                return a.openings[1] && b.openings[3];
        }
        return false;
    }
}
