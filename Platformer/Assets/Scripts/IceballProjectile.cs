using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceballProjectile : BasicProjectile
{
    protected override void OnHitEffect()
    {
        Debug.Log("Slowing enemy...");
    }
}
