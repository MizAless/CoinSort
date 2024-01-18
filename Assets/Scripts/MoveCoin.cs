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
            // ��������� �������� �������� �� 0 �� 1
            float progress = _elapsedTime / totalTime;

            // ���������� ������������ ������� ��� ��������� ������ � ����������� �� �������
            float height = 0.25f * (progress - 0.5f) * (progress - 0.5f); // ����������� ���������

            // ��������� ������� ������� �� �������������� ����������
            transform.position = Vector3.Lerp(transform.position, _targetGameObject.position, progress) + Vector3.up * height;

            // ����������� ��������� �����
            _elapsedTime += Time.deltaTime;
        }
        else
        {
            // ��������� ��������
            transform.position = _targetGameObject.position;
        }
    }
}
