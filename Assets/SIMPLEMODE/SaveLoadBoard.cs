using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveLoadBoard : MonoBehaviour
{
    public class GameSaveInfo
    {
        public List<TileStateClass> tiles = new();
        public List<Toy_Profile> toys = new();
        public List<Dice> dices = new();
        public int currentIndex;
    }
    GameSaveInfo currentSave;

    void UpdateCurrentData()
    {
         GameSaveInfo newInfo = new GameSaveInfo();
        Board_Controller_simple board = Board_Controller_simple.Instance;

        //TILES
        foreach(TileController tileC in board.TilesList)
        {
            newInfo.tiles.Add(Instantiate(tileC._Profile));
        }

        //CURRENT INDEX
        newInfo.currentIndex = board.PlayerIndex;

        //TOYS (Per ara centrarse en les tiles)
        /* 
        foreach(Toy_Controller toyC in ToysManager.Instance.instantiatedToys)
        {
            if (toyC.isActive)
            {
                newInfo.toys.Add(toyC._Profile);
            }
        }
        */

        //TO DO MISSING DICES

    }

    void SaveCurrentData(int index)
    {
        UpdateCurrentData();
        string jsonString = JsonUtility.ToJson(currentSave);
        PlayerPrefs.SetString("Save" + index, jsonString);
        PlayerPrefs.Save();
    }



    private void Update()
    {
        if (Keyboard.current[Key.S].isPressed)
        {
            if (Keyboard.current[Key.Digit0].wasPressedThisFrame) 
            {
                AttemptSave(0);
            }
        }
        if (Keyboard.current[Key.L].isPressed)
        {
            if (Keyboard.current[Key.Digit0].wasPressedThisFrame)
            {
                AttemptLoad(0);
                Debug.Log("Loaded 0");
            }
        }

        void AttemptSave(int number)
        {
            if (GameController_Simple.Instance.currentGameState == GameState.FreeMode)
            {
                SaveCurrentData(number);
                Debug.Log("Saved at "+ number);
            }
        }
        void AttemptLoad(int number)
        {
            if (GameController_Simple.Instance.currentGameState == GameState.FreeMode)
            {
                if(PlayerPrefs.HasKey("Save"+number))
                {
                    string savedJson = PlayerPrefs.GetString("Save" + number);
                    currentSave = JsonUtility.FromJson<GameSaveInfo>(savedJson);

                    StartCoroutine(GameController_Simple.Instance.C_LoadBoard(currentSave));
                }
                else
                {
                    Debug.LogWarning("Empty Save");
                }

            }
        }
    }
}
