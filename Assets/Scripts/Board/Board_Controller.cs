using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;
using JetBrains.Annotations;

public class Board_Controller : MonoBehaviour
{
    public Board_Data Data;
    public Board_View View;
    public Player_Controller playerController;

    public List<Tile_Controller> TileControllers = new List<Tile_Controller>();
    public void InitializeBoard(Board_Data boardData, List<Tile_Controller> tileControllers)
    {
        Data = boardData;
        for (int i = 0; i < tileControllers.Count; i++)
        {
            tileControllers[i].InitializeTile(Data.TilesList[i]);
        } 
        View = GetComponent<Board_View>();
        playerController = GetComponentInChildren<Player_Controller>();
        TileControllers = tileControllers;
    }
    public IEnumerator StepPlayer(bool positiveStep) //If false, its negative step
    {
        if (positiveStep && Data.PlayerIndex == Data.TilesList.Count - 1) { yield break; }
        if (!positiveStep && Data.PlayerIndex == 0) { yield break; }

        int stepAmount = positiveStep ? 1 : -1;
        Data.PlayerIndex += stepAmount;

        yield return playerController.MovePlayerTo(CurrentPlayerTile);
        yield return CurrentPlayerTile.OnPlayerStepped();
    }
    public IEnumerator LandPlayerInCurrentPos()
    {
        Debug.Log($"Landed in:{Data.PlayerIndex}");
        yield return CurrentPlayerTile.TriggerMainEffect();
        playerController.LandPlayerInCurrentPos();
    }
    public IEnumerator JumpPlayerTo(int newIndex, bool triggerLand)
    {
        if(newIndex > TileControllers.Count -1 || newIndex < 0) { Debug.LogError("ERROR: Tried to jump into invalid index"); yield break; }

        Data.PlayerIndex = newIndex;
        yield return StartCoroutine(playerController.MovePlayerTo(TileControllers[Data.PlayerIndex]));
        playerController.LandPlayerInCurrentPos();
        yield return StartCoroutine(TileControllers[Data.PlayerIndex].OnPlayerStepped());
        if (triggerLand) { yield return StartCoroutine(TileControllers[Data.PlayerIndex].TriggerMainEffect()); }
    }

    Tile_Controller CurrentPlayerTile
    {
        get { return TileControllers[Data.PlayerIndex]; }
    }

}
