using UnityEngine;
using System.Collections;

public class PuckForce : MonoBehaviour 
{
	public float forceToAdd;

	private Rigidbody rg;
	private Transform puckTransfrom;

	void Start () 
	{
	rg = GetComponent<Rigidbody>();
	puckTransfrom = GetComponent<Transform>();
	rg.velocity = Random.insideUnitCircle * forceToAdd;
	}

	void OnCollisionEnter (Collision other)
	{
		if (other.collider.tag == "WallsAndFloors")
			{
			Vector3 localHitPoint = other.contacts[0].point - puckTransfrom.position;
			Vector3 newHitForce = (puckTransfrom.position - other.contacts[0].point).normalized * forceToAdd; 

			rg.AddForceAtPosition(newHitForce,localHitPoint,ForceMode.VelocityChange);
			}
	} 
}