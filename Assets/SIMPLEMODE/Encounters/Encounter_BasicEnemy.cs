using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class Encounter_BasicEnemy : MonoBehaviour, IEncounter
{
    [SerializeField] PlayableDirector cutscene_SpawnEnemy, cutscene_KilledEnemy;
    GameController_Simple gameController;
    public float MaxHp;
    public int MoneyReward;
    public IEnumerator OnEncounterEnter()
    {
        gameController = GameController_Simple.Instance;

        Dices_Controller.Instance.Button_Rolldices.onClick.AddListener(GameController_Simple.Instance.ChangeStateToMoving);
        //Load board if it's not loaded

        cutscene_SpawnEnemy.Play();
        yield return new WaitForSeconds((float)cutscene_SpawnEnemy.duration);

        //Show UI for EnemyEncounter
        gameController.SetNewEnemyMaxHP(MaxHp);

       gameController.ChangeGameState(GameState.FreeMode);
            
    }
    public IEnumerator OnEncounterExit()
    {
        Dices_Controller.Instance.Button_Rolldices.onClick.RemoveListener(GameController_Simple.Instance.ChangeStateToMoving);
        Dices_Controller.Instance.DisableRollButtons();

        cutscene_KilledEnemy.Play();
        yield return new WaitForSeconds((float)cutscene_KilledEnemy.duration);

        yield return gameController.OnKilledEnemy_CardEffects.ActivateEffects();

       

        gameController.AddMoney(MoneyReward);

    }

    

}
