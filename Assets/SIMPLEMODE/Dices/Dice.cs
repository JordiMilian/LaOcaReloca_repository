using UnityEngine;
using System.Collections.Generic;
using System;
public class Dice : MonoBehaviour
{
    [Serializable]
    public struct DiceFaces
    {
        public int faceValue;
        public Transform faceTransform;
    }
    [SerializeField] DiceFaces[] diceFaces;
    public bool isSelectedForRoll;
    public int faceUpValue;
    public Rigidbody rb;
    public bool isMoving;

    public virtual int GetRollDiceValue()
    {
        return diceFaces[UnityEngine.Random.Range(0, diceFaces.Length)].faceValue;
    }
    private void Update()
    {
        isMoving = !rb.IsSleeping();

        if(!isMoving)
        {
            float highestHeight = Mathf.NegativeInfinity;
            int highestIndex = -1;
            for (int i = 0; i < diceFaces.Length; i++)
            {
                Transform faceTf = diceFaces[i].faceTransform;
                if (faceTf.position.y > highestHeight)
                {
                    highestHeight = faceTf.position.y;
                    highestIndex = i;

                }
            }
            faceUpValue = diceFaces[highestIndex].faceValue;
        }
       
    }
}

