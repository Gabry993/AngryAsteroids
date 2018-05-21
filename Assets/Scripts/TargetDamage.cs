using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDamage : MonoBehaviour {

    public int hitPoints;
    public Sprite damagedSprite;
    public float damageImpactSpeed;

    private int currentHitPoints;
    private float damageImpactSpeedSqr;
    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        damageImpactSpeedSqr = damageImpactSpeed * damageImpactSpeed;
        currentHitPoints = hitPoints;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag != "Damager")
            return;
        if (collision.relativeVelocity.sqrMagnitude < damageImpactSpeedSqr)
            return;

        spriteRenderer.sprite = damagedSprite;
        currentHitPoints--;

        if (currentHitPoints <= 0)
            Kill();
    }
    // Update is called once per frame
    void Update () {
		
	}

    void Kill()
    {
        spriteRenderer.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<ParticleSystem>().Play();

    }
}
