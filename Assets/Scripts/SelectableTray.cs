using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectableTray : MonoBehaviour
{
    [SerializeField] private GameObject CoinsObject;
    [SerializeField] private float yOffset = 0.1f;
    [SerializeField] private GameObject selectedLight;
    public List<Transform> Coins = new List<Transform>();

    public bool isSelected { get; private set; } = false;
    [SerializeField]  public bool isUnselectable = false;

    private TrayScript _trayScript;


    void Start()
    {
        GetCoinsInTray();
        _trayScript = GetComponent<TrayScript>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            CoinsDown();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            CoinsUp();
        }
    }


    public void GetCoinsInTray()
    {
        Coins.Clear();
        foreach (Transform obj in CoinsObject.transform)
        { 
            var coin = obj.GetComponent<CoinScript>();
            if (coin != null)
            {
                Coins.Add(obj);
            }
        }
    }

    public void CoinsUp()
    {
        List<int> coinIndexes = _trayScript.GetSameLevelCoinIndex();
        if (coinIndexes.Count == 0)
            return;

        foreach (var i in coinIndexes)
        {
            Coins[i].position = Coins[i].position + Vector3.up * yOffset;
        }
    }

    public void CoinsDown()
    {
        List<int> coinIndexes = _trayScript.GetSameLevelCoinIndex();
        if (coinIndexes.Count == 0)
            return;

        foreach (var i in coinIndexes)
        {
            Coins[i].position = Coins[i].position - Vector3.up * yOffset;
        }
    }

    public bool CanRelocate(SelectableTray otherTray)
    {
        if (_trayScript.Coins.Count == 0) return false;

        if (otherTray._trayScript.Coins.Count == 0 || otherTray == this) return true;
        

        bool res = _trayScript.Coins.Last().GetLevel() == otherTray._trayScript.Coins.Last().GetLevel();

        return res; 
    }

    public void ToggleSelection()
    {
        GetCoinsInTray();
        if (isSelected)
        {
            CoinsDown();
            selectedLight.SetActive(false);
        }
        else
        {
            CoinsUp();
            selectedLight.SetActive(true);
        }

        isSelected = !isSelected;
        //print(isSelected);
    }

    public void Unselect()
    {
        isSelected = false;
    }

    public bool RelocateCoinsInTray(SelectableTray otherTray)
    {
        var tray = otherTray.GetComponent<TrayScript>();
        var res = GetComponent<TrayScript>().RelocateCoinsInTray(tray);
        GetCoinsInTray();
        otherTray.GetCoinsInTray();
        return res;
    }


}
