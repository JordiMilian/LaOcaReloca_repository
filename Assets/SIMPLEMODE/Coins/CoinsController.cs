using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class CoinsController : MonoBehaviour
{
    [SerializeField] GameController_Simple gameController;
    [SerializeField] GameObject coinPrefab;
    [SerializeField] float spawnRadius = 2f;
    [SerializeField] float delayBetweenCoins = 0.1f;
    List<GameObject> spawnedCoins = new();
    Coroutine updateCoinsCoroutine;
    int TargetCoins;
    private void OnEnable()
    {
        gameController.OnMoneyUpdated.AddListener(OnCoinsUpdated);
    }
    private void OnDisable()
    {
        gameController.OnMoneyUpdated.RemoveListener(OnCoinsUpdated);
    }
    private void Start()
    {
       OnCoinsUpdated(gameController.GetCurrentMoney());
    }

    void OnCoinsUpdated(int newValue)
    {
        SortCoinsByHeight();

        TargetCoins = newValue;
        if(updateCoinsCoroutine == null)
        {
            StartCoroutine(updatingCoinsToTarget());
        }

        IEnumerator updatingCoinsToTarget()
        {
            while (spawnedCoins.Count != TargetCoins)
            {
                int diference = TargetCoins - spawnedCoins.Count;

                if(diference < 0)
                {
                    RemoveCoin();
                }
                else
                {
                    SpawnCoin();
                }
                yield return new WaitForSeconds(delayBetweenCoins);
            }
        }

    }
    public void SpawnCoin()
    {
        Vector3 randomPos = transform.position + (Random.insideUnitSphere * spawnRadius);
        Quaternion randomRot = Random.rotation;
        GameObject newCoin = Instantiate(coinPrefab, randomPos, randomRot, transform);
        spawnedCoins.Add(newCoin);

    }
    public void RemoveCoin()
    {
        Transform coinTf = spawnedCoins[spawnedCoins.Count-1].transform;
        coinTf.GetComponent<Rigidbody>().isKinematic = true;
        coinTf.DOMove(transform.position, .5f);
        spawnedCoins.RemoveAt(spawnedCoins.Count-1);
        Destroy(coinTf.gameObject, .6f);

    }
    void SortCoinsByHeight()
    {
        spawnedCoins.Sort((a, b) => a.transform.position.y.CompareTo(b.transform.position.y));
    }
}
