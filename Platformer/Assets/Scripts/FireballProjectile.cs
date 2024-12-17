using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballProjectile : BasicProjectile
{
    protected override void OnHitEffect()
    {
        Debug.Log("Burning enemy...");
    }
}
