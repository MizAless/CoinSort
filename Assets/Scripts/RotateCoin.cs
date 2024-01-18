using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCoin : MonoBehaviour
{
    public float rotationSpeed = 150f;
    public Transform PCposition;

    private void Start()
    {
        if (Screen.currentResolution.width / Screen.currentResolution.height > 1.0f)
        {
            transform.position = PCposition.position;
        }
    }

    void Update()
    {
        if ((float)Screen.currentResolution.width / Screen.currentResolution.height > 1.0f)
        {
            transform.position = PCposition.position;
        }
        // �������� ������� ��������� �������
        Vector3 currentRotation = transform.rotation.eulerAngles;

        // ��������� ����� ��������� � ������ ��������
        float newRotationX = currentRotation.x;
        //float newRotationX = currentRotation.x + rotationSpeed * Time.deltaTime;
        float newRotationY = currentRotation.y;
        //float newRotationY = currentRotation.y + rotationSpeed * Time.deltaTime;
        float newRotationZ = currentRotation.z + rotationSpeed * Time.deltaTime;

        // ��������� ����� ��������� � �������
        transform.rotation = Quaternion.Euler(newRotationX, newRotationY, newRotationZ);
    }
}
