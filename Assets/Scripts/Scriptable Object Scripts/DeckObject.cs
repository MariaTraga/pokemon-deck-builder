using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/Deck")]
public class DeckObject : ScriptableObject
{
    [SerializeField] private List<CardData> cards;
    [SerializeField] public int size { get; private set; } = 20;



    public void AddCard(CardData card)
    {
        cards.Add(card);
    }


    public void RemoveCard(CardData card)
    {
        cards.Remove(card);
    }


    public List<CardData> GetCards()
    {
        return cards;
    }
}

[Serializable]
public struct DeckCard
{
    public CardData cardData;
    public int index;
    public bool isEmpty => cardData == null;

    public static DeckCard GetEmptyCard() =>
        new DeckCard
        {
            cardData = null
        };
}
