using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormInputHandler : MonoBehaviour
{
    [SerializeField] string inputAxis = "Horizontal";

    WormController controller;

    private void Awake()
    {
        controller = GetComponent<WormController>();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        int inputX = Mathf.RoundToInt(Input.GetAxis(inputAxis));
        controller.Move(inputX);
    }

}
