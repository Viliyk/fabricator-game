using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlideController : MonoBehaviour
{
    private float timeToMove = 1.0f;
    private float speed;
    private float returnSpeed;
    private bool moving = false;
    private bool returning = false;

    private Vector2 destination;
    private Vector2 startingPosition;

    private RectTransform rt;

    void Start()
    {
        rt = gameObject.GetComponent<RectTransform>();
    }

    void Update()
    {
        // If the object is not at the target destination
        if (moving)
            MoveToTarget();

        if (returning)
            MoveBack();
    }

    void MoveToTarget()
    {
        //720
        //1500
        //speed = 1500 / BattleManager.battleSpeed;
        // Calculate the next position
        //float delta = speed * Time.deltaTime;
        //Vector3 currentPosition = gameObject.transform.position;

        speed += Time.deltaTime / (BattleManager.battleSpeed / timeToMove);
        timeToMove += 0.1f;
        //Vector2 currentPosition = rt.anchoredPosition;
        Vector2 currentPosition = transform.position;
        Vector2 nextPosition = Vector2.Lerp(startingPosition, destination, speed);

        // Move the object to the next position
        gameObject.transform.position = nextPosition;
        //rt.anchoredPosition = nextPosition;

        if (currentPosition == destination)
        {
            moving = false;
            //returning = true;
        }
    }

    void MoveBack()
    {
        returnSpeed += Time.deltaTime / (BattleManager.battleSpeed / timeToMove);
        Vector2 currentPosition = rt.anchoredPosition;
        Vector2 nextPosition = Vector2.Lerp(destination, startingPosition, returnSpeed);

        rt.anchoredPosition = nextPosition;

        if (currentPosition == startingPosition)
            returning = false;
    }

    // Set the destination to cause the object to smoothly glide to the specified location
    public void SetDestination(Vector2 value)
    {
        //if (value == gameObject.transform.position)
        //if (value == rt.anchoredPosition)
        //{
        //    active = false;
        //    return;
        //}

        timeToMove = 1.0f;
        speed = 0f;
        returnSpeed = 0f;
        startingPosition = rt.anchoredPosition;
        //startingPosition = transform.position;
        destination = value;
        moving = true;
    }
}
