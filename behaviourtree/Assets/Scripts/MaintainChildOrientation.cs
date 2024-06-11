using UnityEngine;

public class MaintainChildOrientation : MonoBehaviour
{
    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        transform.localRotation = initialRotation;
    }
}