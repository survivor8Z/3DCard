using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTurn : MonoBehaviour
{
    public void TurnLeft(InputAction.CallbackContext context)
    {
        if (context.performed)  
            Turn(false);
    }
    public void TurnRight(InputAction.CallbackContext context)
    {
        if(context.performed)
            Turn(true);
    }
    void Turn(bool isTurnRight)
    {
        transform.rotation *= Quaternion.AngleAxis(90 * (isTurnRight?1:-1), transform.up);
    }
}
