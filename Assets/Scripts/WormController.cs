using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WormController : MonoBehaviour
{
    [SerializeField] int playerNumber = 0;
    [SerializeField] GameObject wormPart;
    [SerializeField] Sprite wormPartSprite;
    [SerializeField] Sprite wormPartEndSprite;
    [SerializeField] int partCount = 10;
    [SerializeField] float speed = 1;
    [SerializeField] float rotSpeed = 1;
    [SerializeField] float maxDistance = 0.25f;
    [SerializeField] Vector2 xBound;
    [SerializeField] Vector2 yBound;
    [SerializeField] int startingHP = 100;
    [SerializeField] Color wormColor = Color.red;
    [SerializeField] SpriteRenderer shieldRenderer;
    [SerializeField] AnimationCurve wormSize;
    [SerializeField] ParticleSystem bloodEffect;

    public int score = 0;
    public event Action<WormController> DeathEvent;
    public event Action<WormController, int> ScoreChangeEvent;

    List<GameObject> tail;
    float maxDistanceSqr;
    float spawnTime;
    int currentHP = 0;
    float speedModifier = 1;
    float reflectTimer = 0f;
    bool shieldActive = false;
    bool isDead = false;

    private void Awake()
    {
        tail = new List<GameObject>();
        maxDistanceSqr = maxDistance * maxDistance;
        currentHP = startingHP;
    }

    private void Update()
    {
        reflectTimer = Mathf.Max(0, reflectTimer -= Time.deltaTime);
        UpdateWorm();
        BounceFromWall();
    }

    void Start()
    {
        spawnTime = Time.time;
        GetComponent<SpriteRenderer>().color = wormColor;

        for(int i = 0; i < partCount; i++)
        {
            GameObject newPart = Instantiate(wormPart, transform.position, Quaternion.identity);
            SpriteRenderer newRenderer = newPart.GetComponent<SpriteRenderer>();
            Color newColor = wormColor * Mathf.Pow(1.05f, i + 1);
            newColor.a = 1f;
            newRenderer.sprite = i == partCount - 1 ? wormPartEndSprite : wormPartSprite;
            newRenderer.sortingOrder -= i;
            newRenderer.color = newColor;
            newPart.transform.up = -(transform.position - newPart.transform.position);
            tail.Add(newPart);
        }
    }

    public void Move(int input)
    {
        AlignParts();

        Vector3 currenRot = transform.rotation.eulerAngles;
        currenRot.z += input * rotSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(currenRot);
        transform.position += transform.up * speed * speedModifier * Time.deltaTime;
    }

    public void TakeDamage(int damage)
    {
        if (shieldActive || isDead) return;

        currentHP -= damage;

        if (currentHP <= 0)
        {
            isDead = true;
            bloodEffect.Play();
            DeathEvent?.Invoke(this);
            Destroy(gameObject);
        }
    }

    public void ModifySpeed(float modifier, float duration)
    {
        StartCoroutine(ModifySpeedRoutine(modifier, duration));
    }

    public void ActivateShield(float duration)
    {
        StartCoroutine(ActivateShieldRoutine(duration));
    }

    void UpdateWorm()
    {
        int tailLength = tail.Count + 1;
        transform.localScale = Vector3.one * tailLength / 60 * wormSize.Evaluate(0);
        GetComponent<SpriteRenderer>().color = wormColor;

        for (int i = 0; i < tailLength - 1; i++)
        {
            tail[i].transform.localScale = Vector3.one * tailLength / 60 * wormSize.Evaluate(1.0f * (i + 1) / tailLength);
            SpriteRenderer newRenderer = tail[i].GetComponent<SpriteRenderer>();
            Color newColor = wormColor * Mathf.Pow(1.05f, i + 1);
            newColor.a = 1f;
            newRenderer.sprite = i == tailLength - 2 ? wormPartEndSprite : wormPartSprite;
            newRenderer.sortingOrder = 100 - i;
            newRenderer.color = newColor;
        }
    }

    void AlignParts()
    {

        Vector3 direction = transform.position - tail[0].transform.position;
        if(direction.sqrMagnitude > maxDistanceSqr * tail[0].transform.localScale.x)
        {
            tail[0].transform.position += direction.normalized * speed * speedModifier * Time.deltaTime;
            //tail[0].transform.LookAt(transform.position);
            tail[0].transform.up = -(transform.position - tail[0].transform.position);
        }

        for (int i = 1; i < tail.Count; i++)
        {
            direction = tail[i - 1].transform.position - tail[i].transform.position;
            float maxPartDistance = maxDistanceSqr * tail[i].transform.localScale.x;
            if (i == tail.Count - 1) maxPartDistance = maxPartDistance * 3;

            if (direction.sqrMagnitude > maxPartDistance)
            {
                tail[i].transform.position += direction.normalized * speed * speedModifier * Time.deltaTime;
                //tail[i].transform.LookAt(tail[i - 1].transform.position);
                tail[i].transform.up = -(tail[i - 1].transform.position - tail[i].transform.position);
            }
        }

    }

    void BounceFromWall()
    {
        if (reflectTimer > 0) return;

        float currentX = transform.position.x;
        float currentY = transform.position.y;

        Vector2 normal = Vector2.zero;

        if (currentX < xBound.x) normal = Vector2.right;
        if (currentX > xBound.y) normal = Vector2.left;
        if (currentY < yBound.x) normal = Vector2.down;
        if (currentY > yBound.y) normal = Vector2.up;

        if (normal.sqrMagnitude == 0) return;

        transform.up = Vector2.Reflect(transform.up, normal);
        //reflectTimer = 0.1f;
    }

    void OnTriggerEnter2D(Collider2D other){
        if (other.tag != "Resource")
        {
            /*
            if (other.tag != gameObject.tag)
            {
                //print(gameObject.name);
                TakeDamage(1000);
            }
            */
            
            return;
        }

        GameObject newPart = Instantiate(wormPart, tail[tail.Count - 1].transform.position, Quaternion.identity);
        newPart.transform.up = tail[tail.Count - 1].transform.up;
        tail.Add(newPart);

        AudioManager.Instance.PlaySound("Pickup");
        Destroy(other.gameObject);
        score += 1;
        ScoreChangeEvent?.Invoke(this, playerNumber);
    }

    IEnumerator ModifySpeedRoutine(float modifier, float duration)
    {
        speedModifier *= modifier;
        yield return new WaitForSeconds(duration);
        speedModifier /= modifier;
    }

    IEnumerator ActivateShieldRoutine(float duration)
    {
        shieldActive = true;
        shieldRenderer.enabled = true;
        yield return new WaitForSeconds(duration);
        shieldActive = false;
        shieldRenderer.enabled = false;
    }

}
