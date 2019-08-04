using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour
{
    // Unity doesn't like it when you're *too* close to things
    float PlanckDistance = .001f;
    public LayerMask ObstructionMask;
    public SpriteRenderer MobSprite;

    protected void Walk(Vector3 toWalk)
    {
        Vector3 workingMovementDelta = Vector3.zero;

        RaycastHit raycastHit;

        if (toWalk.x != 0)
        {
            if (Physics.SphereCast(transform.position, .5f - PlanckDistance, Vector3.right * Mathf.Sign(toWalk.x), out raycastHit, Mathf.Abs(toWalk.x), ObstructionMask))
            {
                workingMovementDelta.x = (raycastHit.distance - PlanckDistance) * Mathf.Sign(toWalk.x);
            }
            else
            {
                workingMovementDelta.x = toWalk.x;
            }
        }

        if (toWalk.z != 0)
        {
            if (Physics.SphereCast(transform.position, .5f - PlanckDistance, Vector3.forward * Mathf.Sign(toWalk.z), out raycastHit, Mathf.Abs(toWalk.z), ObstructionMask))
            {
                workingMovementDelta.z = (raycastHit.distance - PlanckDistance) * Mathf.Sign(toWalk.z);
            }
            else
            {
                workingMovementDelta.z = toWalk.z;
            }
        }

        transform.position = transform.position + workingMovementDelta;

        if (toWalk.x != 0)
        {
            if (toWalk.x > 0)
            {
                MobSprite.flipX = false;
            }
            else
            {
                MobSprite.flipX = true;
            }
        }
    }
}
