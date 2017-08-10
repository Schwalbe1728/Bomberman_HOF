using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroundScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<NavMeshSurface> ().BuildNavMesh ();	
	}
}
