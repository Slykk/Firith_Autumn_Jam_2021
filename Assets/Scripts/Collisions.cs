using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collisions : MonoBehaviour
{

    [Header("Layers")]
    public LayerMask groundLayer;

    

    public bool isOnGround;
    public bool isOnWall;


    [Header("Collisions")]
    public float collisionRadius = 0.25f;
    public Vector2 groundOffset, wallOffset; //offsets for the main character, wall for the walls and the ground one for the ground
    private Color debugCollisionColor = Color.red;

    void Start()
    {

    }

    void Update()
    {
        isOnGround = Physics2D.OverlapCircle((Vector2)transform.position + groundOffset, collisionRadius, groundLayer);
        isOnWall = Physics2D.OverlapCircle((Vector2)transform.position + wallOffset, collisionRadius, groundLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { groundOffset, wallOffset };

        Gizmos.DrawWireSphere((Vector2)transform.position + groundOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + wallOffset, collisionRadius);
    }
}