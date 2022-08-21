using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    float speed = 0.01f;
    float zoomSpeed = 10.0f;
    float rotationSpeed = 0.1f;

    float maxHeight = 140f;
    float minHeight = 10f;

    Vector2 p1;
    Vector2 p2;

    void Update()
    {
        float hsp = transform.position.y * speed * Input.GetAxis("Horizontal");
        float vsp = transform.position.y * speed * Input.GetAxis("Vertical");
        float scrollSp = Mathf.Log(transform.position.y) * -zoomSpeed * Input.GetAxis("Mouse ScrollWheel");

        if (transform.position.y >= maxHeight && scrollSp > 0)
        {
            scrollSp = 0;
        }
        else if (transform.position.y <= minHeight && scrollSp < 0)
        {
            scrollSp = 0;
        }

        if (transform.position.y + scrollSp > maxHeight)
        {
            scrollSp = maxHeight - transform.position.y;
        }
        else if (transform.position.y + scrollSp < minHeight)
        {
            scrollSp = minHeight - transform.position.y;
        }

        Vector3 verticalMove = new Vector3(0, scrollSp, -scrollSp * 0.75f);
        Vector3 lateralMove = hsp * transform.right;
        Vector3 forwardMove = transform.forward;
        forwardMove.y = 0;
        forwardMove.Normalize();
        forwardMove *= vsp;

        Vector3 move = verticalMove + lateralMove + forwardMove;

        transform.position += move;

        GetCameraRotation();
    }

    void GetCameraRotation()
    {
        if (Input.GetMouseButtonDown(2))    // store mouse position when middle click is pressed
        {
            p1 = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))        // store mouse position when middle click is being held
        {
            p2 = Input.mousePosition;

            float dx = (p2 - p1).x * rotationSpeed;
            float dy = (p2 - p1).y * rotationSpeed;

            transform.rotation *= Quaternion.Euler(new Vector3(0, dx, 0));

            p1 = p2;
        }
    }
}
