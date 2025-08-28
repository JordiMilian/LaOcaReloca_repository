using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Playables;

public class Encounter_BasicEnemy : MonoBehaviour, IEncounter
{
    [SerializeField] PlayableDirector cutscene_SpawnEnemy, cutscene_KilledEnemy;
    GameController_Simple gameController;
    CamerasManager cameras;
    public float MaxHp;
    public int MoneyReward;
    public IEnumerator OnEncounterEnter()
    {

        gameController = GameController_Simple.Instance;
        cameras = CamerasManager.instance;

        Dices_Controller.Instance.Button_Rolldices.onClick.AddListener(GameController_Simple.Instance.ChangeStateToMoving);
        //Load board if it's not loaded

        if(gameController.BoardController.isBoardAssembled == false)
        {
            cameras.SetCameraPriority("CinemachineCamera_Board", 15);
            yield return gameController.BoardController.C_AsembleBoard();
            yield return new WaitForSeconds(0.5f);
            cameras.SetCameraPriority("CinemachineCamera_Board", 0);
        }


        cameras.SetCameraPriority("CinemachineCamera_Goose", 15);
        cutscene_SpawnEnemy.Play();
        yield return new WaitForSeconds((float)cutscene_SpawnEnemy.duration);
        cameras.SetCameraPriority("CinemachineCamera_Goose", 0);

        //Show UI for EnemyEncounter
        gameController.SetNewEnemyMaxHP(MaxHp);

       gameController.ChangeGameState(GameState.FreeMode);
            
    }
    public IEnumerator OnEncounterExit()
    {
        Dices_Controller.Instance.Button_Rolldices.onClick.RemoveListener(GameController_Simple.Instance.ChangeStateToMoving);
        Dices_Controller.Instance.DisableRollButtons();

        cameras.SetCameraPriority("CinemachineCamera_Goose", 15);
        cutscene_KilledEnemy.Play();
        yield return new WaitForSeconds((float)cutscene_KilledEnemy.duration);
        cameras.SetCameraPriority("CinemachineCamera_Goose", 0);

        yield return gameController.OnKilledEnemy_CardEffects.ActivateEffects();

       

        gameController.AddMoney(MoneyReward);

    }

    

}
