using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private bool move = false;
    private float startingPositionX;
    private float startingPositionY;
    private float enemyPositionX;
    private float enemyPositionY;

    // Start is called before the first frame update
    void Start()
    {
        startingPositionX = transform.position.x;
        startingPositionY = transform.position.y;

        Destroy(gameObject, 0.006f / BattleManager.chargeRate);
    }

    // Update is called once per frame
    void Update()
    {
        if (move == true)
            transform.Translate((enemyPositionX - startingPositionX) * Time.deltaTime * BattleManager.chargeRate * 150, (enemyPositionY - startingPositionY) * Time.deltaTime * BattleManager.chargeRate * 150, 0);
        //transform.Translate((enemyPositionX - startingPositionX) * Time.deltaTime / BattleManager.battleSpeed / 15, (enemyPositionY - startingPositionY) * Time.deltaTime / BattleManager.battleSpeed / 15, 0);
    }

    public void StartMoving(float x, float y)
    {
        enemyPositionX = x;
        enemyPositionY = y;
        move = true;
    }
}
