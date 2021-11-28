using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public static event System.Action OnGuardSpottedPlayer;
    public Transform pathHolder;
    public float speed;
    public float waitTime = 0.3f;
    public float turnSpeed = 90;
    public float timeToSpotPlayer = 0.15f;

    public Light spotLight;
    public float viewDistance;
    public LayerMask viewMask;

    Transform player;
    float viewAngle;
    Color originalColor;
    float playerVisibleTimer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotLight.spotAngle;
        originalColor = spotLight.color;

        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }
        StartCoroutine(FollowPath(waypoints));
    }

    private void Update()
    {
        if (CanSeePlayer())
        {
            playerVisibleTimer += Time.deltaTime;
        }
        else
        {
            playerVisibleTimer -= Time.deltaTime;
        }
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
        spotLight.color = Color.Lerp(originalColor, Color.red, playerVisibleTimer / timeToSpotPlayer);
        
        if (playerVisibleTimer >= timeToSpotPlayer)
        {
            if (OnGuardSpottedPlayer != null)
            {
                OnGuardSpottedPlayer();
            }
        }
    }

    bool CanSeePlayer()
    {
        if(Vector3.Distance(player.position,transform.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if(angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, player.position, viewMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];//To default the position of the guard in first waypoint
        int targetWaypointIndex = 1;

        Vector3 targetWayPoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWayPoint);

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWayPoint, speed * Time.deltaTime);
            if(transform.position == waypoints[targetWaypointIndex])
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWayPoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWayPoint));
            }
            yield return null;
        }
    }

    IEnumerator TurnToFace(Vector3 target)
    {
        Vector3 directionToTarget = (target - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(directionToTarget.z, directionToTarget.x) * Mathf.Rad2Deg;

        while(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y,targetAngle)) > 0.05f)//We are using absolute value of delta angle caz if we need to make a anti clock wise turn...Then delta angle is negative(i.e transform.eulerAngles.y must be smaller)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 currentPosition = startPosition;
        foreach (Transform path in pathHolder)
        {
            Gizmos.DrawSphere(path.position, .3f);
            Gizmos.DrawLine(currentPosition, path.position);
            currentPosition = path.position;
        }
        Gizmos.DrawLine(currentPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}
