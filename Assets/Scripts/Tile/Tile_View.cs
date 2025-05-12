using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Tile_View : MonoBehaviour
{
    public Transform positionForPlayer;
    public abstract IEnumerator OnPlayerStepped();
    public abstract IEnumerator OnPlayerLanded();

}
