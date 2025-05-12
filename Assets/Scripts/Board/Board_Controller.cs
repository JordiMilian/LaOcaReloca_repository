using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Board_Controller : MonoBehaviour
{
    public Board_Data Data;
    public Board_View View;
    public Player_Controller playerController;

    List<Tile_Controller> tileControllers = new List<Tile_Controller>();
    public IEnumerator StepPlayer(bool positiveStep) //If false, its negative step
    {
        if (positiveStep && Data.PlayerIndex == Data.TilesList.Count - 1) { yield break; }
        if (!positiveStep && Data.PlayerIndex == 0) { yield break; }

        int stepAmount = positiveStep ? 1 : -1;
        Data.PlayerIndex += stepAmount;

        yield return StartCoroutine(playerController.MovePlayerTo(CurrentPlayerTile));
        yield return StartCoroutine(CurrentPlayerTile.OnPlayerStepped());
    }
    public IEnumerator LandPlayerInCurrentPos()
    {
        Debug.Log($"Landed in:{Data.PlayerIndex}");
        yield return StartCoroutine(CurrentPlayerTile.OnPlayerLanded());
    }
    public IEnumerator JumpPlayerTo(int newIndex)
    {
        if(newIndex > tileControllers.Count -1 || newIndex < 0) { Debug.LogError("ERROR: Tried to jump into invalid index"); yield break; }

        Data.PlayerIndex = newIndex;
        yield return StartCoroutine(playerController.MovePlayerTo(tileControllers[Data.PlayerIndex]));
        yield return StartCoroutine(tileControllers[Data.PlayerIndex].OnPlayerStepped());
    }

    Tile_Controller CurrentPlayerTile
    {
        get { return tileControllers[Data.PlayerIndex]; }
    }

}
