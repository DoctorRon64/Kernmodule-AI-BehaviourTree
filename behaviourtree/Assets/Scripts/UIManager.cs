using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class AgentText
{
    public TextMeshProUGUI AgentTMPro;
    public Transform Agent;
    public Vector3 Offset;
}

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<AgentText> agentTexts = new List<AgentText>();

    private void Awake()
    {
        EventManager.AddListener<string>(EventType.GuardText, UpdateGuardText);
        EventManager.AddListener<string>(EventType.NinjaText, UpdateNinjaText);
    }

    private void Update()
    {
        foreach (AgentText agentText in agentTexts)
        {
            Vector3 targetPos = agentText.Agent.position + agentText.Offset;
            agentText.AgentTMPro.transform.position = targetPos;
        }
    }

    private void UpdateNinjaText(string _newText)
    {
        agentTexts[1].AgentTMPro.text = _newText;
    }
    
    private void UpdateGuardText(string _newText)
    {
        agentTexts[0].AgentTMPro.text = _newText;
    }
}