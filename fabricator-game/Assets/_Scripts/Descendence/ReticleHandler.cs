using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleHandler : MonoBehaviour, IPointerClickHandler
{
    public GameObject a;
    public float spot;

    int admissionNumber;
    Ray ray;

    // Start is called before the first frame update
    void Start()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = (Input.mousePosition + a.transform.position) / spot;       // follow the mouse cursor

        // right click destroys the reticle
        if (Input.GetMouseButton(1))
        {
            GlobalControl.Instance.targetMode = false;
            Destroy(gameObject);
        }

        //GlobalControl.Instance.targetMode = true;

        //if (Input.GetMouseButtonUp(0))
        //{
        //    //RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.up);
        //    RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        //    if (hit.transform != null)
        //    {
        //        Debug.Log("Target Name: " + hit.transform.gameObject.name);
        //    }
        //    else
        //        print("clicked");
        //}
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //ThisCard t = eventData.pointerClick.GetComponent<ThisCard>();
        //if (t != null)
        //    t.attack += 10;
    }
}
