using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface ITableCardCombination
{
    List<CardCombinationRule> CombinationRuleList { get; set; }
}

public class TableCardCombinationMgr : SingletonAutoMono<TableCardCombinationMgr>, ITableCardCombination
{
    public TotalCombinationRule TotalCombinationRule;
    public List<CardCombinationRule> CombinationRuleList => ((ITableCardCombination)this).CombinationRuleList;
    List<CardCombinationRule> ITableCardCombination.CombinationRuleList {get;set;}


    private void Awake()
    {
        TotalCombinationRule = AddressablesMgr.Instance.LoadAsset<TotalCombinationRule>("TotalCombinationRule");
        ((ITableCardCombination)this).CombinationRuleList = new List<CardCombinationRule>();
        foreach (var CR in TotalCombinationRule.totalCombinationRuleList)
        {
            AddCombination(CR);
        }
    }

    public void AddCombination(CardCombinationRule cardCR)
    {
        if (CombinationRuleList.Contains(cardCR))
        {
            return;
        }
        CombinationRuleList.Add(cardCR);
        ((ITableCardCombination)this).CombinationRuleList = CombinationRuleList.OrderByDescending(x => x.priority).ToList();
    }
    public void AddCombination(int id)
    {
        CardCombinationRule cardCR = TotalCombinationRule.totalCombinationRuleDic[id];
        AddCombination(cardCR);
    }
    public void RemoveCombination(CardCombinationRule cardCR)
    {
        if (!CombinationRuleList.Contains(cardCR))
        {
            return;
        }
        CombinationRuleList.Remove(cardCR);
    }
    public void RemoveCombination(int id)
    {
        CardCombinationRule cardCR = TotalCombinationRule.totalCombinationRuleDic[id];
        RemoveCombination(cardCR);
    }
}