using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TrayScript : MonoBehaviour
{
    [SerializeField] private GameObject CoinsObject;
    [SerializeField] private GameObject CoinPrefab;
    [SerializeField] private GameObject MergeLight;
    [SerializeField] private ParticleSystem Smoke;
   

    private static int MaxCoinCount = 10;

    public List<CoinScript> Coins = new List<CoinScript>();

    public bool IsReadyToMerge = false;

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

    public GameObject AddCoins(int count, int level)
    {
        if (Coins.Count + count > MaxCoinCount)
            return null;

        GameObject newCoin = null;
        float offset = 0.56f - Coins.Count * 0.12f;

        for (int i = 0; i < count; i++)
        {
            Vector3 newPos = CoinsObject.transform.position + new Vector3(-0.03799993f, 0.1491f, offset);

            newCoin = Instantiate(CoinPrefab, CoinsObject.transform);
            newCoin.transform.localPosition = Vector3.zero;
            newCoin.transform.localPosition += new Vector3(-0.03799993f, 0.1491f, offset);
            newCoin.GetComponent<CoinScript>().UpdateCoinLevel(level);
            //newCoin.SetActive(false);
            offset -= 0.12f;
        }

        GetCoinsInTray();

        return newCoin;
    }

    public bool AddCoins(int count, int level, Transform prevCoinTransform, TrayScript otherTray, bool isLast)
    {
        if (Coins.Count + count > MaxCoinCount)
            return false;

        float offset = 0.56f - Coins.Count * 0.12f;

        for (int i = 0; i < count; i++)
        {
            Vector3 newPos = CoinsObject.transform.position + new Vector3(-0.03799993f, 0.1491f, offset);

            var newCoin = Instantiate(CoinPrefab, CoinsObject.transform);
            GetCoinsInTray();
            var cor = StartCoroutine(MoveCoinToTray(prevCoinTransform, newPos, newCoin, otherTray, isLast));
            newCoin.transform.localPosition = Vector3.zero;
            newCoin.transform.localPosition += new Vector3(-0.03799993f, 0.1491f, offset);
            newCoin.GetComponent<CoinScript>().UpdateCoinLevel(level);
            newCoin.SetActive(false);
            offset -= 0.12f;
        }

        GetCoinsInTray();

        return true;
    }


    public IEnumerator MoveCoinToTray(Transform coinTransform, Vector3 targetTransform, GameObject newCoin, TrayScript otherTray, bool isLast)
    {
        float duration = 0.4f; // Длительность анимации
        Vector3 startPosition = coinTransform.position;
        Vector3 targetPosition = targetTransform;

        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            float height = 1.25f - 5f * (t - 0.5f) * (t - 0.5f);
            coinTransform.position = Vector3.Lerp(startPosition, targetPosition, t) + Vector3.up * height;

            float rotationAngle = 360f * t;
            coinTransform.rotation = Quaternion.Euler(-90f, rotationAngle, 0f);

            yield return null;
        }

        coinTransform.position = targetPosition;
        coinTransform.rotation = Quaternion.identity;
        if (isLast)
        {
            var tray = coinTransform.GetComponentInParent<SelectableTray>();
            if (tray != null)
            {
                tray.isUnselectable = false;
            }
            otherTray.GetComponent<SelectableTray>().isUnselectable = false;
        }
        Destroy(coinTransform.gameObject);
        newCoin.SetActive(true);
        otherTray.GetCoinsInTray();
        GetCoinsInTray();
    }

    public IEnumerator MoveCoinToTray(Transform coinTransform, Vector3 targetTransform)
    {
        float duration = 0.2f; // Длительность анимации
        Vector3 startPosition = coinTransform.position;
        Vector3 targetPosition = targetTransform;

        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            coinTransform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        coinTransform.position = targetPosition;
        coinTransform.rotation = Quaternion.identity;
        coinTransform.GetComponentInParent<SelectableTray>().isUnselectable = false;
        Destroy(coinTransform.gameObject);
    }


    public bool RelocateCoinsInTray(TrayScript otherTray)
    {
        if (otherTray.Coins.Count == MaxCoinCount) 
            return false; 
        GetComponent<SelectableTray>().isUnselectable = true;
        otherTray.GetComponent<SelectableTray>().isUnselectable = true;
        List<int> coinIndexes = GetSameLevelCoinIndex();
        coinIndexes.Reverse();
        List<int> coinIndexesForDestroy = new List<int>();

        if (coinIndexes.Count == 0)
            return false;

        int count = coinIndexes.Count - (10 - otherTray.Coins.Count);
        for (int i = 0; i < count; i++)
        {
            coinIndexes.RemoveAt(0);
        }


        StartCoroutine(MoveCoinsWithDelay(coinIndexes, otherTray));

        //foreach (var i in coinIndexes)
        //{
        //    //if (otherTray.AddCoins(1, Coins[i].GetLevel(), Coins[i].transform))
        //    otherTray.AddCoins(1, Coins[i].GetLevel(), Coins[i].transform);
        //    //{
        //    //    coinIndexesForDestroy.Add(i);
        //    //}
        //}
        //foreach (var i in coinIndexesForDestroy)
        //{
        //    Destroy(Coins[i].gameObject);
        //}
        GetCoinsInTray();

        return true;
    }

    public IEnumerator MoveCoinsWithDelay(List<int> coinIndexes, TrayScript otherTray)
    {
        float delayBetweenMoves = 0.02f;

        foreach (var i in coinIndexes)
        {
            yield return new WaitForSeconds(delayBetweenMoves);
            if (i == coinIndexes.Last())
            {
                otherTray.AddCoins(1, Coins[i].GetLevel(), Coins[i].transform, otherTray, true);
            }
            else
            {
                otherTray.AddCoins(1, Coins[i].GetLevel(), Coins[i].transform, otherTray, false);
            }
        }
        GetCoinsInTray();
    }

    private void CheckForMerge()
    {
        MergeLight.SetActive(IsReadyToMerge);
    }

    //public void Merge()
    //{
    //    GetComponent<SelectableTray>().isUnselectable = true;
    //    var trans = Coins.First().transform;
    //    int nextCoinLevel = Coins.First().GetLevel() + 1;
    //    for (int i = 0; i < Coins.Count; i++)
    //    {
    //        StartCoroutine(MoveCoinToTray(Coins[i].transform, Coins.First().transform.position));
    //    }
    //    //DestroyCoins();
    //    //GetCoinsInTray();
    //    Coins.Clear();
    //    AddCoins(2, nextCoinLevel);
    //}

    public void Merge()
    {
        Smoke.Play();
        GetComponent<SelectableTray>().isUnselectable = true;
        int nextCoinLevel = Coins.First().GetLevel() + 1;

        // Создаем список монет, который мы будем перемещать
        List<Transform> coinTransforms = new List<Transform>();
        foreach (var coin in Coins)
        {
            coinTransforms.Add(coin.transform);
        }

        // Запускаем корутину перемещения монет с передачей Callback-функции
        StartCoroutine(MoveCoinsToTray(coinTransforms, Coins.First().transform.position, () =>
        {
            Coins.Clear();
            AddCoins(2, nextCoinLevel);
        }));
    }

    public IEnumerator MoveCoinsToTray(List<Transform> coinTransforms, Vector3 targetTransform, Action onComplete)
    {
        float duration = 0.2f; // Длительность анимации

        // Запоминаем начальные позиции монет
        Vector3[] startPositions = new Vector3[coinTransforms.Count];
        for (int i = 0; i < coinTransforms.Count; i++)
        {
            startPositions[i] = coinTransforms[i].position;
        }

        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            for (int i = 0; i < coinTransforms.Count; i++)
            {
                coinTransforms[i].position = Vector3.Lerp(startPositions[i], targetTransform, t);
            }

            yield return null;
        }

        // Устанавливаем конечные позиции монет
        for (int i = 0; i < coinTransforms.Count; i++)
        {
            coinTransforms[i].position = targetTransform;
            coinTransforms[i].rotation = Quaternion.identity;
            coinTransforms[i].GetComponentInParent<SelectableTray>().isUnselectable = false;
            Destroy(coinTransforms[i].gameObject);
        }

        // Вызываем Callback-функцию по завершению перемещения монет
        onComplete?.Invoke();
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
