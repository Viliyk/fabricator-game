using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fabricator.Units;

namespace Fabricator.InputManager
{
    public class InputHandler : MonoBehaviour
    {
        public static InputHandler instance;

        private Camera mainCamera;
        private RaycastHit hit;

        void Start()
        {
            instance = this;

            mainCamera = Camera.main;
        }

        void Update()
        {

        }

        public void HandleUnitMovement()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                if(Physics.Raycast(ray, out hit))
                {
                    LayerMask layerHit = hit.transform.gameObject.layer;

                    switch (layerHit.value)
                    {
                        case 6:
                            break;
                        case 7:
                            break;
                        case 9:
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
