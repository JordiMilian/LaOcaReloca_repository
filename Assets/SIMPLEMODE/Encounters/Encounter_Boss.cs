using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Playables;

public class Encounter_Boss : MonoBehaviour, IEncounter
{
    [SerializeField] PlayableDirector cutscene_SpawnBoss, cutscene_KilledBoss;
    protected GameController_Simple gameController;
    CamerasManager cameras;
    public float MaxHp;
    public int MoneyReward;

    //Per ara ho farem aixi que es mes facil.
    //Si en algun moment algun boss requereix alguna entrada diferent (passa algo durant la entrad whatever),
    //mirem de fer una implementacio sense un pare boss en comú 
    public virtual void ActivateSpecialBossEffect()
    { }
    public virtual void DeactivateSpecialBossEffect() 
    { }
    public IEnumerator OnEncounterEnter()
    {
        gameController = GameController_Simple.Instance;
        cameras = CamerasManager.instance;

        Dices_Controller.Instance.Button_Rolldices.onClick.AddListener(GameController_Simple.Instance.ChangeStateToRollingDice);
        Dices_Controller.Instance.SetMainButtonText("Roll Dices");
        gameController.SetRemainingRolls(gameController.MaxRollsPerEncounter);
        //Load board if it's not loaded

        if (gameController.BoardController.isBoardAssembled == false)
        {
            yield return gameController.BoardController.C_AsembleBoard();
            yield return new WaitForSeconds(0.5f);
        }


        cameras.SetCameraPriority("CinemachineCamera_Goose", 15);
        cutscene_SpawnBoss.Play();
        yield return new WaitForSeconds((float)cutscene_SpawnBoss.duration);
        cameras.SetCameraPriority("CinemachineCamera_Goose", 0);

        //Show UI for EnemyEncounter

        ActivateSpecialBossEffect();
        gameController.ChangeGameState(GameState.FreeMode);

    }

    public IEnumerator OnEncounterExit()
    {
        Dices_Controller.Instance.Button_Rolldices.onClick.RemoveListener(GameController_Simple.Instance.ChangeStateToRollingDice);
        //Dices_Controller.Instance.DisableRollButtons();

        cameras.SetCameraPriority("CinemachineCamera_Goose", 15);
        cutscene_KilledBoss.Play();
        yield return new WaitForSeconds((float)cutscene_KilledBoss.duration);
        cameras.SetCameraPriority("CinemachineCamera_Goose", 0);

        gameController.OnKilledEnemy?.Invoke();
        yield return gameController.OnKilledEnemy_CardEffects.C_ActivateEffects();

        DeactivateSpecialBossEffect();

        gameController.AddMoney(MoneyReward);
        gameController.AddMoney(gameController.MoneyPerRemainignRoll * gameController.RollsRemaining);
    }


}
