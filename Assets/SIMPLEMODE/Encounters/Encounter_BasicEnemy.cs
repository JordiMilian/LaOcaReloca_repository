using System.Collections;
using UnityEngine;

public class Encounter_BasicEnemy : MonoBehaviour, IEncounter
{
    public void OnEncounterEnter()
    {
        ChangeEncounterState(EnemyEncounterState.StartBoard);
    }
    public void OnEncounterExit()
    {
        throw new System.NotImplementedException();
    }
    #region GAME FLOW
    GameController_Simple gameController;
    public enum EnemyEncounterState
    {
        Empty, StartBoard, MovingPlayer, FreeMode, ReachedEnd, KilledEnemy, PlayerDied
    }
    EnemyEncounterState currentState = EnemyEncounterState.Empty;
    private void Awake()
    {
        gameController = GameController_Simple.Instance;
    }

    public void ChangeEncounterState(EnemyEncounterState newState)
    {
        //On EXIT this State
        switch (currentState)
        {
            case EnemyEncounterState.StartBoard:
                break;
            case EnemyEncounterState.MovingPlayer:
                if (regularMovingCoroutine != null) { StopCoroutine(regularMovingCoroutine); }
                break;
            case EnemyEncounterState.FreeMode:
                OnFreeModeExit();
                break;
            case EnemyEncounterState.ReachedEnd:
                break;
            case EnemyEncounterState.KilledEnemy:
                break;
            case EnemyEncounterState.PlayerDied:
                break;
        }

        //On ENTER this State
        switch (newState)
        {
            case EnemyEncounterState.StartBoard:
                StartCoroutine(StartGameCoroutine());
                break;
            case EnemyEncounterState.MovingPlayer:
                regularMovingCoroutine = StartCoroutine(OnMovingPlayer_Coroutine());
                break;
            case EnemyEncounterState.FreeMode:
                //zoom out the camera
                //enable shop items purchase
                OnFreeModeEnter();
                break;
            case EnemyEncounterState.ReachedEnd:
                StartCoroutine(OnReachedEnd_Coroutine());
                break;
            case EnemyEncounterState.KilledEnemy:
                //Display some victory screen
                StartCoroutine(KilledEnemy_Coroutine());

                break;
            case EnemyEncounterState.PlayerDied:
                //called when not killing the dude before X turns
                //SHow game over screen
                //restart game
                Debug.Log("PlayerDied");
                RollDiceButton.interactable = false;
                break;
        }
        currentState = newState;
    }
    #region START GAME
    IEnumerator StartGameCoroutine()
    {
        gameController.shopController.ResetAllShopItems();
        OnFreeModeExit();
        yield return gameController.BoardController.StartBoard();
        ChangeEncounterState(EnemyEncounterState.FreeMode);
    }
    #endregion
    #region FREE MODE
    void OnFreeModeEnter()
    {
        RollDiceButton.interactable = true;
        gameController.shopController.EnableShop();
    }
    void OnFreeModeExit()
    {
        RollDiceButton.interactable = false;
        gameController.shopController.DisableShop();
    }
    #endregion
    #region MOVING PLAYER 
    Coroutine regularMovingCoroutine;
    //This mode is entered when the rolling dice button is pressed
    //during this whole coroutine, if we change state the movement coroutine is canceled, so dont worry about switching into FreeMode after all
    public int remainingStepsToTake;
    [Header("stepping audio")]
    [SerializeField] AudioSource StepSound;
    [SerializeField] float addPitchPerStep;
    [Header("Money to Roll")]
    public int MoneyToRoll = 1;
    IEnumerator OnMovingPlayer_Coroutine()
    {
        if(gameController.dicesController.GetDicesToRoll().Count == 0)
        {
            Debug.LogWarning("No dices to roll, please select at least one");
            ChangeEncounterState(EnemyEncounterState.FreeMode);
            yield break;
        }
        Dices_Controller dicesController = gameController.dicesController;
        gameController.RemoveMoney(MoneyToRoll);
        dicesController.SetDicesDraggable(false);
        RollDiceButton.interactable = false;
        yield return dicesController.RollDicesCoroutine();
        dicesController.SetDicesDraggable(true);
        remainingStepsToTake = dicesController.LastRolledValue;

        TMP_rolledDiceAmount.text = remainingStepsToTake.ToString();

        yield return gameController.OnRolledDice_CardEffects.ActivateEffects();

        float basePitch = StepSound.pitch;
        Board_Controller_simple BoardController = gameController.BoardController;

        while (remainingStepsToTake > 0)
        {
            StepSound.pitch += addPitchPerStep;
            StepSound.Play();
            yield return BoardController.L_StepPlayer(true);
            remainingStepsToTake--;
        }
        while (remainingStepsToTake < 0)
        {
            yield return BoardController.L_StepPlayer(false);
            remainingStepsToTake++;
        }
        StepSound.pitch = basePitch;

        yield return BoardController.L_LandPlayerInCurrentPos();

        yield return DealTotalDamage();

        ChangeEncounterState(EnemyEncounterState.FreeMode); 
            
    }
    public void Button_RollDicesTestButton()
    {
        ChangeEncounterState(EnemyEncounterState.MovingPlayer);
    }
    #endregion
    #region REACHED END
    IEnumerator OnReachedEnd_Coroutine()
    {
        yield return new WaitForSeconds(0.5f); 
        yield return DealTotalDamage();
        yield return BoardController.JumpPlayerToStartTile();

        ChangeEncounterState(EnemyEncounterState.FreeMode);
    }
    #endregion
    #region KILLED ENEMY
    [SerializeField] PlayableDirector Timeline_KilledEnemy;
    IEnumerator KilledEnemy_Coroutine()
    {
        Timeline_KilledEnemy.Play();
        float timelineDuration = (float)Timeline_KilledEnemy.duration;
        yield return new WaitForSeconds(timelineDuration);

        AddMoney(10);
        Enemy_MaxHP *= 1.2f;
        Enemy_CurrentHP = Enemy_MaxHP;
        UpdateEnemyHPBar();

        yield return BoardController.JumpPlayerToStartTile();

        yield return OnKilledEnemy_CardEffects.ActivateEffects();

        ChangeEncounterState(EnemyEncounterState.FreeMode);
    }
    #endregion
    #endregion
    

}
