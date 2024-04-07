using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI guardText;
    [SerializeField] private Guard guard;
    [SerializeField] private Vector3 offset;

    private void Awake()
    {
        EventManager.AddListener<string>(EventType.GuardText, UpdateGuardText);
    }

    private void Update()
    {
        Vector3 targetPos = guard.transform.position + offset;
        guardText.transform.position = targetPos;
    }

    private void UpdateGuardText(string _newText)
    {
        Debug.Log(_newText);
        guardText.text = _newText;
    }
}