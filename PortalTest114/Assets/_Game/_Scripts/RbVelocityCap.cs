using UnityEngine;


public class RbVelocityCap : MonoBehaviour
{
    [Header("Veclocity Cap Settings")]
    public Rigidbody rigidBody;
    public float maxVelocity = 10f;

    private void FixedUpdate()
    {
        if (!rigidBody)
            return;

        if (rigidBody.isKinematic)
            return;

        rigidBody.linearVelocity = Vector3.ClampMagnitude(rigidBody.linearVelocity, maxVelocity);
    }
}
