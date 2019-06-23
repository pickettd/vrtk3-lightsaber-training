using UnityEngine;
using System.Collections;

// By default an object would keep on rotating after impact and keep on spinning. This class fixes the rotation, so that there is no spinning.
// Example: laster shots shouldn't be spinning after an impact
public class FixRotationOnImpact : MonoBehaviour {

    private bool rotateEnabled;

    private Rigidbody rigidBody;

    public bool debug = false;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        // Debug.Log("Rotation: " + rigidBody.transform.rotation);
    }

    void Update()
    {
        if (rotateEnabled)
        {
            // get velocity after impact
            Vector3 velocity = rigidBody.velocity;

            // Debug.Log("Velocity: " + velocity);

            // rotate towards velocity
            transform.rotation = Quaternion.LookRotation(velocity);

            // correct bullet rotation
            // TODO: this is proprietary, depending on the rotation in our prefab
            transform.Rotate(Vector3.left * 90);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        rotateEnabled = false;

        if (debug) { 
            // Debug-draw all contact points and normals
            foreach (ContactPoint contact in collision.contacts)
            {

                float length = 5;
                float duration = 2;

                // get the hit normal
                // http://docs.unity3d.com/ScriptReference/ContactPoint-normal.html
                Vector3 normalVector = contact.normal * length;
                Debug.DrawRay(contact.point, normalVector, Color.red, duration);

                // get movement velocity
                Vector3 velocityVector = rigidBody.velocity * length;
                Debug.DrawRay(contact.point, velocityVector, Color.blue, duration);


            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        rotateEnabled = true;
    }
}
