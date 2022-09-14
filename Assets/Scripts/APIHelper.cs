using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using SimpleJSON;

public static class APIHelper
{
    public static CardData GetCard(int id)
    {
        //string apiURL = "https://api.pokemontcg.io/v2/cards/?q=nationalPokedexNumbers:[387%20TO%20493]";
        //Creating request
        string apiURL = "https://api.pokemontcg.io/v2/cards/dp1-" + id;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiURL);
        //Getting response from request
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //Getting the value stream into a reader from the response
        StreamReader reader = new StreamReader(response.GetResponseStream());
        //Passing the reader into a string
        string json = reader.ReadToEnd();

        /*CardData cd = JsonUtility.FromJson(json, typeof(CardData)) as CardData;
        Debug.LogError(cd.name);
        return cd;*/

        JSONNode cardKeyValuePairs = JSON.Parse(json);

        CardData cardData = new CardData();
        cardData.id = cardKeyValuePairs["data"]["id"];
        cardData.name = cardKeyValuePairs["data"]["name"];
        cardData.hp = cardKeyValuePairs["data"]["hp"];
        cardData.rarity = cardKeyValuePairs["data"]["rarity"];
        cardData.small = cardKeyValuePairs["data"]["images"]["small"];
        cardData.types = new List<string>();

        JSONNode typesKeyValuePairs = cardKeyValuePairs["data"]["types"];
        
        foreach(JSONNode type in typesKeyValuePairs)
        {
            cardData.types.Add(type.ToString());
        }
        

        return cardData;

    }

    public static List<CardData> GetCards()
    {
        string apiURL = "https://api.pokemontcg.io/v2/cards/?q=set.id:dp1";
        //Creating request
        //string apiURL = "https://api.pokemontcg.io/v2/cards/dp1-" + id;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiURL);
        //Getting response from request
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        if(response.StatusCode == HttpStatusCode.OK)
        {
            Debug.Log("OK! - 200");
        }
        //Getting the value stream into a reader from the response
        StreamReader reader = new StreamReader(response.GetResponseStream());
        //Passing the reader into a string
        string json = reader.ReadToEnd();

        /*CardData cd = JsonUtility.FromJson(json, typeof(CardData)) as CardData;
        Debug.LogError(cd.name);
        return cd;*/

        JSONNode dataKeyValuePairs = JSON.Parse(json);
        List<CardData> cards = new List<CardData>();
       
        foreach(JSONNode cardKeyValuePairs in dataKeyValuePairs["data"])
        {
            CardData cardData = new CardData();
            cardData.id = cardKeyValuePairs["id"];
            cardData.name = cardKeyValuePairs["name"];
            cardData.hp = cardKeyValuePairs["hp"];
            cardData.rarity = cardKeyValuePairs["rarity"];
            cardData.small = cardKeyValuePairs["images"]["small"];
            cardData.types = new List<string>();

            JSONNode typesKeyValuePairs = cardKeyValuePairs["types"];

            foreach (JSONNode type in typesKeyValuePairs)
            {
                cardData.types.Add(type.ToString());
            }

            cards.Add(cardData);
        }

        return cards;

    }
}
