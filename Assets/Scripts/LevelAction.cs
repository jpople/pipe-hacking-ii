using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAction
{
    public CursorMovement movement;
    public virtual void Execute() {
        // nothing happens
    }
}

public class MoveAction : LevelAction {
    Vector2Int direction;

    public MoveAction(CursorMovement movement, Vector2Int direction) {
        this.movement = movement;
        this.direction = direction;
    }

    public override void Execute() {
        movement.Move(direction);
    }
}

public class InteractAction : LevelAction {
    public InteractAction(CursorMovement movement) {
        this.movement = movement;
    }

    public override void Execute() {
        movement.HandleInteract();
    }
}

public class CancelAction : LevelAction {
    public CancelAction(CursorMovement movement) {
        this.movement = movement;
    }

    public override void Execute() {
        movement.CancelSelection();
    }
}

public class FinishAction : LevelAction {
    public FinishAction(CursorMovement movement) {
        this.movement = movement;
    }

    public override void Execute() {
        movement.Finish();
    }
}
