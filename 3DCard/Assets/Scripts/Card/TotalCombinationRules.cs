using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TotalCombinationRule", menuName = "Card/TotalCombinationRule", order = 1)]
public class TotalCombinationRule : ScriptableObject
{
    public List<CardCombinationRule> totalCombinationRuleList = new();
    public Dictionary<int,CardCombinationRule> totalCombinationRuleDic = new Dictionary<int,CardCombinationRule>();
}
