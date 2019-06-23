using UnityEngine;
using System.Collections;

/**
 * Rotate towards an object, but using a child of the parent for the final look rotation
 * https://docs.unity3d.com/ScriptReference/Vector3.RotateTowards.html
 * 
 * Fires a bullet from the direction origin (not the barrel origin) once the parent reaches the destination rotation. This means the bullet is fired from the center of the parent, if you put all directions there.
 * TODO: bullet firing was quickly added. we'd need to calculate the end of the barrel. for our current purposes this is only a demo and it suffices to have the rotation and fire from the center of the parent.
 * TODO: needed quick periodic firing in case there's only 1 barrel. we should use a dedicated class for the timing of the bullet firing
 * 
 * The rotation direction is in direction of the z-axis, i. e. the blue arrow in unity
 * 
 * If there is only 1 barrel, then there's a fallback to fireing periodically. In that case fireIntervalSeconds takes effect.
 * 
 * Preconditions:
 * + Bullet must have a 
 *     - RigidBody: 
 *         Use Gravity: false
 *         Kinematic: false
 *         Collision Detection: Continuous (otherwise the fast laster bullets might fly through instead of colliding)
 *     - Collider (Capsule Collider)
 *       Material: Bouncy (to bounce off lightsaber and walls)
 *     
 */
public class RotateMultiTowardsAndFire : MonoBehaviour {

    [Tooltip("The target to which we rotate.")]
    public Transform target;

    [Tooltip("Speed at which to rotate towards target.")]
    public float rotateSpeed = 100;


    [Tooltip("The list of barrels which will be used.")]
    public GameObject[] direction;

    [Tooltip("The list of barrels which will be used.")]

    // Current direction array index, will be initialized in Start().
    // A value < 0 starts at 0 for direction iteration in sequence. For random direction the value doesn't matter.
    private int currentDirectionIndex = -1;

    // Random or Sequence direction determination. Determines the way the direction array is iterated
    private bool randomDirection = true;

    public AudioClip soundFireBullet;

    public AudioSource AudioSource;

    // the bullet
    public GameObject bulletPrefab;
    public float bulletForce = 100f;

    public bool moveAndFireEnabled = false;

    public float bulletLifeTime = 1.2f;


    // interval between firings in seconds
    // can be decimal if lower than seconds
    //
    // only used when periodic firing is enabled
    public float fireIntervalSeconds = 1f;

    // next possible firing time for periodic firing
    private float nextFireTime;

    private bool firePeriodically = false;

    void Start()
    {

        if( direction.Length == 0)
        {
            Debug.LogError("Direction mustn't be empty!");
        }

        NextDirectionIndex();

        if( direction.Length == 1)
        {
            firePeriodically = true;

            // set the next possible firing time
            nextFireTime = Time.time + fireIntervalSeconds;

        }

    }

    void Update () {

        // keyboard handler
        // toggle enabled state with keyboard
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleActivation();
        }

        if (!moveAndFireEnabled)
            return;

        Rotate();

    }

    public void ToggleActivation()
    {
        moveAndFireEnabled = !moveAndFireEnabled;
    }

    /**
     * Rotate parent towards target, but using the child for the rotation calculation
     * source: http://answers.unity3d.com/questions/715123/rotate-parent-to-aim-child.html
     */
    void Rotate()
    {

        GameObject child = direction[currentDirectionIndex];

        // relative rotation between parent and child
        Quaternion relativeRotation = Quaternion.Inverse(transform.rotation) * child.transform.rotation;

        // target direction from the child
        Vector3 targetDir = target.transform.position - child.transform.position;

        // look rotation
        Quaternion lookRotation = Quaternion.LookRotation(targetDir);

        // rotate parent towards child's target rotation, consider relative rotation between parent and child
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation * Quaternion.Inverse(relativeRotation), rotateSpeed * Time.deltaTime);

        if (firePeriodically)
        {
            // only shoot when there are bullets and it's time to shoot
            if (Time.time > nextFireTime)
            {
                FireBullet();

                // set the next possible firing time
                nextFireTime = Time.time + fireIntervalSeconds;
            }
        }
        else
        {
            // check if the rotation is close enough and if so get the next direction
            float angleDiff = Quaternion.Angle(child.transform.rotation, lookRotation);

            // get next direction
            if (angleDiff < 0.1f)
            {

                // fire bullet
                FireBullet();

                // next direction
                NextDirectionIndex();

            }
        }
    }

    private void NextDirectionIndex()
    {
        if (randomDirection)
        {
            NextRandomDirectionIndex();
        }
        else
        {
            NextSequenceDirectionIndex();
        }
    }

    private void NextSequenceDirectionIndex()
    {
        currentDirectionIndex++;

        if(currentDirectionIndex < 0 || currentDirectionIndex >= direction.Length)
        {
            currentDirectionIndex = 0;
        }

    }

    /**
     * Get the next random direction.
     * Ensure that it is different to the current one
     */
    private void NextRandomDirectionIndex()
    {
        // get next index
        int nextIndex = Random.Range(0, direction.Length);

        // ensure that the next index is different to the current one
        if( currentDirectionIndex == nextIndex)
        {
            nextIndex++;
        }

        if( nextIndex >= direction.Length)
        {
            nextIndex = 0;
        }

        currentDirectionIndex = nextIndex;

    }

    void FireBullet()
    {

        // play sound
        AudioSource.PlayOneShot(soundFireBullet);

        // fire bullet

        GameObject child = direction[currentDirectionIndex];

        // instantiate the bullet game object, align it to the barrel position and rotating object's rotation
        GameObject bullet = GameObject.Instantiate(bulletPrefab, child.transform.position, child.transform.rotation) as GameObject;

        // sometimes bullets may appear rotated incorrectly due to the way its pivot was set from the original modeling package.
        // this can be corrected here, you might have to rotate it from a different axis and or angle based on your particular mesh.
        bullet.transform.Rotate(Vector3.left * 90);

        // ignore collisions of the bullet with the turret/body/barrel
        // https://docs.unity3d.com/ScriptReference/Physics.IgnoreCollision.html
        if (child.GetComponent<Collider>() != null)
        {
            Physics.IgnoreCollision(child.GetComponent<Collider>(), bullet.GetComponent<Collider>());
        }
        if (child.GetComponent<Collider>() != null)
        {
            Physics.IgnoreCollision(child.GetComponent<Collider>(), bullet.GetComponent<Collider>());
        }

        // retrieve the Rigidbody component from the instantiated Bullet and control it.
        Rigidbody bulletRigidBody = bullet.GetComponent<Rigidbody>();

        // Tell the bullet to be "pushed" forward by an amount set by Bullet_Forward_Force.
        // This depends on the rotating object's transform, not on the bullet's transform!
        bulletRigidBody.AddForce(child.transform.forward * bulletForce);

        // Basic Clean Up, set the Bullets to self destruct after 3 Seconds
        Destroy(bullet, bulletLifeTime);

    }
}
