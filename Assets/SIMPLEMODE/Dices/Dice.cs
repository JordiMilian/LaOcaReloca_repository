using UnityEngine;
using System.Collections.Generic;
public class Dice : MonoBehaviour
{
    public bool isSelectedForRoll;
    public List<int> faces = new List<int>();

    public virtual int GetRollDiceValue()
    {
        return faces[Random.Range(0, faces.Count)];
    }
}

