using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    [Tooltip("Health"), Range(1f, 100f)]
    [SerializeField] private float health = 100f;

    [Header("Movement")]
    [Tooltip("AI movement speed"), Range(0f, 20f)]
    [SerializeField] private float speed = 10f;

    [Tooltip("The rotation speed during movement")]
    public float rotationSpeed = 20f; //Rotation during movement modifier. If AI starts spinning at random, increase this value. (First check to make sure it's not due to visual radius limitations)

    [Header("Attack")]
    [Tooltip("The Distance below the AI will attack"), Range(0.001f, 20f)]
    [SerializeField] public float attackRange = 20.0f;

    [Tooltip("The speed of attacks in seconds"), Range(0.01f, 5f)]
    [SerializeField] public float attackSpeed = 20.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveTo(Vector3 destination)
    {
        Vector3 direction = destination - transform.position;
        direction = direction.normalized * speed * Time.deltaTime;

        float distance = Vector3.Distance(transform.position, destination);

        if (distance > 1f || distance < -1f)
        {
            transform.Translate(direction);
        }
    }

    public void MoveTowads(Vector3 direction)
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);
    }

    internal void Attack(Transform target)
    {
        Debug.Log("Attack");
        // TODO
    }
}
