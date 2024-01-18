using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class СoinStorage : MonoBehaviour
{
    [SerializeField] public TMP_Text coinsCountText;
    public int coinsCount { get; private set; }
    public int startCoinsCount = 100;
    public int MergeCoinPrize = 15;
    public int MergeBonusCoinPrize = 20;
    public int DealCoinPrice = 10;

    private void UpdateCoinsCount()
    {
        coinsCountText.text = coinsCount.ToString();
    }

    public void MergeCoins(int multiplayer)
    {
        if (multiplayer != 0)
            coinsCount += MergeCoinPrize * multiplayer + MergeBonusCoinPrize * (multiplayer - 1);
    }

    public void DealCoins()
    {
        coinsCount -= DealCoinPrice;
    }

    public void BuyUnlockTray(int cost)
    {
        coinsCount -= cost;
    }

    public void ResetCoinStorage()
    {
        if (coinsCount < startCoinsCount)
        {
            coinsCount = startCoinsCount;
        }
    }

    void Start()
    {
        coinsCount = startCoinsCount;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCoinsCount();
    }
}
