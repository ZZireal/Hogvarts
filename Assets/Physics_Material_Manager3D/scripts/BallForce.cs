using UnityEngine;
using System.Collections;

public class BallForce : MonoBehaviour 
{
	public float forceToAdd = 5.0f;

	private Rigidbody rg;
	private Transform ballTransfrom;

	void Start () 
	{
	rg = GetComponent<Rigidbody>();
	ballTransfrom = GetComponent<Transform>();
	rg.velocity = Random.insideUnitSphere * forceToAdd;
	}

	void OnCollisionEnter (Collision other)
	{
		if (other.collider.tag == "WallsAndFloors")
			{
			Vector3 localHitPoint = other.contacts[0].point - ballTransfrom.position;
			Vector3 newHitForce = (ballTransfrom.position - other.contacts[0].point).normalized * forceToAdd; 

			rg.AddForceAtPosition(newHitForce,localHitPoint,ForceMode.Impulse);
			}
	} 
}