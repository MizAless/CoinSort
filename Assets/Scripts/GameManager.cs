using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] public List<TrayScript> trayList;
    [SerializeField] public PlayerRay playerRay;
    [SerializeField] public GameObject ShuffleButton;
    [SerializeField] public GameObject DealButton;
    [SerializeField] public GameObject CoinsDealSpawn;
    [SerializeField] public GameObject CoinPrefab;
    public Dictionary<int, int> NeedCoinsToMerge = new Dictionary<int, int>();// List where key == CoinLevel, value == CoinsToMergeInTray


    private List<CoinScript> Coins = new List<CoinScript>();
    private PlayerLevel _playerLevel;
    private ÑoinStorage _ñoinStorage;

    private void InitNeedCoinsToMerge()
    {
        NeedCoinsToMerge.Clear();
        for (int i = 1; i <= _playerLevel.playerLevel; i++)
        {
            NeedCoinsToMerge.Add(i, 0);
        }
    }

    private void InitUnlockTrayCost()
    {
        int i = 0;
        foreach (var tray in trayList)
        {
            var lockTray = tray.GetComponent<LockTrayScript>();
            if (lockTray.isLocked)
            {
                lockTray.SetUnlockCost(250 + i * 150);
                i++;
            }
        }
    }


    void Start()
    {
        _playerLevel = GetComponent<PlayerLevel>();
        _ñoinStorage = GetComponent<ÑoinStorage>();
        InitUnlockTrayCost();
        CalcNeedCoinsToMerge();
    }


    void Update()
    {
        CheckTraysForMerge();
        UpdateCoinsList();
        CalcNeedCoinsToMerge();

        if (Input.GetKey(KeyCode.Space))
        {
            MergeTrays();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Deal();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            DeleteAllCoins();
        }

        ShuffleButton.SetActive(CheckFullTrays());
        DealButton.GetComponent<Button>().interactable = _ñoinStorage.coinsCount >= _ñoinStorage.DealCoinPrice;
        //DealButton.GetComponent<Button>().interactable = false;

    }

    public void CalcNeedCoinsToMerge()
    {
        InitNeedCoinsToMerge();
        for (int i = 1; i <= NeedCoinsToMerge.Count; i++)
        {
            NeedCoinsToMerge[i] = 10 - Coins.Where(coin => coin.GetLevel() == i).Count();
            //print("Level " +  i  + " coins needs to merge " + NeedCoinsToMerge[i] + " coins");
        }
    }

    public void UpdateCoinsList()
    {
        Coins.Clear();
        foreach (TrayScript tray in trayList)
        {
            foreach (CoinScript coin in tray.Coins)
            {
                Coins.Add(coin);
            }
        }
    }

    public void MergeTrays()
    {
        int count = 0;
        foreach (TrayScript tray in trayList)
        {
            playerRay.UnselectAll();
            if (tray.IsReadyToMerge && !tray.GetComponent<SelectableTray>().isUnselectable)
            {
                tray.Merge();
                count++;
            }
        }
        _ñoinStorage.MergeCoins(count);
    }

    public void DeleteAllCoins()
    {
        foreach (TrayScript tray in trayList)
        {
            tray.DestroyCoins();
        }
    }


    //public void Deal()
    //{
    //    _ñoinStorage.DealCoins();
    //    Dictionary<int, int> CoinsCountToSpawn = new Dictionary<int, int>();
    //    List<TrayScript> TraysToAddCoins = new List<TrayScript>();
    //    for (int i = 1; i <= NeedCoinsToMerge.Count; i++)
    //    {

    //        CoinsCountToSpawn.Add(i, Math.Clamp(UnityEngine.Random.Range(1, 6) - i, 1, NeedCoinsToMerge[i]));
    //        var randomIndex = UnityEngine.Random.Range(0, trayList.Count(tray => !tray.GetComponent<LockTrayScript>().isLocked));
    //        while (TraysToAddCoins.Any(tray => tray == trayList[randomIndex] && 10 - trayList[randomIndex].Coins.Count < NeedCoinsToMerge.Values.Min()))
    //        {
    //            randomIndex = UnityEngine.Random.Range(0, trayList.Count(tray => !tray.GetComponent<LockTrayScript>().isLocked));
    //        }
    //        TraysToAddCoins.Add(trayList[randomIndex]);
    //    }
    //    for (int i = 0; i < TraysToAddCoins.Count; i++)
    //    {
    //        int level = i + 1;
    //        TraysToAddCoins[i].AddCoins(CoinsCountToSpawn[level], level);
    //    }
    //}

    public bool CheckFullTrays()
    {

        if (trayList.Count(tray => !tray.GetComponent<LockTrayScript>().isLocked) * 10 - Coins.Count < 10)
            return true;
        else
            return false;
    }

    public void RestartGame()
    {
        if (CheckUnselecableTrays())
            return;
        DeleteAllCoins();
        _playerLevel.ResetLevel();
        for (int i = 0; i < 3; i++)
        {
            trayList[i].AddCoins(2, i + 1);
        }
        _ñoinStorage.ResetCoinStorage();
        playerRay.UnselectAll();

    }

    public void Shuffle()
    {
        int lastLevelCoinCount = Coins.Count(coin => coin.GetLevel() == _playerLevel.playerLevel);
        DeleteAllCoins();
        for (int i = 0; i < NeedCoinsToMerge.Count; i++)
        {
            if (!trayList[i].GetComponent<LockTrayScript>().isLocked)
            {
                if (i == NeedCoinsToMerge.Count - 1)
                {
                    trayList[i].AddCoins(lastLevelCoinCount, i + 1);
                }
                else
                {
                    trayList[i].AddCoins(UnityEngine.Random.Range(2, 6), i + 1);
                }
            }
        }

    }

    private bool CheckUnselecableTrays()
    {
        foreach (var tray in trayList)
        {
            if (tray.GetComponent<SelectableTray>().isUnselectable)
                return true;
        }
        return false;
    }

    public void Deal()
    {
        if (CheckUnselecableTrays())
            return;
        _ñoinStorage.DealCoins();

        Dictionary<int, int> CoinsCountToSpawn = new Dictionary<int, int>();
        List<TrayScript> TraysToAddCoins = new List<TrayScript>();
        for (int i = 1; i <= NeedCoinsToMerge.Count; i++)
        {

            CoinsCountToSpawn.Add(i, UnityEngine.Random.Range(5, 8));
            if (i >= NeedCoinsToMerge.Count - 2)
            {
                CoinsCountToSpawn[i]--;
            }
            if (i >= NeedCoinsToMerge.Count - 1)
            {
                CoinsCountToSpawn[i] -= 4;
            }
            if (i >= NeedCoinsToMerge.Count)
            {
                CoinsCountToSpawn[i] -= 3;
            }
            if (CoinsCountToSpawn[i] < 0) CoinsCountToSpawn[i] = 0;

            int randomIndex;
            var CheckedTrays = trayList.Count(tray => tray.Coins.Count == 10 || tray.GetComponent<LockTrayScript>().isLocked);
            if (trayList.Count - CheckedTrays >= NeedCoinsToMerge.Count)
            {
                randomIndex = UnityEngine.Random.Range(0, trayList.Count(tray => !tray.GetComponent<LockTrayScript>().isLocked));
                while (TraysToAddCoins.Any(tray => tray == trayList[randomIndex]) || trayList[randomIndex].Coins.Count == 10)
                {
                    randomIndex = UnityEngine.Random.Range(0, trayList.Count(tray => !tray.GetComponent<LockTrayScript>().isLocked));
                }
            }
            else
            {
                randomIndex = UnityEngine.Random.Range(0, trayList.Count(tray => !tray.GetComponent<LockTrayScript>().isLocked));
            }
            TraysToAddCoins.Add(trayList[randomIndex]);
        }
        List<GameObject> spawnedCoins = new List<GameObject>();
        for (int i = 0; i < TraysToAddCoins.Count; i++)
        {
            
            int level = i + 1;
            int j = 0;
            bool newCoinIsNull = false;
            while (j < CoinsCountToSpawn[level] && !newCoinIsNull)
            {
                var newCoin = TraysToAddCoins[i].AddCoins(1, level);
                if (newCoin == null)
                {
                    newCoinIsNull = true;
                    break;
                }

                TraysToAddCoins[i].GetComponent<SelectableTray>().isUnselectable = true;
                newCoin.SetActive(false);
                spawnedCoins.Add(Instantiate(CoinPrefab, CoinsDealSpawn.transform.position, Quaternion.Euler(-90f, 0f, 0f)));
                spawnedCoins.Last().GetComponent<CoinScript>().UpdateCoinLevel(level);
                StartCoroutine(TraysToAddCoins[i].MoveCoinToTray(spawnedCoins.Last().transform, newCoin.transform.position, newCoin, TraysToAddCoins[i], true));
                j++;
                
            }
            CoinsCountToSpawn[level] -= j;
            //print(CoinsCountToSpawn[level]);
        }

        TraysToAddCoins.Clear();
        for (int i = 1; i <= CoinsCountToSpawn.Count; i++)
        {
            if (CoinsCountToSpawn[i] <= 1) { continue; }
            int randomIndex;
            var CheckedTrays = trayList.Count(tray => tray.Coins.Count == 10 || tray.GetComponent<LockTrayScript>().isLocked);
            randomIndex = UnityEngine.Random.Range(0, trayList.Count(tray => !tray.GetComponent<LockTrayScript>().isLocked));
            TraysToAddCoins.Add(trayList[randomIndex]);
        }

        for (int i = 0; i < TraysToAddCoins.Count; i++)
        {
            int level = i + 1;
            int j = 0;
            bool newCoinIsNull = false;
            while (j < CoinsCountToSpawn[level] && !newCoinIsNull)
            {
                var newCoin = TraysToAddCoins[i].AddCoins(1, level);
                if (newCoin == null)
                {
                    newCoinIsNull = true;
                    break;
                }

                TraysToAddCoins[i].GetComponent<SelectableTray>().isUnselectable = true;
                newCoin.SetActive(false);
                spawnedCoins.Add(Instantiate(CoinPrefab, CoinsDealSpawn.transform.position, Quaternion.Euler(-90f, 0f, 0f)));
                spawnedCoins.Last().GetComponent<CoinScript>().UpdateCoinLevel(level);
                StartCoroutine(TraysToAddCoins[i].MoveCoinToTray(spawnedCoins.Last().transform, newCoin.transform.position, newCoin, TraysToAddCoins[i], true));
                j++;

            }
            CoinsCountToSpawn[level] -= j;
            //print(CoinsCountToSpawn[level]);
        }




        //List<GameObject> spawnedCoins = new List<GameObject>();
        //foreach (var level in CoinsCountToSpawn.Keys)
        //{
        //    for (int i = 0; i < CoinsCountToSpawn[level]; i++)
        //    {
        //        var newCoin = Instantiate(CoinPrefab, CoinsDealSpawn.transform.position, Quaternion.identity);
        //        newCoin.SetActive(false);
        //        spawnedCoins.Add(newCoin);
        //    }
        //}

        //StartCoroutine(MoveCoinsToTrays(spawnedCoins, TraysToAddCoins));







    }

    //public bool AddCoinOnPosition( int level)
    //{



    //    for (int i = 0; i < count; i++)
    //    {
    //        Vector3 newPos = CoinsObject.transform.position + new Vector3(-0.03799993f, 0.1491f, offset);

    //        var newCoin = Instantiate(CoinPrefab, CoinsObject.transform);
    //        newCoin.transform.localPosition = Vector3.zero;
    //        newCoin.transform.localPosition += new Vector3(-0.03799993f, 0.1491f, offset);
    //        newCoin.GetComponent<CoinScript>().UpdateCoinLevel(level);
    //        //newCoin.SetActive(false);
    //        offset -= 0.12f;
    //    }

    //    GetCoinsInTray();

    //    return true;
    //}


    public void CheckTraysForMerge()
    {
        foreach (TrayScript tray in trayList)
        {
            tray.IsReadyToMerge = CheckTrayForMerge(tray);
        }
    }

    private bool CheckTrayForMerge(TrayScript tray)
    {
        int count = tray.Coins.Count;
        if (count != 10)
            return false;

        CoinScript first = tray.Coins.First();
        foreach (CoinScript coin in tray.Coins)
        {
            if (first.GetLevel() != coin.GetLevel())
            {
                return false;
            }
        }

        return true;
    }

}
