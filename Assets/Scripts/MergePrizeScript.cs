using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergePrizeScript : MonoBehaviour
{
    private float time;
    public float duration = 1f;
    public float speed = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (time + duration < Time.time)
        {
            Destroy(gameObject);
        }
        transform.position += Vector3.up * speed * Time.deltaTime;
    }
}
