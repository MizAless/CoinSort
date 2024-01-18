using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LockTrayScript : MonoBehaviour
{
    [SerializeField] public SpriteRenderer LockImage;
    [SerializeField] public bool isLocked = true;
    [SerializeField] public TMP_Text unlockCostText;
    [SerializeField] public GameObject UnclockButton;
    public int unlockCost { get; private set; }

    public void Unlock()
    {
        isLocked = false;
        LockImage.gameObject.SetActive(false);
        UnclockButton.SetActive(false);
    }

    private void Lock()
    {
        isLocked = true;
        LockImage.gameObject.SetActive(true);
        UnclockButton.SetActive(true);
    }

    public void SetUnlockCost(int cost)
    {
        unlockCost = cost;
        unlockCostText.text = cost.ToString();
    }

    void Start()
    {
        if (!isLocked)
        {
            Unlock();
        }
        else
        {
            Lock();
        }
    }

    void Update()
    {
        if (!isLocked)
        {
            Unlock();
        }
        else
        {
            Lock();
        }
    }
}
