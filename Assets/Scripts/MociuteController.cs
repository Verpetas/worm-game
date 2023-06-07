using System.Collections;
using UnityEngine;

public class MociuteController : MonoBehaviour
{
    [SerializeField] float speed = 1;
    [SerializeField] AnimationCurve nadeChance;
    [SerializeField] float scoreLimit;
    [SerializeField] Vector2 stateTimeBounds;
    [SerializeField] Vector2 mapBounds;
    [SerializeField] GameObject nade;
    [SerializeField] GameObject seed;

    enum MociuteState { goingLeft, goingRight, idling, seeding, throwingNade };
    SpriteRenderer spriteRenderer;
    Animator animator;
    float time;
    MociuteState mociuteState = MociuteState.idling;
    bool locked = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        time = 0;

    }

    void Update()
    {
        //HandleInput();

        if(time > 0)
        {
            time -= Time.deltaTime;
        }
        else
        {
            time = Random.Range(stateTimeBounds.x, stateTimeBounds.y);

            if (Random.value < nadeChance.Evaluate(1.0f * GameManager.Instance.maxScore / scoreLimit))
            {
                mociuteState = MociuteState.throwingNade;
            }
            else
            {
                mociuteState = (MociuteState)Random.Range(0, 4);
            }

            if (mociuteState == MociuteState.throwingNade)
            {
                AudioManager.Instance.PlaySound("AngryGran");
            }
        }

        HandleState();
    }

    void HandleState()
    {
        if (locked) return;

        switch (mociuteState)
        {
            case MociuteState.idling:
                //animator.SetBool("isWalking", false);
                break;
            case MociuteState.goingLeft:
                HandleMovement(1);
                break;
            case MociuteState.goingRight:
                HandleMovement(-1);
                break;
            case MociuteState.throwingNade:
                StartCoroutine("ThrowNade");
                locked = true;
                break;
            case MociuteState.seeding:
                StartCoroutine("ThrowSeeds");
                locked = true;
                break;
            default:
                Debug.LogError("No state found!");
                break;
        }

        animator.SetInteger("state", (int)mociuteState);
    }

    IEnumerator ThrowNade()
    {
        yield return new WaitForSeconds(1f);
        Instantiate(nade, transform.position, Quaternion.identity);
        locked = false;
    }

    IEnumerator ThrowSeeds()
    {
        yield return new WaitForSeconds(1f);
        Instantiate(seed, transform.position, Quaternion.identity);
        locked = false;
    }

    void HandleMovement(int goingLeft)
    {
        if(transform.position.x < mapBounds.x)
        {
            mociuteState = MociuteState.goingRight;
        }
        else if (transform.position.x > mapBounds.y)
        {
            mociuteState = MociuteState.goingLeft;
        }

        transform.position -= goingLeft * transform.right * speed * Time.deltaTime;
        spriteRenderer.flipX = goingLeft!=1;
    }
}
