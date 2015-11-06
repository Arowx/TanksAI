using UnityEngine;
using System.Collections;
using System;

namespace Complete
{
    public class TankAI : MonoBehaviour {

		[HideInInspector]
        public string tankName = "<Insert Name>";

        protected TankMovement tankMover;
        protected TankNavigation tankNav;
        protected TankSensor tankSensor;
        protected TankShooting tankShooter;
        protected TankHealth tankHealth;

        protected Transform target;

        protected Vector3 moveTo;
        
        void Awake() {
            tankMover = GetComponent<TankMovement>();
            tankNav = GetComponent<TankNavigation>();
            tankSensor = GetComponentInChildren<TankSensor>();
            tankShooter = GetComponent<TankShooting>();
            tankHealth = GetComponent<TankHealth>();

            Reset();
        }

        // Needed to reset the settings of subsystem between battles

        public void Reset()
        {
            target = null;
            moveTo.y = -2f;

            if (tankShooter) tankShooter.Reset();
            if (tankSensor) tankSensor.Reset();
        }

        // Your TankAI should mainly be controlled from the Update() function

        // Use the tankNav, tankMover and tankShooter to manouver and battle

        // tankShooter.m_Fire - true power up / false to shoot

        protected bool RandomMove(out Vector3 result)
        {
            float range = 40.0f;
            Vector3 center = Vector3.zero;


            for (int i = 0; i < 30; i++)
            {
                Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;

                randomPoint.y = 0.5f;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 2.0f, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
        }
        
    }
}
