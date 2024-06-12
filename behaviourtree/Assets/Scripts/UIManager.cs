using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class AgentText
{
    public TextMeshProUGUI AgentTMPro;
    public Transform Agent;
    public Vector3 Offset;
}

[Serializable]
public class PlayerSlider
{
    public Slider slider;
    public Transform player;
    public Vector3 Offset;
}

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<AgentText> agentTexts = new List<AgentText>();
    [SerializeField] private PlayerSlider hpSlider;
    
    private void Awake()
    {
        EventManager.AddListener<string>(EventType.GuardText, UpdateGuardText);
        EventManager.AddListener<string>(EventType.NinjaText, UpdateNinjaText);
        EventManager.AddListener<int>(EventType.OnPlayerHpChanged, UpdateHpSlider);
        EventManager.AddListener<bool>(EventType.OnPlayerDied, OnPlayerDied);
        hpSlider.slider.value = hpSlider.slider.maxValue;
    }

    private void Update()
    {
        foreach (AgentText agentText in agentTexts)
        {
            Vector3 targetPos = agentText.Agent.position + agentText.Offset;
            agentText.AgentTMPro.transform.position = targetPos;
        }

        if (hpSlider.player != null)
        {
            Vector3 targetPos2 = hpSlider.player.position + hpSlider.Offset;
            hpSlider.slider.transform.position = targetPos2;
        }

    }
    
    private void OnPlayerDied(bool isDead)
    {
        if (isDead)
        {
            hpSlider.player = null;
        }
    }


    private void UpdateHpSlider(int _newHp)
    {
        hpSlider.slider.value = _newHp;
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