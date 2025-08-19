using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour 
{
    public CardSO cardSO;

    public TextMeshProUGUI title;
    public TextMeshProUGUI description;



    private void Awake()
    {
        
    }
    private void OnEnable()
    {
        //不太能放在这里
        //title.text = cardSO.cardName;
        //description.text = cardSO.cardDescription;
    }
    private void Start()
    {
        title.text = cardSO.cardName;
        description.text = cardSO.cardDescription;
    }
}
