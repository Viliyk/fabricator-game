using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private ShopManager shopManager = null;
    [SerializeField] private GameObject map = null;
    [SerializeField] private GameObject mapNode = null;
    [SerializeField] private ChoicePanel choicePanel = null;

    private RectTransform rt;

    public int turnNumber;

    void OnEnable()
    {
        turnNumber = shopManager.turnNumber;
        while (turnNumber > 10)
            turnNumber -= 10;
            

        if (GlobalControl.Instance.currentNode == 1)    // credits
            OfferCredits(25);
        if (GlobalControl.Instance.currentNode == 2)    // rest
            Heal(10);
        if (GlobalControl.Instance.currentNode == 3)    // relic
            OfferRelic(1);
        if (GlobalControl.Instance.currentNode == 5)    // elite
            OfferCredits(50);

        /*
        mapNode = Instantiate(mapNode, new Vector3(200, 100, 0), Quaternion.identity);
        mapNode.transform.SetParent(map.transform);
        mapNode.GetComponent<MapNode>().nodeType = MapNode.Node.RELIC;
        */

        rt = map.GetComponent<RectTransform>();

        rt.anchoredPosition = new Vector2(0, Mathf.Clamp(rt.anchoredPosition.y + (turnNumber - 1) * 100, -460, 330));

        map.SetActive(true);
    }

    void Update()
    {
        float mapY = rt.anchoredPosition.y;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f && rt.anchoredPosition.y > -430)
            rt.anchoredPosition = new Vector2(0, mapY - 40);
        if (Input.GetAxis("Mouse ScrollWheel") < 0f && rt.anchoredPosition.y < 330)
            rt.anchoredPosition = new Vector2(0, mapY + 40);
    }

    public void OfferCredits(int amount)
    {
        shopManager.credits = shopManager.credits + amount;
    }

    public void Heal(int amount)
    {
        shopManager.lives = Mathf.Clamp(shopManager.lives + amount, 0, GlobalControl.Instance.maxLives);
    }

    public void OfferRelic(int tier)
    {
        choicePanel.SetPoolRelic(tier);
    }

    public void CloseMap()
    {
        shopManager.StartOfTurnEffects();
        map.SetActive(false);
        gameObject.SetActive(false);
    }
}
