using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Assets/Skill")]
public class Skill : ScriptableObject
{
    public int skillNo;
    public string skillName;
    public string skillExplanation;
    public GameObject powerUpIcon;
    public int sp;
}
