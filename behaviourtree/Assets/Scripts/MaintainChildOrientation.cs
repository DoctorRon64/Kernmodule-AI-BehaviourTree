using UnityEngine;

public class MaintainChildOrientation : MonoBehaviour
{
    public Transform parentTransform;
    public Transform particleSystemTransform;
    private Vector3 initialLocalPosition;

    void Start()
    {
        initialLocalPosition = particleSystemTransform.localPosition;
    }

    void Update()
    {
        particleSystemTransform.position = parentTransform.position + initialLocalPosition;
    }
}