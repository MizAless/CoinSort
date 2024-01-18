using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 15);
    }

    void Update()
    {
        transform.position += -Vector3.forward * Time.deltaTime;
    }
}
