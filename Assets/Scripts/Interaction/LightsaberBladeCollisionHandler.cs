using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsaberBladeCollisionHandler : MonoBehaviour {

    public AudioClip soundCollision;
    public AudioSource AudioSourceCollision;


    // just a quick collision test when the lightsaber gets hit
    // source: https://unity3d.com/learn/tutorials/topics/physics/detecting-collisions-oncollisionenter
    // TODO: check only beam collision
    void OnCollisionEnter(Collision collision)
    {

        Debug.Log("collision: " + collision + ", name: " + collision.gameObject.name);

        if (collision.gameObject.name == "Laser Bullet(Clone)") // TODO: depend on object type, not on some name
        {
            AudioSourceCollision.PlayOneShot(soundCollision);
            // Destroy(collision.gameObject);
        }
    }
}
