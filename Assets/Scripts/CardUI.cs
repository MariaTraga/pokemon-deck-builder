using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public Image cardImage;
    public TextMeshProUGUI cardName;
    public DeckCard deckCard;


    public event Action<CardUI> onCardClicked, onSelectedClicked, onCardBeginDrag, onCardEndDrag, onCardDropped;

    ImageDownloadHandler downloadHandler; //to make singleton (make gamemanager singleton that holds reference)

    private Action<Texture2D> onSuccess;
    private Action<Exception> onFailure;

    private void Awake()
    {
        downloadHandler = GetComponent<ImageDownloadHandler>(); //TO MAKE SINGLETON
        cardName = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetData(String name)
    {
        if (cardName)
        {
            cardName.SetText(name);
            cardName.ForceMeshUpdate(true);
        }   
    }

    public void SetData()
    {
        if (cardImage)
        {
            downloadHandler = GetComponent<ImageDownloadHandler>();
            downloadHandler.DownloadImage(deckCard.cardData.small, deckCard.cardData.name, OnSuccess, OnFailure, false);
        }
    }

    public void ClearData()
    {
        cardName.text = null;
    }

    public void OnBeginDrag()
    {
        onCardBeginDrag?.Invoke(this);
    }

    public void OnEndDrag()
    {
        onCardEndDrag?.Invoke(this);
    }

    public void OnDropped()
    {
        onCardDropped?.Invoke(this);
    }

    public void OnCardClicked(BaseEventData baseEventData)
    {     
        onCardClicked?.Invoke(this);
    }

    public void OnSelectedClicked(BaseEventData baseEventData)
    {
        onSelectedClicked?.Invoke(this);
    }

    private void OnSuccess(Texture2D texture)
    {
        if (cardImage)
        {
            cardImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f)); //0.5f center pivot
            cardImage.transform.localScale = new Vector3(1f, 1f, 1f);
            //Debug.Log(cardImage.sprite.pivot);
        }

    }

    private void OnFailure(Exception e)
    {
        Debug.Log(e.Message);
    }
}
