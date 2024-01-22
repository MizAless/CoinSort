using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CoinsCloudMove : MonoBehaviour
{
    private GameObject target;
    public IEnumerator MoveCloud(Transform coinTransform, Vector3 targetTransform)
    {
        float duration = 1f; // Длительность анимации
        Vector3 startPosition = coinTransform.position;
        Vector3 targetPosition = targetTransform;

        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            float height = 1.25f - 5f * (t - 0.5f) * (t - 0.5f);
            coinTransform.position = Vector3.Lerp(startPosition, targetPosition, t) + Vector3.up * height;

            yield return null;
        }

        Destroy(coinTransform.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("GameManager").GetComponent<GameManager>().MoneyTarget;
        StartCoroutine(MoveCloud(transform, target.transform.position));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
