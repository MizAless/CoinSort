using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    private GameManager _gameManager;
    public int playerLevel { get; private set; }

    private void Awake()
    {
        _gameManager = GetComponent<GameManager>();
        playerLevel = 0;
        GetMaxCoinLevel();
    }
    //private void Update()
    //{
    //    _gameManager = GetComponent<GameManager>();
    //    playerLevel = 0;
    //    GetMaxCoinLevel();
    //}

    public void GetMaxCoinLevel()
    {
        foreach (TrayScript tray in _gameManager.trayList)
        {
            foreach (CoinScript coin in tray.Coins)
            {
                int coinLevel = coin.GetLevel();
                if (coinLevel > playerLevel)
                {
                    playerLevel = coinLevel;
                }
            }
        }
    }

    public void ResetLevel()
    {
        playerLevel = 0;
    }

    private void Update()
    {
        GetMaxCoinLevel();
    }

}
