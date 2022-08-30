using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeLogic
{
    public static List<Tile> pipeline = new List<Tile>();

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

    public static void TracePipeline(Vector2Int inputPosition) {
        foreach (Tile tile in pipeline) {
            tile.isPartOfPipeline = false;
        }
        Tile input = BoardManager.GetTile(inputPosition);
        int direction = 0;
        for (int i = 0; i < 4; i++) {
            if (input.openings[i]) direction = i;
        }

        List<Tile> newPipeline = new List<Tile>();
        newPipeline.Add(input);
        Tile start = null;

        switch (direction) {
            case(0):
                start = BoardManager.GetTile(inputPosition + new Vector2Int(0, 1));
                break;
            case(1):
                start = BoardManager.GetTile(inputPosition + new Vector2Int(1, 0));
                break;
            case(2):
                start = BoardManager.GetTile(inputPosition + new Vector2Int(0, -1));
                break;
            case(3):
                start = BoardManager.GetTile(inputPosition + new Vector2Int(-1, 0));
                break;
        }
        if (start.openings[(direction + 2) % 4]) {
            newPipeline.Add(start);
            start.isPartOfPipeline = true;
            
            bool foundEnd = false;
            Tile current = start;
            Tile next = null;
            while (!foundEnd) {
                foundEnd = true;
                for (int i = 0; i < 4; i++) {
                    switch (i) {
                        case(0):
                            next = BoardManager.GetTile(current.position + new Vector2Int(0, 1));
                            break;
                        case(1):
                            next = BoardManager.GetTile(current.position + new Vector2Int(1, 0));
                            break;
                        case(2):
                            next = BoardManager.GetTile(current.position + new Vector2Int(0, -1));
                            break;
                        case(3):
                            next = BoardManager.GetTile(current.position + new Vector2Int(-1, 0));
                            break;
                    }

                    if(!newPipeline.Contains(next) && current.openings[i] && next.openings[(i + 2) % 4]) {
                        newPipeline.Add(next);
                        next.isPartOfPipeline = true;
                        current = next;
                        foundEnd = false;
                        break;
                    }
                }
            }
            pipeline = newPipeline;
        }
    }
}
