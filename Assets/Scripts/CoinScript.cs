using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    [SerializeField] private int coinLevel = 1;
    [SerializeField] private List<Material> levelMaterials;
    [SerializeField] private TMP_Text text;

    public CoinScript(int CoinLevel) {
        UpdateCoinLevel(CoinLevel);
    }

    void Start()
    {
        UpdateCoinLevel(coinLevel);
    }

    void Update()
    {
        UpdateCoinLevel(coinLevel);
    }

    public int GetLevel()
    {
        return coinLevel;
    }

    public void UpdateCoinLevel(int newLevel)
    {
        coinLevel = newLevel;
        text.text = Convert.ToString(coinLevel);
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = levelMaterials[(newLevel - 1) % levelMaterials.Count];
        }
    }
}
