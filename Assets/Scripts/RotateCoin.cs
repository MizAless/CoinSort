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
        // Получаем текущее положение объекта
        Vector3 currentRotation = transform.rotation.eulerAngles;

        // Вычисляем новое положение с учетом вращения
        float newRotationX = currentRotation.x;
        //float newRotationX = currentRotation.x + rotationSpeed * Time.deltaTime;
        float newRotationY = currentRotation.y;
        //float newRotationY = currentRotation.y + rotationSpeed * Time.deltaTime;
        float newRotationZ = currentRotation.z + rotationSpeed * Time.deltaTime;

        // Применяем новое положение к объекту
        transform.rotation = Quaternion.Euler(newRotationX, newRotationY, newRotationZ);
    }
}
