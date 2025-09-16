using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardCombinationRule", menuName = "Card/Create New Card Combination Rule", order = 1)]
public class CardCombinationRule :ScriptableObject
{
    public List<CardSO> requiredCards;
    public CardSO resultCard;
    public int id;
    public int priority;//越大优先级越高
}
