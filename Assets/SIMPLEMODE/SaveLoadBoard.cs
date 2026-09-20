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

        //MONEY
        currentSave.money = GameController_Simple.Instance.GetCurrentMoney();

         //TOYS
        foreach(Toy_Controller toyC in ToysManager.Instance.instantiatedToys)
        {
            if (toyC.isActive)
            {
                newInfo.toys.Add(toyC._Profile);
            }
        }
        

        //TO DO MISSING DICES
        //TO DO MISSING ENCOUNTER

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
            Debug.Log("Saved at " + number );
        }
        else { Debug.Log("Failed save: not in freemode"); }
    }
    void AttemptLoad(int number)
    {
        if (GameController_Simple.Instance.currentGameState == GameState.FreeMode)
        {
            if (PlayerPrefs.HasKey("Save" + number))
            {
               LoadSave(number);
               Debug.Log("Loaded at "+number);
            }
            else
            {
                Debug.LogWarning("Failed Load: Empty Save "+number);
            }
        }
        else { Debug.Log("Failed Load: Not in freemode"); }
    }

    private void Update()
    {
        if (Keyboard.current[Key.S].isPressed)
        {
            if (Keyboard.current[Key.Digit0].wasPressedThisFrame) { AttemptSave(0); }
            if (Keyboard.current[Key.Digit1].wasPressedThisFrame) { AttemptSave(1); }
            if (Keyboard.current[Key.Digit2].wasPressedThisFrame) { AttemptSave(2); }
            if (Keyboard.current[Key.Digit3].wasPressedThisFrame) { AttemptSave(3); }
            if (Keyboard.current[Key.Digit4].wasPressedThisFrame) { AttemptSave(4); }
            if (Keyboard.current[Key.Digit5].wasPressedThisFrame) { AttemptSave(5); }
            if (Keyboard.current[Key.Digit6].wasPressedThisFrame) { AttemptSave(6); }
            if (Keyboard.current[Key.Digit7].wasPressedThisFrame) { AttemptSave(7); }
            if (Keyboard.current[Key.Digit8].wasPressedThisFrame) { AttemptSave(8); }
            if (Keyboard.current[Key.Digit9].wasPressedThisFrame) { AttemptSave(9); }

            if (Keyboard.current[Key.T].wasPressedThisFrame)
            {
                Debug.Log("save test");
                UpdateCurrentData();
            }
        }
        if (Keyboard.current[Key.L].isPressed)
        {
            if (Keyboard.current[Key.Digit0].wasPressedThisFrame) { AttemptLoad(0); }
            if (Keyboard.current[Key.Digit1].wasPressedThisFrame) { AttemptLoad(1); }
            if (Keyboard.current[Key.Digit2].wasPressedThisFrame) { AttemptLoad(2); }
            if (Keyboard.current[Key.Digit3].wasPressedThisFrame) { AttemptLoad(3); }
            if (Keyboard.current[Key.Digit4].wasPressedThisFrame) { AttemptLoad(4); }
            if (Keyboard.current[Key.Digit5].wasPressedThisFrame) { AttemptLoad(5); }
            if (Keyboard.current[Key.Digit6].wasPressedThisFrame) { AttemptLoad(6); }
            if (Keyboard.current[Key.Digit7].wasPressedThisFrame) { AttemptLoad(7); }
            if (Keyboard.current[Key.Digit8].wasPressedThisFrame) { AttemptLoad(8); }
            if (Keyboard.current[Key.Digit9].wasPressedThisFrame) { AttemptLoad(9); }

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
