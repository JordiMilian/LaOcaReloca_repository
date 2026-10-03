using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class Encounter_BasicEnemy : Encounter
{
    [SerializeField] PlayableDirector cutscene_SpawnEnemy, cutscene_KilledEnemy;
    protected GameController_Simple gameController;
    CamerasManager cameras;
    [HideInInspector]public float MaxHp;
    public int MoneyReward;
    public override IEnumerator OnEncounterEnter()
    {
        yield return base.OnEncounterEnter();

        gameController = GameController_Simple.Instance;
        cameras = CamerasManager.instance;

        Dices_Controller.Instance.Button_Rolldices.onClick.AddListener(GameController_Simple.Instance.ChangeStateToRollingDice);
        Dices_Controller.Instance.SetMainButtonText("Roll Dices");
        gameController.SetRemainingRolls(gameController.MaxRollsPerEncounter);
        //Load board if it's not loaded

        if(gameController.BoardController.isBoardAssembled == false)
        {
            yield return gameController.BoardController.C_AsembleBoard();
            yield return new WaitForSeconds(0.5f);
        }


        cameras.SetCameraPriority("CinemachineCamera_Goose", 15);
        cutscene_SpawnEnemy.Play();
        yield return new WaitForSeconds((float)cutscene_SpawnEnemy.duration);
        cameras.SetCameraPriority("CinemachineCamera_Goose", 0);

        //Show UI for EnemyEncounter
        gameController.SetNewEnemyMaxHP(MaxHp);

       gameController.ChangeGameState(GameState.FreeMode);
            
    }
    public override IEnumerator OnEncounterExit()
    {
        yield return base.OnEncounterExit();

        Dices_Controller.Instance.Button_Rolldices.onClick.RemoveListener(GameController_Simple.Instance.ChangeStateToRollingDice);
        //Dices_Controller.Instance.DisableRollButtons();

        cameras.SetCameraPriority("CinemachineCamera_Goose", 15);
        cutscene_KilledEnemy.Play();
        yield return new WaitForSeconds((float)cutscene_KilledEnemy.duration);
        cameras.SetCameraPriority("CinemachineCamera_Goose", 0);

        gameController.OnKilledEnemy?.Invoke();
        yield return gameController.OnKilledEnemy_CardEffects.C_ActivateEffects();

       

        gameController.AddMoney(MoneyReward);
        gameController.AddMoney(gameController.MoneyPerRemainignRoll * gameController.RollsRemaining);

    }
    public override bool MeetsRequirementsToSpawn() { return true; }

    public override string GetTooltipDescription()
    {
        return "basic enemy, no special skills";
    }
}
