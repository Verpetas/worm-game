using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NadeController : MonoBehaviour
{
    [SerializeField] AnimationCurve trajectory;
    [SerializeField] Vector2 mapBoundsX;
    [SerializeField] Vector2 mapBoundsY;
    [SerializeField] float speed;
    [SerializeField] float rotSpeed = 100;
    [SerializeField] GameObject targetGizmo;
    [SerializeField] float explosionRadius = 0.5f;
    [SerializeField] float maxDamage = 100;
    [SerializeField] bool spawnSeed = false;

    Vector2 targetPos;
    Vector2 startPos;
    float travelTime = 0;
    new ParticleSystem particleSystem;
    SpriteRenderer spriteRenderer;
    bool exploaded = false;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        targetPos = new Vector2(
            Random.Range(mapBoundsX.x, mapBoundsX.y),
            Random.Range(mapBoundsY.x, mapBoundsY.y)
        );

        startPos = transform.position;

        if (targetGizmo != null)
            Destroy(Instantiate(targetGizmo, targetPos, Quaternion.identity), 1.2f);
    }

    private void Update()
    {
        if(travelTime > 1.2f && !exploaded)
        {
            particleSystem.Play();
            if (spawnSeed) ResourceHandler.Instance.Root(targetPos);
            AudioManager.Instance.PlaySound("Explosion");
            spriteRenderer.enabled = false;
            Destroy(gameObject, 0.5f);
            exploaded = true;

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

            foreach(Collider2D hit in hits)
            {
                if (hit.tag == "Player1" || hit.tag == "Player2")
                {
                    WormController controller = hit.GetComponent<WormController>();
                    if (controller == null) continue;

                    float distance = (transform.position - hit.transform.position).magnitude;
                    float distanceRatio = distance / explosionRadius;

                    hit.GetComponent<WormController>().TakeDamage((int)(distanceRatio * maxDamage));
                }
            }
        }

        travelTime += Time.deltaTime * speed;

        Vector2 currentPos = Vector2.Lerp(startPos, targetPos, travelTime);
        transform.position = currentPos + Vector2.up * trajectory.Evaluate(travelTime);

        transform.RotateAround(transform.position, Vector3.forward, Time.deltaTime * rotSpeed);
    }
}
