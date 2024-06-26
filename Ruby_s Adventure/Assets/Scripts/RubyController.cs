using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RubyController: MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip takeDmg;

    public int score = 0;
    public GameObject scoreText;

    public int maxHealth = 5;
    public int health { get { return currentHealth; } }
    int currentHealth;

    public GameObject healthIncreaseEffect;
    public GameObject healthDecreaseEffect;
    public GameObject projectilePrefab;
    public float speed = 4.0f;
    public float timeInvincible = 2.0f;
    public float force = 300.0f;

    bool gameOver;

    public GameObject gameOverText;
        
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    TextMeshProUGUI scoreText_text;
    TextMeshProUGUI gameOverText_text;

    bool isInvincible;
    float invincibleTimer;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        scoreText_text = scoreText.GetComponent<TextMeshProUGUI>();
        gameOverText_text = gameOverText.GetComponent<TextMeshProUGUI>();
        gameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);


        scoreText_text.text = "Fixed Robots: " + score.ToString();

        if (currentHealth < 1)
        {
            gameOver = true;
            gameOverText.SetActive(true);
            gameOverText_text.text = "You lost! Press R to Restart!";
            speed = 0.0000001f;
        }

        if (score > 1)
        {
            gameOver = true;
            gameOverText.SetActive(true);
            gameOverText_text.text = "You won! Press R to Restart!";
        }

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                if (hit.collider != null)
                {
                    NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                    if (character != null)
                    {
                        character.DisplayDialog();
                    }
                }
            }
        }
        if (Input.GetKey(KeyCode.R))
        {
            if (gameOver == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;
        

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            GameObject healthDecreaseObject = Instantiate(healthDecreaseEffect, rigidbody2d.position + Vector2.up * 0f, Quaternion.identity);
            if (isInvincible)
                return;
            audioSource.PlayOneShot(takeDmg);

            isInvincible = true;
            invincibleTimer = timeInvincible;
        }

        if (amount > 0)
        {
            GameObject healthIncreaseObject = Instantiate(healthIncreaseEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }
    public void ChangeScore(int scoreAmount)
    {
        score = score + scoreAmount;
       
    }
    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");


    }
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
