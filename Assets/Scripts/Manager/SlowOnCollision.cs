using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class SlowOnCollision : MonoBehaviour
{
    public float slowWeight = 0.5f;

    private void OnCollisionStay2D(Collision2D other)
    {
        BoatController playerRB = other.gameObject.GetComponent<BoatController>();
        EnemyBoatAI enemyRB = other.gameObject.GetComponent<EnemyBoatAI>();

        if (playerRB)
        {
            float velocity = playerRB.speed;
            velocity *= (1 - slowWeight);
            playerRB.speed = velocity;
        }
        if (enemyRB)
        {
            float velocity = enemyRB.speed;
            velocity *= (1 - slowWeight);
            enemyRB.speed = velocity;
        }
    }
}
