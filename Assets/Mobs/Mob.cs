using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour
{
    public LayerMask WallMask;

    protected void Walk(Vector3 toWalk)
    {
        Vector3 workingMovementDelta = Vector3.zero;

        RaycastHit raycastHit;

        if (toWalk.x != 0)
        {
            if (Physics.BoxCast(transform.position, Vector3.one / 2f, Vector3.right * Mathf.Sign(toWalk.x), out raycastHit, Quaternion.identity, Mathf.Abs(toWalk.x), WallMask))
            {
                workingMovementDelta.x = raycastHit.distance * Mathf.Sign(toWalk.x);
            }
            else
            {
                workingMovementDelta.x = toWalk.x;
            }
        }

        if (toWalk.z != 0)
        {
            if (Physics.BoxCast(transform.position, Vector3.one / 2f, Vector3.forward * Mathf.Sign(toWalk.z), out raycastHit, Quaternion.identity, Mathf.Abs(toWalk.z), WallMask))
            {
                workingMovementDelta.z = raycastHit.distance * Mathf.Sign(toWalk.z);
            }
            else
            {
                workingMovementDelta.z = toWalk.z;
            }
        }

        transform.position = transform.position + workingMovementDelta;
    }
}
