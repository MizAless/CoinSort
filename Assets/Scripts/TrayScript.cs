using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TrayScript : MonoBehaviour
{
    [SerializeField] private GameObject CoinsObject;
    [SerializeField] private GameObject CoinPrefab;
    [SerializeField] private GameObject MergeLight;

    private static int MaxCoinCount = 10;

    public List<CoinScript> Coins = new List<CoinScript>();

    public bool IsReadyToMerge { get; set; } = false;

    public void GetCoinsInTray()
    {
        Coins.Clear();
        foreach (Transform obj in CoinsObject.transform)
        {
            var coin = obj.GetComponent<CoinScript>();
            if (coin != null)
            {
                Coins.Add(coin);
            }
        }
    }

    public void DestroyCoins()
    {
        foreach (Transform obj in CoinsObject.transform)
        {
            Destroy(obj.gameObject);
        }
        GetCoinsInTray();
        Coins.Clear();
    }

    public void SetCoinsInTray()
    {
        DestroyCoins();
        float offset = 0;
        foreach (CoinScript coin in Coins)
        {
            //var newCoin = Instantiate(CoinPrefab, CoinsObject.transform.position + new Vector3(-0.03799993f, 0.1491f, 0.5f - offset), CoinPrefab.transform.rotation);
            var newCoin = Instantiate(CoinPrefab, CoinsObject.transform);
            newCoin.transform.position += new Vector3(-0.03799993f, 0.1491f, offset);
            newCoin.GetComponent<CoinScript>().UpdateCoinLevel(coin.GetLevel());

            offset += 0.15f;
        }
    }

    public bool AddCoins(int count, int level)
    {
        if (Coins.Count + count > MaxCoinCount)
            return false;

        float offset = 0.56f - Coins.Count * 0.12f;

        for (int i = 0; i < count; i++)
        {
            var newCoin = Instantiate(CoinPrefab, CoinsObject.transform);
            newCoin.transform.localPosition = Vector3.zero;
            newCoin.transform.localPosition += new Vector3(-0.03799993f, 0.1491f, offset);
            newCoin.GetComponent<CoinScript>().UpdateCoinLevel(level);
            offset -= 0.12f;
        }

        GetCoinsInTray();

        return true;
    }

    public bool RelocateCoinsInTray(TrayScript otherTray)
    {
        //for (int i = 0; i < Coins.Count; i++)
        //{
        //    //var newCoin = new CoinScript(Coins[i].GetLevel());
        //    //otherTray.Coins.Add(newCoin);
        //    otherTray.Coins.Add(Coins[i]);
        //    //otherTray.Coins[i].UpdateCoinLevel(Coins[i].GetLevel());
        //}
        //float offset = 0;

        List<int> coinIndexes = GetSameLevelCoinIndex();
        List<int> coinIndexesForDestroy = new List<int>();

        if (coinIndexes.Count == 0)
            return false;

        foreach (var i in coinIndexes)
        {
            if (otherTray.AddCoins(1, Coins[i].GetLevel()))
            {
                coinIndexesForDestroy.Add(i);
            }
        }
        foreach (var i in coinIndexesForDestroy)
        {
            Destroy(Coins[i].gameObject);
        }
        GetCoinsInTray();


        //for (int i = 0; i < Coins.Count; i++)
        //{
        //    otherTray.AddCoins(1, Coins[i].GetLevel());
        //}

        //Coins.Clear();
        //DestroyCoins();
        //otherTray.SetCoinsInTray();
        //otherTray.GetCoinsInTray();
        return true;
    }

    private void CheckForMerge()
    {
        MergeLight.SetActive(IsReadyToMerge);
    }

    public void Merge()
    {
        int nextCoinLevel = Coins.First().GetLevel() + 1;
        DestroyCoins();
        //GetCoinsInTray();
        Coins.Clear();
        AddCoins(2, nextCoinLevel);
    }

    public List<int> GetSameLevelCoinIndex()
    {
        List<int> result = new List<int>(); 
        if (Coins.Count == 1)
        {
            result.Add(0);
            return result;
        }

        int lastLevel = Coins.Last().GetLevel();

        for (int i = Coins.Count - 1; i >= 0; i--)
        {
            if (Coins[i].GetLevel() == lastLevel)
            {
                result.Add(i);
            }
            else
            {
                break;
            }
        }
        return result;
    }


    void Awake()
    {
        GetCoinsInTray();
    }



    void Update()
    {
        GetCoinsInTray();
        CheckForMerge();
    }
}
