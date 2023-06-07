using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPowerup : Powerup
{
    [SerializeField] float duration = 5f;

    public override void Collect(WormController controller)
    {
        controller.ActivateShield(duration);
    }
}
