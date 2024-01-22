using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyTarget : MonoBehaviour
{
    [SerializeField] private GameObject rotateCoin;
    void Update()
    {
        transform.position = rotateCoin.transform.position;
    }
}
