using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyAttack : BaseEnemyAttack
{
    // TODO: Different attack animations, etc
    protected override void AttackPlayer(PlayerHealth player)
    {
        player.TakeDamage(attackDamage);
    }
}
