using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    float speed = 0.01f;
    float zoomSpeed = 10.0f;
    float rotationSpeed = 0.1f;

    float maxHeight = 140f;
    float minHeight = 10f;

    Vector2 p1;
    Vector2 p2;

    void LateUpdate()
    {
        // Old input system
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

        MiddleClickScroll();
    }

    void MiddleClickScroll()
    {
        // Store mouse position when middle click is pressed
        if (Mouse.current.middleButton.wasPressedThisFrame)
        {
            p1 = Mouse.current.position.ReadValue();
        }
        // Store mouse position when middle click is being held
        if (Mouse.current.middleButton.isPressed)
        {
            p2 = Mouse.current.position.ReadValue();

            float dx = (p2 - p1).x * rotationSpeed;
            float dy = (p2 - p1).y * rotationSpeed;

            Vector3 move = new Vector3(-dx, 0, -dy);

            transform.position += move;

            p1 = p2;
        }
    }

    //void GetCameraRotation()
    //{
    //    if (Mouse.current.middleButton.wasPressedThisFrame)    // store mouse position when middle click is pressed
    //    {
    //        p1 = Mouse.current.position.ReadValue();
    //    }

    //    if (Mouse.current.middleButton.isPressed)        // store mouse position when middle click is being held
    //    {
    //        p2 = Mouse.current.position.ReadValue();

    //        float dx = (p2 - p1).x * rotationSpeed;
    //        float dy = (p2 - p1).y * rotationSpeed;

    //        transform.rotation *= Quaternion.Euler(new Vector3(0, dx, 0));

    //        p1 = p2;
    //    }
    //}
}
