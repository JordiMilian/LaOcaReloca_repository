using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Player_Controller : MonoBehaviour
{
    Player_View view;
    Player_Data data;
    private void Awake()
    {
        view = GetComponent<Player_View>();
        data = new Player_Data();
    }
    public IEnumerator MovePlayerTo(Tile_Controller tile)
    {
        if(view == null) { view = GetComponent<Player_View>();}
        if(tile == null) { Debug.Log("Tile???"); }
        yield return view.StepPlayerTo(tile.transform.position);
    }
    public void LandPlayerInCurrentPos()
    {
        view.LandPlayerHere();
    }
   
}
