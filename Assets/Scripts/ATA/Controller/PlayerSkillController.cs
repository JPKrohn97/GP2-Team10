using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
     
    
    private Dictionary<EnemyHealth.EnemyMutationType, int> skillLevels = new Dictionary<EnemyHealth.EnemyMutationType, int>();
    
    private const int MAX_LEVEL = 3;
    
    private void Awake()
    {
        InitializeSkills();
    }
    
    private void InitializeSkills()
    {
        skillLevels[EnemyHealth.EnemyMutationType.Sword] = 0;
        skillLevels[EnemyHealth.EnemyMutationType.Dash] = 0;
        skillLevels[EnemyHealth.EnemyMutationType.Range] = 0;

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

        if (skillLevels[type] < MAX_LEVEL)
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
            if (level >= MAX_LEVEL)
            {
                textComponent.text = "M";
            }
            else
            {
                textComponent.text = level.ToString(); 
            }
        }
    }
    
    public int GetSkillLevel(EnemyHealth.EnemyMutationType type)
    {
        return skillLevels.ContainsKey(type) ? skillLevels[type] : 0;
    }
}