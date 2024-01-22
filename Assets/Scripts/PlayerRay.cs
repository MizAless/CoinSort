using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerRay : MonoBehaviour
{
    public GameObject GameManager;

    private Camera mainCamera;
    private RaycastHit hitInfo;
    private ÑoinStorage _ñoinStorage;
    [SerializeField] private SelectableTray selectableTray;
    [SerializeField] private LayerMask trayLayerMask;
    [SerializeField] private LayerMask unlockTrayButtonLayerMask;


    private void CastTrayRay(Ray ray)
    {
        RaycastHit[] hits = Physics.RaycastAll(ray, 100, trayLayerMask);
        if (!hits.Any(hit => hit.collider.GetComponent<SelectableTray>()))
            return;

        SelectableTray tray = hits.FirstOrDefault(hit => hit.collider.GetComponent<SelectableTray>()).collider.GetComponent<SelectableTray>();

        if (tray == null)
            return;

        if (tray.isUnselectable)
            return;

        if (tray.GetComponent<LockTrayScript>().isLocked)
            return;

        if (selectableTray != null && !selectableTray.CanRelocate(tray))
        {
            UnselectAll();
            return;
        }


        if (selectableTray != null && tray != selectableTray)
        {
            selectableTray.ToggleSelection();
            selectableTray.RelocateCoinsInTray(tray);
            selectableTray = null;
        }
        else
        {
            tray.ToggleSelection();
            if (tray.isSelected) selectableTray = tray;
            else selectableTray = null;
        }
    }

    private void CastUnlockTrayButtonRay(Ray ray)
    {
        RaycastHit[] hits = Physics.RaycastAll(ray, 100, unlockTrayButtonLayerMask);
        if (!hits.Any(hit => hit.collider.GetComponentInParent<LockTrayScript>()))
            return;

        LockTrayScript lockTray = hits.FirstOrDefault(hit => hit.collider.GetComponentInParent<LockTrayScript>()).collider.GetComponentInParent<LockTrayScript>();

        if (lockTray == null)
            return;

        if (_ñoinStorage.coinsCount >= lockTray.unlockCost)
        {
            lockTray.Unlock();
            _ñoinStorage.BuyUnlockTray(lockTray.unlockCost);
        }
    }

    public void UnselectAll()
    {
        if (selectableTray != null)
        {
            selectableTray.ToggleSelection();
            selectableTray = null;
        }
    }

    void Start()
    {
        mainCamera = Camera.main;
        selectableTray = null;
        _ñoinStorage = GameManager.GetComponent<ÑoinStorage>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray ray;

            if (Input.touchCount > 0)
            {
                ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
            }
            else
            {
                ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            }

            Debug.DrawRay(transform.position, ray.direction * 100, Color.yellow);

            CastTrayRay(ray);
            CastUnlockTrayButtonRay(ray);
        }
    }



}
