using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("General")]

    [Tooltip("Enable AI")]
    [SerializeField] private bool on = true;
    [SerializeField] private EnemyAIController aiController;
    [SerializeField] private LayerMask layerMask;

    [Header("Movement")]

    [Tooltip("Distance until target can be seen. 0=disabled"), Range(0f, 100f)]
    [SerializeField] private float visionDistance = 100.0f;

    [Tooltip("If the AI should keep a distance to the target")]
    [SerializeField] private bool keepDistance = false;

    [Tooltip("If runAway==true, the distance to the target the AI tries to keep"), Range(0f, 50f)]
    [SerializeField] private float keepDistanceAmount = 100.0f;

    [Header("Patrole")]

    [Tooltip("Patrole along waypoints")]
    [SerializeField] private bool enablePatrole = false; //If true, the AI will make use of the waypoints assigned to it until over-ridden by another functionality.

    [Tooltip("Array of Waypoints")]
    [SerializeField] private Transform[] waypoints;

    [Tooltip("Patrole along waypoints backwards")]
    [SerializeField] private bool reversePatrol = true; //if true, patrol units will walk forward and backward along their patrol.

    [Header("Target")]

    [Tooltip("Search for player time in seconds"), Range(0.1f, 10f)]
    [SerializeField] private float huntingTimer = 5.0f;

    [Tooltip("The target the AI is looking for")]
    [SerializeField] private Transform target; //The target, or whatever the AI is looking for.

    //private script handled variables
    private bool initialGo = false; //AI cannot function until it is initialized. //An enhancement to how the AI functions prior to visibly seeing the target. Brings AI to life when target is close, but not visible.

    // Patrole
    private bool patrolBackwards = false; //used to determine if we're moving forward or backward through the waypoints.
    private int curPatroleWayPoint = 0; //determines what waypoint we are heading toward.

    // Hunting
    private Vector3 lastVisTargetPos;
    private bool playerHasBeenSeen = false;
    private bool huntingStarted = false;
    private float lostPlayerTimer;

    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;

        initialGo = true;
    }

    void Update()
    {
        if (!on || !initialGo)
        {
            return;
        }
        else
        {
            AIFunctionality();
        }
    }

    void AIFunctionality()
    {
        // no target but target required -> disable AI
        if ((!target && !enablePatrole) || !on)
        {
            return;
        }

        //Functionality Updates
        lastVisTargetPos = target.position;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        Vector3 moveAway = (transform.position - lastVisTargetPos).normalized * keepDistanceAmount;

        Debug.Log("Distance1=" + distanceToTarget);

        // target found
        if (TargetIsInSight())
        {
            // move away from Player
            if (keepDistance && distanceToTarget < keepDistanceAmount)
            {
                MoveTo(moveAway);
            }

            // target is too far away to attack -> move closer
            else if (distanceToTarget > aiController.attackRange)
            {
                MoveTo(lastVisTargetPos);
            }

            // atttack if close enough
            if (distanceToTarget <= aiController.attackRange)
            {
                // if (Time.time > lastShotFired + attackTime)
                aiController.Attack(target);
            }
        }

        // search target if AI lost player
        else if (playerHasBeenSeen && !huntingStarted)
        {
            lostPlayerTimer = Time.time + huntingTimer;
            StartCoroutine(HuntDownTarget(lastVisTargetPos));
        }

        // patrol
        else if (enablePatrole)
        {
            Patrol();
        }
    }

    // standard movement behaviour
    private void MoveTo(Vector3 destination)
    {
        float buffer = 1f;
        float distance = Vector3.Distance(transform.position, destination);

        if ((!keepDistance && distance > buffer) || (keepDistance && (distance - buffer) > keepDistanceAmount))
        {
            aiController.MoveTo(destination);
        }
    }

    // verify AI can see the target
    private bool TargetIsInSight()
    {
        Debug.Log("Distance2=" + Vector3.Distance(transform.position, target.position));

        // check if target is in vision range
        if (visionDistance == 0 || Vector3.Distance(transform.position, target.position) > visionDistance)
        {
            return false;
        }

        // check nothing is blocking the line of sight
        RaycastHit sight;
        bool visionObstructed = Physics.Linecast(transform.position, target.position, out sight, layerMask);

        if (!visionObstructed)
        {
            Debug.Log("player found");
            playerHasBeenSeen = true;
            return true;
        }

        return false;

        if (Physics.Linecast(transform.position, target.position, out sight))
        {
            if (!playerHasBeenSeen && sight.transform == target)
            {
                playerHasBeenSeen = true;
            }

            return sight.transform == target;
        }

    }

    // target tracking
    // called if AI hast lost target -> AI moves to last known position
    private IEnumerator HuntDownTarget(Vector3 lastPosition)
    {
        huntingStarted = true;

        while (huntingStarted)
        {
            MoveTo(lastPosition - transform.position);

            // check if AI found target
            if (TargetIsInSight())
            {
                huntingStarted = false;
                break;
            }

            // check if AI should cancel search
            if (Time.time > lostPlayerTimer)
            {
                huntingStarted = false;
                playerHasBeenSeen = false;
                break;
            }

            yield return null;
        }

    }

    // move along waypoints
    void Patrol()
    {
        if (!enablePatrole)
        {
            return;
        }

        MoveTo(CurrentPath());

        float distance = Vector3.Distance(transform.position, CurrentPath());

        // next waypoint if close enough
        if (distance <= 1f)
        {
            NextWaypoint();
        }

    }

    // current waypoint
    Vector3 CurrentPath()
    {
        return waypoints[curPatroleWayPoint].position;
    }

    // next waypoint
    void NextWaypoint()
    {
        if (!patrolBackwards)
        {
            curPatroleWayPoint++;

            if (curPatroleWayPoint >= waypoints.GetLength(0))
            {
                // patrol backwards to first waypoint
                if (reversePatrol)
                {
                    patrolBackwards = true;
                    curPatroleWayPoint -= 2;

                }

                // go straight to first waypoint
                else
                {
                    curPatroleWayPoint = 0;
                }
            }
        }

        else if (reversePatrol)
        {
            curPatroleWayPoint--;

            if (curPatroleWayPoint < 0)
            {
                patrolBackwards = false;
                curPatroleWayPoint = 1;
            }
        }
    }
}
