using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
   
    AudioSource audioSource;
    public AudioClip throwClip;



    // Start is called before the first frame update
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
        audioSource.PlayOneShot(throwClip);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        {
            EnemyController e = other.collider.GetComponent<EnemyController>();
            if (e != null)
            {
                e.Fix();
            }

            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (transform.position.magnitude > 1000.0f)
        {
            Destroy(gameObject);
        }
    }
}
