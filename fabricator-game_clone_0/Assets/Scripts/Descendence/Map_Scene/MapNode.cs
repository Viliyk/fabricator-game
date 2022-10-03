using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MapNode : MonoBehaviour
{
    public TMP_Text buttonText;
    public Image viableNodeArrow;
    private Image nodeImage;
    private GameObject mapManagerObject;
    private MapManager mapManager;
    private bool viableNode = false;

    public enum Node { NONE, CREDITS, REST, RELIC, TRADER, ELITE, BOSS }
    public Node nodeType;

    public int nodeLevel;
    public int pathNumber;

    void Start()
    {
        nodeImage = GetComponent<Image>();
        mapManagerObject = GameObject.FindWithTag("Map Manager");
        mapManager = mapManagerObject.GetComponent<MapManager>();

        //nodeType = (Node)Random.Range(0, System.Enum.GetValues(typeof(Node)).Length);
        //if (nodeLevel == 1)
        //    nodeType = Node.BATTLE;

        buttonText.text = nodeType.ToString();
        if (nodeType == Node.CREDITS)
            nodeImage.color = new Color32(255, 0, 0, 255); // red
        if (nodeType == Node.REST)
            nodeImage.color = new Color32(0, 255, 0, 255); // green
        if (nodeType == Node.RELIC)
            nodeImage.color = new Color32(255, 215, 0, 255); // gold
        if (nodeType == Node.TRADER)
            nodeImage.color = new Color32(0, 55, 255, 255); // blue
        if (nodeType == Node.ELITE)
            nodeImage.color = new Color32(255, 0, 0, 255); // red
        if (nodeType == Node.BOSS)
            nodeImage.color = new Color32(255, 0, 0, 255); // red

        //CheckNodeViability();
    }

    private void Update()
    {
        CheckNodeViability();
    }

    private void CheckNodeViability()
    {
        if (mapManager.turnNumber > 10 && nodeType == Node.BOSS || GlobalControl.Instance.debugMode == true)
        {
            viableNode = true;
            viableNodeArrow.gameObject.SetActive(true);
        }

        if (nodeLevel != mapManager.turnNumber)
            return;

        int p = GlobalControl.Instance.pathNumber;
        if (p == 0 || pathNumber == p || pathNumber == p - 1 || pathNumber == p + 1)
        {
            viableNode = true;
            viableNodeArrow.gameObject.SetActive(true);
        }
    }

    // button
    public void ButtonPress()
    {
        if (viableNode == true)
        {
            GlobalControl.Instance.turnNumber = nodeLevel;
            GlobalControl.Instance.pathNumber = pathNumber;
            GlobalControl.Instance.currentNode = (int)nodeType;

            mapManager.CloseMap();
        }
    }
}
