using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailFire : Mob
{
    float MovementSpeed { get; } = 16f;
    public Vector3 Direction { get; set; }

    private void Update()
    {
        Walk(Direction * Time.deltaTime * MovementSpeed);

        if (Physics.CheckSphere(transform.position, .6f, ObstructionMask))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMob playerMob = other.GetComponent<PlayerMob>();

        if (playerMob == null)
        {
            return;
        }

        HurtPlayer(playerMob);
    }

    void HurtPlayer(PlayerMob playerMobInstance)
    {
        if (PhaseManager.CurrentGameState != GameState.Movement && PhaseManager.CurrentGameState != GameState.Exhaustion)
        {
            return;
        }

        if (playerMobInstance.InHurtTime)
        {
            return;
        }

        playerMobInstance.TakeDamage(1);
        Destroy(this.gameObject);
    }
}
