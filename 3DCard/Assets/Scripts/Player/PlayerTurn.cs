using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTurn : MonoBehaviour
{
    void OnTurn(InputValue value)
    {
        Turn(value.Get<float>() > 0);
    }
    void Turn(bool isTurnRight)
    {
        transform.rotation *= Quaternion.AngleAxis(90 * (isTurnRight?1:-1), transform.up);
    }
}
