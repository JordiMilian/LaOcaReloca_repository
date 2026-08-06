using NUnit.Framework;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class SaveLoadBoard : MonoBehaviour
{
    [Serializable]
    public class GameSaveInfo
    {
        public List<TileInfo> tiles = new();
        public List<Toy_Info> toys = new();
        public List<Dice> dices = new();
        public int currentIndex;
        public int money;
    }
    [SerializeField] GameSaveInfo currentSave;

    void UpdateCurrentData()
    {
        GameSaveInfo newInfo = new GameSaveInfo();
        Board_Controller_simple board = Board_Controller_simple.Instance;

        //TILES
        foreach(TileController tileC in board.TilesList)
        {
            newInfo.tiles.Add(tileC._Info.GetCopy());
        }

        //CURRENT INDEX
        newInfo.currentIndex = board.PlayerIndex;
        currentSave = newInfo;

        currentSave.money = GameController_Simple.Instance.GetCurrentMoney();

        //TOYS (Per ara centrarse en les tiles)
         
        foreach(Toy_Controller toyC in ToysManager.Instance.instantiatedToys)
        {
            if (toyC.isActive)
            {
                newInfo.toys.Add(toyC._Profile);
            }
        }
        

        //TO DO MISSING DICES

    }

    void SaveCurrentData(int index)
    {
        UpdateCurrentData();

        byte[] bytes = SerializationUtility.SerializeValue(
        currentSave,
        DataFormat.JSON
        );

        string json = System.Text.Encoding.UTF8.GetString(bytes);

        PlayerPrefs.SetString($"Save{index}", json);
        PlayerPrefs.Save();
    }
    void LoadSave(int index)
    {
        string json = PlayerPrefs.GetString($"Save{index}");

        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);

        currentSave = SerializationUtility.DeserializeValue<GameSaveInfo>(
            bytes,
            DataFormat.JSON);

        GameController_Simple game = GameController_Simple.Instance;
        StartCoroutine(game.C_LoadBoard(currentSave));
        game.ForceSetMoney(currentSave.money);
    }

    void AttemptSave(int number)
    {
        if (GameController_Simple.Instance.currentGameState == GameState.FreeMode)
        {
            SaveCurrentData(number);
        }
        else { Debug.Log("Not in freemode"); }
    }
    void AttemptLoad(int number)
    {
        if (GameController_Simple.Instance.currentGameState == GameState.FreeMode)
        {
            if (PlayerPrefs.HasKey("Save" + number))
            {

               LoadSave(number);
            }
            else
            {
                Debug.LogWarning("Empty Save");
            }
        }
        else { Debug.Log("Not in freemode"); }
    }

    private void Update()
    {
        if (Keyboard.current[Key.S].isPressed)
        {
            if (Keyboard.current[Key.Digit0].wasPressedThisFrame) 
            {
                AttemptSave(0);
                Debug.Log("Saved at 0");
            }
            if (Keyboard.current[Key.T].wasPressedThisFrame)
            {
                Debug.Log("save test");
                UpdateCurrentData();
            }
        }
        if (Keyboard.current[Key.L].isPressed)
        {
            if (Keyboard.current[Key.Digit0].wasPressedThisFrame)
            {
                AttemptLoad(0);
                Debug.Log("Loaded at 0");
            }
            if (Keyboard.current[Key.T].wasPressedThisFrame)
            {
                Debug.Log("load test");
                GameController_Simple game = GameController_Simple.Instance;
                StartCoroutine(game.C_LoadBoard(currentSave));
                game.ForceSetMoney(currentSave.money);
            }
        }
    }
}
