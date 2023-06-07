using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerup : Powerup
{
    [SerializeField] float speedModifier = 1.2f;
    [SerializeField] float duration = 5f;

    public override void Collect(WormController controller)
    {
        controller.ModifySpeed(speedModifier, duration);
    }
}
