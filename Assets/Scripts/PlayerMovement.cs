using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{

    private GameObject BgMusic;
    public LayerMask ground;
    Animator animation_player;
    public Rigidbody2D player;
    float speed = 7f;
    float jump = 17f;
    float direction;
    bool disable_inputs = false;
    public SpriteRenderer sprite;
    public BoxCollider2D player_collider;
    public AudioSource jumpSound;
    public AudioSource hitSound;
    // Start is called before the first frame update
    void Start()
    {
        animation_player = GetComponent<Animator>();
        player = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        player_collider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawRay(transform.position, Vector2.down*1, Color.green);
        IsGrounded();
        // Horizontal movement
        if (!disable_inputs && GameController.isPaused == false) {
            direction = Input.GetAxisRaw("Horizontal");
            PlayerMoving();
            //Jumping
            if (Input.GetKeyDown("space") && IsGrounded()) {
                jumpSound.Play();
                player.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
            }
            player.velocity = new Vector2(speed * direction, player.velocity.y);
        } 
        animation_player.SetInteger("velocity_y", (int)player.velocity.y);
    }

    void PlayerMoving() {
        // player moves hirizontally and sprite flips
        if (direction > 0f) {
            sprite.flipX = false;
            animation_player.SetBool("running", true);
        } else if (direction < 0f) {
            sprite.flipX = true;
            animation_player.SetBool("running", true);
        } else {
            animation_player.SetBool("running", false);
        }
    }

    bool IsGrounded() {
        // player is touching the ground, disable the jump
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2f, ground);
        if (hit.collider != null) {
            return true;
        }
        return false;

    }

    void OnCollisionEnter2D(Collision2D collision) {
        // player collides with enemy and gets hurt
        if (collision.gameObject.tag == "Enemies") {
            hitSound.Play();
            disable_inputs = true;
            player.velocity = new Vector2(0f, 0f);
            StartCoroutine(InputDisabler());
            Vector3 knockback_direction = (player.transform.position - collision.gameObject.transform.position);
            if (knockback_direction.x >= 0) {
                // player located on rght of enemy
                sprite.flipX = true;
                player.AddForce(new Vector2(1f, 0.7f) * 10f, ForceMode2D.Impulse);
            } else if (knockback_direction.x < 0) {
                // player located on left of enemy

                player.AddForce(new Vector2(-1f, 0.7f) * 10f, ForceMode2D.Impulse);
            }
            animation_player.SetTrigger("is_hit");
        }

    }

    void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.tag == "Module2" && Input.GetKey(KeyCode.J))
            SceneManager.LoadScene("GameSceneLevTwo");
        if (collision.gameObject.tag == "Module3" && Input.GetKey(KeyCode.J)) {
            GameController.currentSpawnPoint = new Vector3(121f, 29.93f, 0f);
            SceneManager.LoadScene("GameSceneLevThree");
        }
        if (collision.gameObject.tag == "EndGameModule" && Input.GetKey(KeyCode.J)) {
            BgMusic = GameObject.Find("BgMusic");
            Destroy(BgMusic);
            SceneManager.LoadScene("CreditsScene");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "DEATH") {
            speed = 7f;
            player.transform.localPosition = GameController.currentSpawnPoint;
        }

        if (collision.gameObject.tag == "PowerUp") {
            StopCoroutine("MoreSpeed");
            speed = 11f;
            StartCoroutine("MoreSpeed");
        }
    }

    IEnumerator InputDisabler() {
        // cant press anything
        while(!IsGrounded())
            yield return null; 
        disable_inputs = false;
        animation_player.SetTrigger("is_recovered");

    }

    IEnumerator MoreSpeed() {
        yield return new WaitForSeconds(4f); //wait 
        speed = 7f;
    }
}
