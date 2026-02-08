using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject btnSkillSword; 
    public GameObject btnSkillDash;  
    public GameObject btnSkillRange;
    
    [Header("Level Colors")]
    public Color level1Color = Color.white;   // Başlangıç rengi
    public Color level2Color = Color.yellow;  // Orta seviye
    public Color level3Color = new Color(1f, 0.2f, 0.2f);
    
    
    // private Dictionary<EnemyHealth.EnemyMutationType, int> skillLevels = new Dictionary<EnemyHealth.EnemyMutationType, int>();
    //
    // private const int MAX_LEVEL = 3;
    //
    // private void Awake()
    // {
    //     InitializeSkills();
    // }
    //
    // private void InitializeSkills()
    // {
    //     skillLevels[EnemyHealth.EnemyMutationType.Sword] = 0;
    //     skillLevels[EnemyHealth.EnemyMutationType.Dash] = 0;
    //     skillLevels[EnemyHealth.EnemyMutationType.Range] = 0;
    //
    //
    //     if (btnSkillSword) btnSkillSword.SetActive(false);
    //     if (btnSkillDash) btnSkillDash.SetActive(false);
    //     if (btnSkillRange) btnSkillRange.SetActive(false);
    // }
    //
    // public void AbsorbSkill(EnemyHealth.EnemyMutationType type)
    // {
    //     if (skillLevels.ContainsKey(type) && skillLevels[type] < MAX_LEVEL)
    //     {
    //         skillLevels[type]++;
    //         UnlockSkillButton(type);
    //     }
    // }
    //
    // private void UnlockSkillButton(EnemyHealth.EnemyMutationType type)
    // {
    //     switch (type)
    //     {
    //         case EnemyHealth.EnemyMutationType.Sword:
    //             if (btnSkillSword) btnSkillSword.SetActive(true);
    //             UpdateButtonColor(btnSkillSword, skillLevels[type]);
    //             break;
    //         case EnemyHealth.EnemyMutationType.Dash:
    //             if (btnSkillDash) btnSkillDash.SetActive(true);
    //             UpdateButtonColor(btnSkillSword, skillLevels[type]);
    //             break;
    //         case EnemyHealth.EnemyMutationType.Range:
    //             if (btnSkillRange) btnSkillRange.SetActive(true);
    //             UpdateButtonColor(btnSkillSword, skillLevels[type]);
    //             break;
    //     }
    // }
    //
    // private void UpdateButtonColor(GameObject button, int level)
    // {
    //     var img = button.GetComponent<UnityEngine.UI.Image>();
    //     if (!img) return;
    //
    //     img.color = level switch
    //     {
    //         1 => level1Color,
    //         2 => level2Color,
    //         3 => level3Color,
    //         _ => Color.white
    //     };
    // }
    //
    // public int GetSkillLevel(EnemyHealth.EnemyMutationType type)
    // {
    //     return skillLevels.ContainsKey(type) ? skillLevels[type] : 0;
    // }
}