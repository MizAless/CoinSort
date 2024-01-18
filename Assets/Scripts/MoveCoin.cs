using UnityEngine;

public class MoveCoin : MonoBehaviour
{
    [SerializeField]
    private Transform _targetGameObject;

    public float totalTime = 1f;
    private float _elapsedTime = 0f;

    private void Update()
    {
        if (_elapsedTime < totalTime && Vector3.Distance(transform.position, _targetGameObject.position) > 0.01f)
        {
            // Вычисляем прогресс движения от 0 до 1
            float progress = _elapsedTime / totalTime;

            // Используем квадратичную функцию для изменения высоты в зависимости от времени
            float height = 0.25f * (progress - 0.5f) * (progress - 0.5f); // Уменьшенный множитель

            // Обновляем позицию объекта по параболической траектории
            transform.position = Vector3.Lerp(transform.position, _targetGameObject.position, progress) + Vector3.up * height;

            // Увеличиваем прошедшее время
            _elapsedTime += Time.deltaTime;
        }
        else
        {
            // Завершаем движение
            transform.position = _targetGameObject.position;
        }
    }
}
