using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        // Update is called once per frame
        void Update()
        {

        }

        public void HandleUnitMovement()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                if(Physics.Raycast(ray, out hit))
                {

                }
            }
        }
    }
}
