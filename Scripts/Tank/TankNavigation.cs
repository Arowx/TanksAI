using UnityEngine;
using System.Collections;

// Acts as a wrapper to the NavMeshAgent
public class TankNavigation : MonoBehaviour {
	
	private NavMeshAgent agent;
	private Transform destinationMarker;

	public Vector3 destination {
		get {
			return agent.destination;
		} set {
			SetDestination(value);
		}
	}

	public Vector3 velocity {
		get {
			return agent.velocity;
		} set {
			agent.velocity = value;
		}
	}

	public float remainingDistance {
		get {
			return agent.remainingDistance;
		}
	}

	void Awake() {
		agent = GetComponent<NavMeshAgent>();
		destinationMarker = transform.Find("moveToSphere");

		if (destinationMarker) destinationMarker.SetParent(null);
	}

	public bool SetDestination(Vector3 value) {
		if (agent.SetDestination(value)) {
			if (destinationMarker) destinationMarker.position = value;
			return true;
		} else {
			return false;
		}
	}

	public void Resume() {
		agent.Resume();
	}

	public void Stop() {
		agent.Stop();
	}

}
