using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadBoard : MonoBehaviour
{
    public class GameSaveInfo
    {
        public List<Tile_Profile> tiles;
        public List<Toy_Profile> toys;
        public List<Dice> dices;
        public int currentIndex;
        public int diceRollsRemaining;
    }
}
