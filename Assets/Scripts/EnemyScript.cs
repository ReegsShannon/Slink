using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//patrolling - player hasn't been seen
//chasing - player currently in enemy vision range
//sawplayer - player not in vision range, but enemy still chasing
//playerslinked - player went into slink to escape enemy
public enum EnemyState
{
	Patrolling,
	Chasing,
	SawPlayer,
	PlayerSlinked
}
;

public class EnemyScript : MonoBehaviour
{

	public List<Vector3> patrolPoints = new List<Vector3> ();
	public GameObject[] allEnemies;
	public float waitTime = 2f;
	public bool chasingPlayer = false;
	public int nextDest = 0;
	public float PlayerSeenTimer = 2f;
	public float patrolTimer = 2f;
	public EnemyState curState = EnemyState.Patrolling;
	public bool moving = false;
	public float seenAngle; //angle enemy was facing when player slinked
	public bool turningLeft = false;
	public bool turningRight = false;
	public float rotationSpeed = 15f; //rotation speed in rpm
	GameObject player;
	NavMeshAgent navMesh;
	PlayerController playerScript;
	Vector3 originalPosition;
	bool waiting;
	float toDest;

	// Use this for initialization
	void Start ()
	{
		navMesh = GetComponent<NavMeshAgent> ();
		player = GameObject.Find ("Player");
		playerScript = player.GetComponent<PlayerController> ();
		allEnemies = GameObject.FindGameObjectsWithTag ("Enemy");

		originalPosition = transform.position;
		navMesh.SetDestination (originalPosition);

		//StartCoroutine (Patrol ());
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (curState == EnemyState.Patrolling/*!waiting && !chasingPlayer*/) {
			if (moving) {
				toDest = Vector3.Distance (transform.position, navMesh.destination);
				
				if (toDest < 0.5f) {
					//StartCoroutine (Patrol ());
					//start to keep track of time, after 2 seconds, move to next point
					moving = false;
					patrolTimer = waitTime;
				}
			} else {
				patrolTimer -= Time.deltaTime;
				if (patrolTimer <= 0) {
					GoToNextPoint ();
					moving = true;
				}
			}
		} else if (curState == EnemyState.PlayerSlinked) {
			if (turningLeft) {
				if(transform.eulerAngles.y <= seenAngle - 90) {
					turningRight = true;
					turningLeft = false;
				} else {
					transform.Rotate(0f,-6f*rotationSpeed*Time.deltaTime,0f);
				}
			} else if (turningRight) {
				if(transform.eulerAngles.y >= seenAngle + 90) {
					turningRight = false;
					curState = EnemyState.Patrolling;
				} else {
					transform.Rotate (0f,6f*rotationSpeed*Time.deltaTime,0f);
				}
			} else {
				turningLeft = true;
				seenAngle = transform.rotation.y;
			}
		} else {
			if (playerScript.isSlinking ()) {
				curState = EnemyState.PlayerSlinked;
				return;
			}
			if (curState == EnemyState.Chasing) {
				navMesh.speed = 8f;
			} else if (curState == EnemyState.SawPlayer) {
				navMesh.speed = 6f;
				PlayerSeenTimer -= Time.deltaTime;
				if (PlayerSeenTimer <= 0)
					curState = EnemyState.Patrolling;
			}

			navMesh.SetDestination (player.transform.position);
		}
	}

	IEnumerator Patrol ()
	{
		waiting = true;
		yield return new WaitForSeconds (waitTime);
		GoToNextPoint ();
		waiting = false;
	}

	void GoToNextPoint ()
	{

		if (patrolPoints.Count <= 1) {
			navMesh.SetDestination (originalPosition);
			return;
		}

		navMesh.SetDestination (patrolPoints [nextDest]);

		nextDest = (nextDest + 1) % patrolPoints.Count;
	}

	void OnTriggerEnter (Collider other)
	{

		if (other.tag != "Player")
			return;

		if (!playerScript.isSlinking ()) {
			curState = EnemyState.Chasing;
		}
	}

	void OnTriggerStay (Collider other)
	{
		if (other.tag != "Player")
			return;

		if (!playerScript.isSlinking ()) {
			curState = EnemyState.Chasing;
		} else {
			curState = EnemyState.PlayerSlinked;
		}
	}

	void OnTriggerExit (Collider other)
	{
		print ("exit trigger");
		if (other.tag == "Player") {
			curState = EnemyState.SawPlayer;
			PlayerSeenTimer = 2f;
		}
	}

	public void resetEnemy ()
	{
		transform.position = originalPosition;
		navMesh.SetDestination (originalPosition);
		curState = EnemyState.Patrolling;
		PlayerSeenTimer = 0f;
	}

	void OnCollisionEnter (Collision other)
	{
		if (other.gameObject.tag == "Player") {
			foreach (GameObject enemy in allEnemies) {
				if (enemy.name == "cone")
					continue;

				enemy.GetComponent<EnemyScript> ().resetEnemy ();
			}
			playerScript.playerCaught ();
		}
	}
}
