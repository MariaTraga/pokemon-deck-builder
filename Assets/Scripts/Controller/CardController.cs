using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum SortBy
{
    HP = 0,
    RARITY = 1,
    TYPE = 2
}

public class CardController : MonoBehaviour
{
    /// 
    /// Singleton
    /// 
    public static CardController Instance;
    private void Awake()
    {
        if (Instance != null)
            Instance = this;
    }
    ///
    ///
    ///

    [SerializeField] private GridLayoutGroup stashLayout;
    [SerializeField] private GridLayoutGroup deckLayout;
    [SerializeField] private CardUI cardPrefabStash;
    [SerializeField] private CardUI cardPrefabDeck;

    List<CardUI> availableCards = new List<CardUI>();
    List<CardUI> cardsInDeck = new List<CardUI>();

    [SerializeField] private TextMeshProUGUI deckLabel;
    [SerializeField] private DeckObject[] decks;
    [SerializeField] private int selectedDeckIdx = -1;
    [SerializeField] private DeckObject selectedDeck;

    private void Start()
    {
        LoadAvailableCards();

        SelectNextDeck();
    }

    private void LoadAvailableCards()
    {

        List<CardData> availableCardsData = APIHelper.GetCards().Where(c=>c.types != null && c.types.Count > 0).ToList();
        foreach(CardData cardData in availableCardsData)
        {
            CardUI card = Instantiate(cardPrefabStash, stashLayout.transform, false);
            card.deckCard.cardData = cardData;
            card.SetData();
            card.onCardClicked += AddCard;

            availableCards.Add(card);
        }
    }

    private void LoadDeckCards()
    {
        if (selectedDeck == null)
        {
            Debug.LogError("No deck selected");
            return;
        }

        // clear cards
        cardsInDeck.Clear();
        foreach (Transform child in deckLayout.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var deckCard in selectedDeck.GetCards())
        {
            CardUI card = Instantiate(cardPrefabDeck, deckLayout.transform, false);
            card.deckCard.cardData = deckCard;
            card.SetData(deckCard.name);
            card.onSelectedClicked += RemoveCard;
            cardsInDeck.Add(card);
        }
    }

    public void SelectNextDeck()
    {
        if(decks.Length <= 0)
        {
            return;
        }

        if(selectedDeck == null)
        {
            selectedDeck = decks[0];
            LoadDeckCards();
            return;
        }

        if(selectedDeckIdx + 1 >= decks.Length)
        {
            selectedDeckIdx = 0;
        }
        else
        {
            selectedDeckIdx++;
        }

        selectedDeck = decks[selectedDeckIdx];
        deckLabel.SetText(selectedDeck.name);

        LoadDeckCards();
    }

    public void AddCard(CardUI card)
    {
        CardUI newCard = Instantiate(cardPrefabDeck, deckLayout.transform, false);
        newCard.deckCard.cardData = card.deckCard.cardData;
        newCard.onSelectedClicked += RemoveCard;
        newCard.SetData(newCard.deckCard.cardData.name);
        cardsInDeck.Add(newCard);
        selectedDeck.AddCard(newCard.deckCard.cardData);
    }

    public void RemoveCard(CardUI card)
    {
        cardsInDeck.Remove(card);
        selectedDeck.RemoveCard(card.deckCard.cardData);
        Destroy(card.gameObject);
    }

    public void Sort(int shortBy)
    {
        // clear layout
        stashLayout.transform.DetachChildren();
        deckLayout.transform.DetachChildren();

        switch ((SortBy)shortBy)
        {
            case SortBy.HP:
                SortByHp(availableCards);
                SortByHp(cardsInDeck);
                break;
            case SortBy.TYPE:
                SortByType(availableCards);
                SortByType(cardsInDeck);
                break;
            case SortBy.RARITY:
                SortByRarity(availableCards);
                SortByRarity(cardsInDeck);
                break;
            default:
                SortById(availableCards);
                SortById(cardsInDeck);
                break;

        }

        // set layout
        foreach (var item in availableCards)
            item.transform.SetParent(stashLayout.transform);
        foreach (var item1 in cardsInDeck)
            item1.transform.SetParent(deckLayout.transform);
    }

    private void SortById(List<CardUI> cards)
    {
        cards.Sort(delegate (CardUI x, CardUI y)
        {
            if (x.deckCard.cardData.id == null && y.deckCard.cardData.id == null) return 0;
            else if (x.deckCard.cardData.id == null) return -1;
            else if (y.deckCard.cardData.id == null) return 1;
            else return x.deckCard.cardData.id.CompareTo(y.deckCard.cardData.id);
        });
    }

    private void SortByRarity(List<CardUI> cards)
    {
        cards.Sort(delegate (CardUI x, CardUI y)
        {
            if (x.deckCard.cardData.rarity == null && y.deckCard.cardData.rarity == null) return 0;
            else if (x.deckCard.cardData.rarity == null) return -1;
            else if (y.deckCard.cardData.rarity == null) return 1;
            else return x.deckCard.cardData.rarity.CompareTo(y.deckCard.cardData.rarity);
        });
    }

    private void SortByType(List<CardUI> cards)
    {
        cards.Sort(delegate (CardUI x, CardUI y)
        {
            if ((x.deckCard.cardData.types == null && y.deckCard.cardData.types == null) ||
                (x.deckCard.cardData.types.Count <= 0 && y.deckCard.cardData.types.Count <= 0)
            ) return 0;
            else if (x.deckCard.cardData.types == null || x.deckCard.cardData.types.Count <= 0) return -1;
            else if (y.deckCard.cardData.types == null || y.deckCard.cardData.types.Count <= 0) return 1;
            else return x.deckCard.cardData.types[0].CompareTo(y.deckCard.cardData.types[0]);
        });
    }

    private void SortByHp(List<CardUI> cards)
    {
        cards.Sort(delegate (CardUI x, CardUI y)
        {
            try
            {
                var hp1 = int.Parse(x.deckCard.cardData.hp);
                var hp2 = int.Parse(y.deckCard.cardData.hp);
                if (hp1 > hp2)
                    return -1;
                else if (hp1 < hp2)
                    return 1;
                else
                    return 0;
            }
            catch { return 0; }
        });
    }
}
