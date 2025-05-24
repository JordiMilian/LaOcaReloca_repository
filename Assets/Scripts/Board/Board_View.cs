using System.Collections;
using UnityEngine;

public class Board_View : MonoBehaviour
{
    [SerializeField] float TimeToCreateBoard = 1;
    Board_Controller BoardController;
    private void Awake()
    {
        BoardController = GetComponent<Board_Controller>();
    }
    public IEnumerator AnimateStartingBoard()
    {
        float delayBetweenTiles = TimeToCreateBoard / BoardController.TileControllers.Count;

        foreach (Tile_Controller controller in BoardController.TileControllers)
        {
            controller.gameObject.SetActive(false);
        }
        foreach (Tile_Controller controller in BoardController.TileControllers)
        {
            yield return new WaitForSeconds(delayBetweenTiles);
            controller.gameObject.SetActive(true);
            controller.AppearTile();
        }
    }

}
