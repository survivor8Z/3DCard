using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_CardType
{
    E_Entity,
    E_Modificatory,
    E_Behavior,
    E_Condition,
}

[CreateAssetMenu(fileName = "Card", menuName = "Card/Create New Card", order = 1)]
public class CardSO : ScriptableObject
{
    public string cardName;
    public int cardID;
    public E_CardType cardType;
    public string cardDescription;

    public CardSO cardSO;
}
