using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Declaring public variables at same place for clean and easy to read code.
    public Vector2 rayOrigion;
    public float rayLength;
    public LayerMask rayCastLayers;
    [HideInInspector]
    public int rawVelocityX = 0;
    public GameObject playerSprite;

    //Declaring private variables at same place for clean and easy to read code.
    private float speed;
    private float jumpHeight;
    private Rigidbody2D rb;
    private SpriteRenderer spr;
    [HideInInspector]
    public Animator anim;
    private float horizontalDirection;
    private float boundingBoxSizeX;

    //Start is called only once when scene loads up.
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spr = playerSprite.GetComponent<SpriteRenderer>();
        anim = playerSprite.GetComponent<Animator>();
        boundingBoxSizeX = GetComponent<CapsuleCollider2D>().size.x;
    }

    //Update is called every frame by unity game engine.
    private void Update()
    {
        //Calling function inside of update, so it is executed every frame.
        Movement();
        characterAnimation();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //check if player hit coin.
        if(collision.tag == "coin")
        {
            collision.gameObject.SetActive(false);
            GameManager.instance.coinCount++;
        }
    }

    //Custom function to add movement to our character.
    private void Movement()
    {
        speed = GameManager.instance.speed;
        jumpHeight = GameManager.instance.jumpHeight;
        horizontalDirection = GameManager.instance.inputDir;
        
        //this is just the direction of velocity in x direction. it outputs only -1, 0 & 1 depending on if player is moving or stationary; 
        rawVelocityX = Physics2D.Raycast(transform.position, Vector2.right * horizontalDirection, boundingBoxSizeX, rayCastLayers)? 0 : 1; //this called the ternary operator.
                                                                                                                                             //it is a short hand way of writing if/else statement using
                                                                                                                                             // '? :' notation.
        Debug.DrawRay(transform.position, Vector2.right * horizontalDirection * boundingBoxSizeX, Color.red);

        //Check to see if we are hitting the ground.
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + rayOrigion.x, transform.position.y + rayOrigion.y), Vector2.down, rayLength, rayCastLayers);
        
        //Draw a ray for our visual refrence. Only seen in editor.
        Debug.DrawRay(new Vector2(transform.position.x + rayOrigion.x, transform.position.y + rayOrigion.y), rayLength * Vector2.down, Color.red);

        //Add jump velocity when player hits jump key and is on ground.
        if (Input.GetKeyDown(KeyCode.Space) & hit)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, jumpHeight));
        }

        //Add linear velocity to character when player hits direction keys. Key input is taken by GameManager.
        rb.velocity = new Vector2(speed * Time.deltaTime * 100 * horizontalDirection, rb.velocity.y);


    }

    //Custom function to add animation to our character. You will notice few code lines are exact copy from 'Movement()' function. We could have called put this code in 'Movement()' function
    //but i keep it seperate for clear and easy to understand code. 'Movement()' only does movement and 'characterAnimation()' only does animation.
    private void characterAnimation()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + rayOrigion.x, transform.position.y + rayOrigion.y), Vector2.down, rayLength, rayCastLayers);

        //Flip the character based on direction of movement.
        if (Input.GetAxis("Horizontal") > 0) 
        {
            spr.flipX = false;
        }
        if (Input.GetAxis("Horizontal") < 0) 
        {
            spr.flipX = true;
        }

        //Setting variables we set in animator (ctrl + 6) window. We have to build a animation transition tree in animator window.
        anim.SetFloat("speedX", Math.Abs(rb.velocity.x));
        anim.SetBool("grounded", hit);

        if (GameManager.instance.playerHealth < 1)
        {
            if(!anim.GetBool("dead"))
                StartCoroutine(GameManager.instance.GameOverMenu(1));
            anim.SetBool("dead", true);
        }
    }
}
