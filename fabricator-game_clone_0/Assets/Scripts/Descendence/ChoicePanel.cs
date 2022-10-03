using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoicePanel : MonoBehaviour
{
    [SerializeField] private ShopManager shopManager = null;
    [SerializeField] private GameObject cardTemplate = null;
    [SerializeField] private GameObject consumableTemplate = null;
    [SerializeField] private GameObject relicTemplate = null;

    private ThisCard createdCard;
    private GameObject spawnedCard;
    private ThisConsumable createdConsumable;
    private GameObject spawnedConsumable;
    private ThisRelic createdRelic;
    private GameObject spawnedRelic;

    private ThisCard cardOne;
    private ThisCard cardTwo;
    private ThisCard cardThree;

    private ThisConsumable consumableOne;
    private ThisConsumable consumableTwo;
    private ThisConsumable consumableThree;

    private ThisRelic relicOne;
    private ThisRelic relicTwo;
    private ThisRelic relicThree;

    private ThisCard chosenCard;
    private ThisConsumable chosenConsumable;
    private ThisRelic chosenRelic;

    private bool isCard;
    private bool isConsumable;
    private bool isRelic;

    private List<int> availableCards = new List<int>();

    void OnDisable()
    {
        if (cardOne != null)
            Destroy(cardOne.gameObject);
        if (cardTwo != null)
            Destroy(cardTwo.gameObject);
        if (cardThree != null)
            Destroy(cardThree.gameObject);

        if (consumableOne != null)
            Destroy(consumableOne.gameObject);
        if (consumableTwo != null)
            Destroy(consumableTwo.gameObject);
        if (consumableThree != null)
            Destroy(consumableThree.gameObject);

        if (relicOne != null)
            Destroy(relicOne.gameObject);
        if (relicTwo != null)
            Destroy(relicTwo.gameObject);
        if (relicThree != null)
            Destroy(relicThree.gameObject);
    }

    public void SetPoolTriple(int tier)
    {
        isCard = true;

        availableCards.Clear();
        foreach (KeyValuePair<int, Card> card in CardDB.cardList)
        {
            if (card.Value.tier == tier)
                availableCards.Add(card.Value.id);
        }

        cardOne = SpawnCard();
        cardTwo = SpawnCard();
        cardThree = SpawnCard();

        transform.parent.gameObject.SetActive(true);
    }

    public void SetPoolConsumable(int tier)
    {
        isConsumable = true;

        availableCards.Clear();
        foreach (KeyValuePair<int, Consumable> consumable in ConsumableDB.consumableList)
        {
            if (consumable.Value.tier == tier)
                availableCards.Add(consumable.Value.id);
        }

        consumableOne = SpawnConsumable();
        consumableTwo = SpawnConsumable();
        consumableThree = SpawnConsumable();

        transform.parent.gameObject.SetActive(true);
    }

    public void SetPoolRelic(int tier)
    {
        isRelic = true;

        availableCards.Clear();
        foreach (KeyValuePair<int, Relic> relic in RelicDB.relicList)
        {
            if (relic.Value.tier == tier)
                availableCards.Add(relic.Value.id);
        }
        foreach (int x in GlobalControl.Instance.ownedRelics)
            availableCards.Remove(x);

        if (availableCards.Count == 0)
            return;

        relicOne = SpawnRelic();
        relicTwo = SpawnRelic();
        relicThree = SpawnRelic();

        transform.parent.gameObject.SetActive(true);
    }

    private ThisCard SpawnCard()
    {
        if (availableCards.Count == 0)
            return null;

        createdCard = cardTemplate.GetComponent<ThisCard>();
        createdCard.thisId = availableCards[Random.Range(0, availableCards.Count)];
        availableCards.Remove(createdCard.thisId);
        spawnedCard = Instantiate(cardTemplate, new Vector3(0, 0, 0), Quaternion.identity);
        spawnedCard.GetComponent<Draggable>().enabled = false;
        spawnedCard.transform.SetParent(transform, false);
        spawnedCard.transform.localScale = new Vector3(1f, 1f, 1);
        createdCard = spawnedCard.GetComponent<ThisCard>();

        return createdCard;
    }

    private ThisConsumable SpawnConsumable()
    {
        if (availableCards.Count == 0)
            return null;

        createdConsumable = consumableTemplate.GetComponent<ThisConsumable>();
        createdConsumable.thisId = availableCards[Random.Range(0, availableCards.Count)];
        availableCards.Remove(createdConsumable.thisId);
        spawnedConsumable = Instantiate(consumableTemplate, new Vector3(0, 0, 0), Quaternion.identity);
        spawnedConsumable.GetComponent<Draggable>().enabled = false;
        spawnedConsumable.transform.SetParent(transform, false);
        spawnedConsumable.transform.localScale = new Vector3(1f, 1f, 1);
        createdConsumable = spawnedConsumable.GetComponent<ThisConsumable>();

        return createdConsumable;
    }

    private ThisRelic SpawnRelic()
    {
        if (availableCards.Count == 0)
            return null;

        createdRelic = relicTemplate.GetComponent<ThisRelic>();
        createdRelic.thisId = availableCards[Random.Range(0, availableCards.Count)];
        availableCards.Remove(createdRelic.thisId);
        spawnedRelic = Instantiate(relicTemplate, new Vector3(0, 0, 0), Quaternion.identity);
        spawnedRelic.transform.SetParent(transform, false);
        spawnedRelic.transform.localScale = new Vector3(1f, 1f, 1);
        createdRelic = spawnedRelic.GetComponent<ThisRelic>();
        createdRelic.cooldown = true;

        return createdRelic;
    }

    // button
    public void ChoiceOne()
    {
        if (isCard)
        {
            chosenCard = cardOne;
            shopManager.SpawnPlayerCard(chosenCard.id, true);
            isCard = false;
        }
        else if (isConsumable)
        {
            chosenConsumable = consumableOne;
            shopManager.SpawnConsumable(chosenConsumable.id);
            isConsumable = false;
        }
        else if (isRelic)
        {
            chosenRelic = relicOne;
            GlobalControl.Instance.ownedRelics.Add(chosenRelic.id);
            shopManager.SpawnRelic(chosenRelic.id);
            isRelic = false;
        }

        transform.parent.gameObject.SetActive(false);
    }

    // button
    public void ChoiceTwo()
    {
        if (isCard)
        {
            chosenCard = cardTwo;
            shopManager.SpawnPlayerCard(chosenCard.id, true);
            isCard = false;
        }
        else if (isConsumable)
        {
            chosenConsumable = consumableTwo;
            shopManager.SpawnConsumable(chosenConsumable.id);
            isConsumable = false;
        }
        else if (isRelic)
        {
            chosenRelic = relicTwo;
            GlobalControl.Instance.ownedRelics.Add(chosenRelic.id);
            shopManager.SpawnRelic(chosenRelic.id);
            isRelic = false;
        }

        transform.parent.gameObject.SetActive(false);
    }

    // button
    public void ChoiceThree()
    {
        if (isCard)
        {
            chosenCard = cardThree;
            shopManager.SpawnPlayerCard(chosenCard.id, true);
            isCard = false;
        }
        else if (isConsumable)
        {
            chosenConsumable = consumableThree;
            shopManager.SpawnConsumable(chosenConsumable.id);
            isConsumable = false;
        }
        else if (isRelic)
        {
            chosenRelic = relicThree;
            GlobalControl.Instance.ownedRelics.Add(chosenRelic.id);
            shopManager.SpawnRelic(chosenRelic.id);
            isRelic = false;
        }

        transform.parent.gameObject.SetActive(false);
    }
}
