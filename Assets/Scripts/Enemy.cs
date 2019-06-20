using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Enemy : Car
{
    [SerializeField] private float targetRadius;
    [SerializeField] private float maxWanderingTime;
    [SerializeField] private EnemyType type;
    [SerializeField] private Material goldenMaterial;

    private GameObject floor;
    private Bounds floorArea;
    private Transform target;
    private NavMeshAgent agent;
    private List<GameObject> enemiesList;

    private float timer;
    private float time;
    private bool isDestinationSet;
    private bool goldStateActivated;

    public bool GoldState { get; set; }
    public bool HasGrantedCoins { get; set; }
    public EnemyType Type => type;

    // Start is called before the first frame update
    void Start()
    {
        GoldState = false;
        goldStateActivated = false;
        HasGrantedCoins = false;
        isDestinationSet = false;
        enemiesList = GameObject.FindGameObjectsWithTag("Car").Where(c => c.transform != transform).ToList();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = maxSpeed;

        floor = GameObject.FindGameObjectWithTag("Floor");
        Collider col = floor.GetComponent<Collider>();
        floorArea = new Bounds(col.bounds.center, new Vector3(col.bounds.extents.x * 2, 0, col.bounds.extents.z * 2));

        ResetTimers();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        SetTarget();
    }

    private void Update()
    {
        if (GoldState && !goldStateActivated)
        {
            goldStateActivated = true;
            transform.GetChild(0).GetChild(1).GetComponent<Renderer>().material = goldenMaterial;
        }
    }

    private void SetTarget()
    {
        if (target == null)
        {
            time += Time.deltaTime;

            if (!isDestinationSet)
            {
                agent.SetDestination(RandomPointInBounds(floorArea));
                isDestinationSet = true;
            }

            if (time >= timer)
            {
                isDestinationSet = false;
                target = SeekEnemy();
                ResetTimers();
            }
        }
        else
        {
            agent.SetDestination(target.position);
        }
    }

    private void ResetTimers()
    {
        time = 0;
        timer = Random.Range(0, maxWanderingTime);
    }

    private Transform SeekEnemy()
    {
        float distance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemiesList)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < distance)
            {
                distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance <= targetRadius)
                    nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null)
            return nearestEnemy.transform;
        else
            return null;
    }

    private Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            0,
            Random.Range(bounds.min.z, bounds.max.z)
            );
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Car")
        {
            /*
            Rigidbody rb = collision.transform.GetComponent<Rigidbody>();
            Vector3 force = transform.position - collision.transform.position;
            force.Normalize();

            rb.AddForce(force * speed * 200, ForceMode.Impulse);
            agent.velocity = rb.velocity; */
            target = null;
        }
    }
}
