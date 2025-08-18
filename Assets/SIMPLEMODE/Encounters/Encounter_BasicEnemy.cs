using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class Encounter_BasicEnemy : MonoBehaviour, IEncounter
{
    public int Enemy_MaxHP = 100;
    [SerializeField] PlayableDirector Timeline_KilledEnemy, Timeline_SpawnEnemy;
    GameController_Simple gameController;
    #region ENTER/EXIT ENCOUNTER
    public IEnumerator OnEncounterEnter()
    {
        Dices_Controller.Instance.Button_RollDice.onClick.AddListener(Button_RollDicesTestButton);
        Dices_Controller.Instance.DisableRollButton();

        Timeline_SpawnEnemy.Play();
        yield return new WaitForSeconds((float)Timeline_SpawnEnemy.duration);

        gameController.AddMoney(10);
        gameController.SetNewEnemyMaxHP(Enemy_MaxHP);

        ChangeEnemyEncounterState(EnemyEncounterState.FreeMode);
    }
    public IEnumerator OnEncounterExit()
    {
        Dices_Controller.Instance.Button_RollDice.onClick.RemoveListener(Button_RollDicesTestButton);
        yield break;
    }
    #endregion
    #region GAME FLOW
    public enum EnemyEncounterState
    {
        Empty, MovingPlayer, FreeMode, ReachedEnd, KilledEnemy, PlayerDied
    }
    EnemyEncounterState currentState = EnemyEncounterState.Empty;
    private void Awake()
    {
        gameController = GameController_Simple.Instance;
    }

    public void ChangeEnemyEncounterState(EnemyEncounterState newState)
    {
        //On EXIT this State
        switch (currentState)
        {
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
            case EnemyEncounterState.MovingPlayer:
                regularMovingCoroutine = StartCoroutine(C_MovingPlayer());
                break;
            case EnemyEncounterState.FreeMode:
                OnFreeModeEnter();
                break;
            case EnemyEncounterState.ReachedEnd:
                StartCoroutine(OnReachedEnd_Coroutine());
                break;
            case EnemyEncounterState.KilledEnemy:
                StartCoroutine(KilledEnemy_Coroutine());
                break;
            case EnemyEncounterState.PlayerDied:
                Debug.Log("PlayerDied");
                Dices_Controller.Instance.DisableRollButton();
                break;
        }
        currentState = newState;
    }
    #region FREE MODE
    void OnFreeModeEnter()
    {
        gameController.dicesController.EnableRollButton();
        gameController.shopController.EnableShop();
    }
    void OnFreeModeExit()
    {
        gameController.shopController.DisableShop();
    }
    #endregion
    #region MOVING PLAYER 

    //This mode is entered when the rolling dice button is pressed
    //during this whole coroutine, if we change state the movement coroutine is canceled, so dont worry about switching into FreeMode after all
    Coroutine regularMovingCoroutine;
    int remainingStepsToTake;
    [Header("stepping audio")]
    [SerializeField] AudioSource StepSound;
    [SerializeField] float addPitchPerStep;
    [Header("Money to Roll")]
    public int MoneyToRoll = 1;
    IEnumerator C_MovingPlayer()
    {
        if(gameController.dicesController.GetDicesToRoll().Count == 0)
        {
            Debug.LogWarning("No dices to roll, please select at least one");
            ChangeEnemyEncounterState(EnemyEncounterState.FreeMode);
            yield break;
        }
        Dices_Controller dicesController = gameController.dicesController;
        Board_Controller_simple BoardController = gameController.BoardController;

        gameController.RemoveMoney(MoneyToRoll);
        yield return dicesController.RollDicesCoroutine();
        remainingStepsToTake = dicesController.LastRolledValue;

        yield return gameController.OnRolledDice_CardEffects.ActivateEffects();

        float basePitch = StepSound.pitch;
        
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

        yield return gameController.C_DealTotalDamage();

        ChangeEnemyEncounterState(EnemyEncounterState.FreeMode); 
            
    }
    public void Button_RollDicesTestButton()
    {
        ChangeEnemyEncounterState(EnemyEncounterState.MovingPlayer);
    }
    #endregion
    #region REACHED END
    IEnumerator OnReachedEnd_Coroutine()
    {
        yield return new WaitForSeconds(0.5f); 
        yield return gameController.C_DealTotalDamage();
        yield return gameController.BoardController.JumpPlayerToStartTile();

        ChangeEnemyEncounterState(EnemyEncounterState.FreeMode);
    }
    #endregion
    #region KILLED ENEMY
    
    public virtual IEnumerator KilledEnemy_Coroutine()
    {
        Timeline_KilledEnemy.Play();
        float timelineDuration = (float)Timeline_KilledEnemy.duration;
        yield return new WaitForSeconds(timelineDuration);

        yield return gameController.OnKilledEnemy_CardEffects.ActivateEffects();

        yield return gameController.BoardController.JumpPlayerToStartTile();


        gameController.LoadNextEncounter();
    }
    #endregion
    #endregion
    

}
