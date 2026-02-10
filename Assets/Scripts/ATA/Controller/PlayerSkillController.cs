using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject lockIconSword; 
    public GameObject lockIconDash;  
    public GameObject lockIconRange;
    [Space] 
    public TMP_Text txtSwordLevel; 
    public TMP_Text txtDashLevel;
    public TMP_Text txtRangeLevel;
    [Header("Cooldown Fills")]
    public Image fillSword;
    public Image fillDash;
    public Image fillRange;
     
    
    private Dictionary<EnemyHealth.EnemyMutationType, int> skillLevels = new Dictionary<EnemyHealth.EnemyMutationType, int>();

    private const int MAX_SWORD_LEVEL = 4;
    private const int MAX_LEVEL = 3;
    
    private Dictionary<EnemyHealth.EnemyMutationType, float> lastUseTime
        = new Dictionary<EnemyHealth.EnemyMutationType, float>();

    private Dictionary<EnemyHealth.EnemyMutationType, float> cooldowns
        = new Dictionary<EnemyHealth.EnemyMutationType, float>()
        {
            { EnemyHealth.EnemyMutationType.Sword, 5f },
            { EnemyHealth.EnemyMutationType.Dash, 5f },
            { EnemyHealth.EnemyMutationType.Range, 5f }
        };
    
    private void Awake()
    {
        InitializeSkills();
    }

    private void Update()
    {
        UpdateCooldownUI(EnemyHealth.EnemyMutationType.Sword, fillSword);
        UpdateCooldownUI(EnemyHealth.EnemyMutationType.Dash, fillDash);
        UpdateCooldownUI(EnemyHealth.EnemyMutationType.Range, fillRange);
    }

    private void InitializeSkills()
    {
        skillLevels[EnemyHealth.EnemyMutationType.Sword] = 0;
        skillLevels[EnemyHealth.EnemyMutationType.Dash] = 0;
        skillLevels[EnemyHealth.EnemyMutationType.Range] = 0;
        
        lastUseTime[EnemyHealth.EnemyMutationType.Sword] = -999f;
        lastUseTime[EnemyHealth.EnemyMutationType.Dash] = -999f;
        lastUseTime[EnemyHealth.EnemyMutationType.Range] = -999f;

        if (lockIconSword) lockIconSword.SetActive(true);
        if (lockIconDash) lockIconDash.SetActive(true);
        if (lockIconRange) lockIconRange.SetActive(true);

        if (txtSwordLevel) txtSwordLevel.text = "";
        if (txtDashLevel) txtDashLevel.text = "";
        if (txtRangeLevel) txtRangeLevel.text = "";
    }
    
    public void AbsorbSkill(EnemyHealth.EnemyMutationType type)
    {
        if (!skillLevels.ContainsKey(type)) skillLevels[type] = 0;

        int maxLevel = type == EnemyHealth.EnemyMutationType.Sword
            ? MAX_SWORD_LEVEL
            : MAX_LEVEL;

        if (skillLevels[type] < maxLevel)
        {
            skillLevels[type]++;
            UpdateSkillUI(type);
        }
    }
    
    private void UpdateSkillUI(EnemyHealth.EnemyMutationType type)
    {
        int currentLevel = skillLevels[type];

        switch (type)
        {
            case EnemyHealth.EnemyMutationType.Sword:
                if (currentLevel > 0)
                {
                    if (lockIconSword != null)
                    {
                        lockIconSword.SetActive(false);
                    }
                    
                }
                UpdateText(txtSwordLevel, currentLevel);
                break;

            case EnemyHealth.EnemyMutationType.Dash:
                if (currentLevel > 0)
                {
                    if (lockIconDash != null)
                    {
                        lockIconDash.SetActive(false);
                       
                    }
                   
                }
                UpdateText(txtDashLevel, currentLevel);
                break;

            case EnemyHealth.EnemyMutationType.Range:
                if (currentLevel > 0)
                {
                    if (lockIconRange != null)
                    {
                        lockIconRange.SetActive(false);
                        
                    }
                   
                }
                UpdateText(txtRangeLevel, currentLevel);
                break;
        }
    }
    
    private void UpdateText(TMP_Text textComponent, int level)
    {
        if (textComponent != null)
        {
            int maxLevel = MAX_LEVEL;

            if (textComponent == txtSwordLevel)
                maxLevel = MAX_SWORD_LEVEL;

            if (level >= maxLevel)
            {
                textComponent.text = "M";
            }
            else
            {
                textComponent.text = level.ToString();
            }
        }
    }
    
    private void UpdateCooldownUI(
        EnemyHealth.EnemyMutationType type,
        Image fillImage
    )
    {
        if (fillImage == null) return;

        if (GetSkillLevel(type) <= 0)
        {
            fillImage.fillAmount = 0f;
            return;
        }

        float elapsed = Time.time - lastUseTime[type];
        float cd = cooldowns[type];

        fillImage.fillAmount = 1f - Mathf.Clamp01(elapsed / cd);
    }
    public void NotifySkillUsed(EnemyHealth.EnemyMutationType type)
    {
        if (!lastUseTime.ContainsKey(type)) return;
        lastUseTime[type] = Time.time;
    }
    
    public int GetSkillLevel(EnemyHealth.EnemyMutationType type)
    {
        return skillLevels.ContainsKey(type) ? skillLevels[type] : 0;
    }
}