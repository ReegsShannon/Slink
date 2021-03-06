﻿using UnityEngine;
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
	public Vector3 originalPosition;
	bool waiting;
	float toDest;
	float rotTraveled = 0;
	Quaternion startRot;

	// Use this for initialization
	void Start ()
	{
		navMesh = GetComponent<NavMeshAgent> ();
		player = GameObject.Find ("Player");
		playerScript = player.GetComponent<PlayerController> ();
		allEnemies = GameObject.FindGameObjectsWithTag ("Enemy");

		originalPosition = transform.position;
		startRot = transform.rotation;
		navMesh.SetDestination (originalPosition);

		//StartCoroutine (Patrol ());
	}
	
	// Update is called once per frame
	void Update (){
		if (curState == EnemyState.Patrolling/*!waiting && !chasingPlayer*/) {
			toDest = Vector3.Distance (transform.position, originalPosition);
			if(patrolPoints.Count <= 1 && toDest < .5f){
				transform.rotation = Quaternion.RotateTowards(transform.rotation, startRot, 8 * rotationSpeed * Time.deltaTime);
			}
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
				if (rotTraveled >= 90) {
					turningRight = true;
					turningLeft = false;
					seenAngle = transform.eulerAngles.y;
					rotTraveled = 0;
				} else {
					transform.Rotate (0f, -6f * rotationSpeed * Time.deltaTime, 0f);
					rotTraveled += 6f * rotationSpeed * Time.deltaTime;
				}
			} else if (turningRight) {
				if (rotTraveled >= 180) {
					turningRight = false;
					curState = EnemyState.Patrolling;
					rotTraveled = 0;
				} else {
					transform.Rotate (0f, 6f * rotationSpeed * Time.deltaTime, 0f);
					rotTraveled += 6f * rotationSpeed * Time.deltaTime;
				}
			} else {
				turningLeft = true;
				seenAngle = transform.eulerAngles.y;
				rotTraveled = 0;
			}
		} else if (curState == EnemyState.Chasing) {
			navMesh.speed = 8f;
			if(playerScript.isSlinking()){
				curState = EnemyState.PlayerSlinked;
			}
			else{
				navMesh.SetDestination (player.transform.position);
			}
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

	void OnTriggerStay (Collider other)
	{
		if (other.tag != "Player")
			return;

		if (!playerScript.isSlinking ()) {
			curState = EnemyState.Chasing;
		} 
	}

	void OnTriggerExit (Collider other)
	{
		//if (other.tag == "Player") {
		//	curState = EnemyState.SawPlayer;
		//	PlayerSeenTimer = 2f;
		//}
	}

	public void resetEnemy ()
	{
		transform.position = originalPosition;
		transform.rotation = startRot;
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
