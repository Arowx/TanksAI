using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace Complete
{
    public class TankAITH : TankAI
    {
        enum TankAITHState
        {
            None,
            Search,
            Orbit,
            Fight
        }

        TankAITHState currentState;
        float distancteToTarget = -1;

        float[] distanceToTime = new float[11] { 0.01f, 0.08f, 0.15f, 0.22f, 0.29f, 0.36f, 0.43f, 0.5f, 0.62f, 0.69f, 0.75f };

        List<Vector3> path = new List<Vector3>() { new Vector3(25.0f, 0f, 25.0f), new Vector3(-25.0f, 0f, 25.0f), new Vector3(-25.0f, 0f, -20.0f), new Vector3(25.0f, 0f, -25.0f) };
        int currentPathIdx = -1;

        float sensorMaxDistance;
        float timeStarted;
        float reloadingTime;

        float nextAngle = 0;
        float lastRemainingDistance;

        NavMeshAgent navMeshAgent;

        protected override void SetName()
        {
            tankName = "THoeppner";
        }

        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            Assert.IsNotNull(navMeshAgent, "NavMeshAgent isn't set");
            navMeshAgent.stoppingDistance = 1.0f;
            sensorMaxDistance = tankSensor.GetComponent<SphereCollider>().radius;
            tankShooter.m_FireButton = false;
            currentState = TankAITHState.Search;
        }


        // Your TankAI should mainly be controlled from the Update() function

        // Use the tankNav, tankMover and tankShooter to manouver and battle

        // tankShooter.m_Fire - true power up / false to shoot

        void Update()
        {
            distancteToTarget = -1;
            target = tankSensor.GetNearestEnemy();
            if (target)
            {
                distancteToTarget = (target.position - transform.position).magnitude;
                if (tankSensor.LOS(target))
                {
                    currentState = TankAITHState.Fight;
                }
                else
                {
                    if (currentState != TankAITHState.Orbit)
                    {
                        tankNav.Stop();
                        navMeshAgent.ResetPath();
                    }
                    currentState = TankAITHState.Orbit;
                }
            }
            else
            {
                if (currentState != TankAITHState.Search)
                {
                    tankNav.Stop();
                    navMeshAgent.ResetPath();
                    currentPathIdx = -1;
                }
                currentState = TankAITHState.Search;
            }

            switch (currentState)
            {
                case (TankAITHState.Fight):
                    FightEnemy();
                    break;

                case (TankAITHState.Orbit):
                    OrbitEnemy(target.position);
                    break;

                case (TankAITHState.Search):
                    SearchEnemy();
                    break;
            }
        }

        private void SearchEnemy()
        {
            tankMover.m_TurnInputValue = 0f;

            if (navMeshAgent.hasPath)
            {   // Already a path set
                return;
            }

            if (currentPathIdx < 0)
            {
                float distance = float.MaxValue;
                for (int idx = 0; idx < path.Count; idx++)
                {
                    if ((path[idx] - transform.position).sqrMagnitude < distance)
                    {
                        currentPathIdx = idx;
                    }
                }
            }

            Vector3 moveTo = path[currentPathIdx];

            tankNav.SetDestination(moveTo);
            currentPathIdx++;
            currentPathIdx = currentPathIdx % path.Count;
        }

        private void FightEnemy()
        {
            tankNav.Stop();
            navMeshAgent.ResetPath();

            if (reloadingTime > Time.time)
            {
                return;
            }

            // Get the vector to the enemy
            Vector3 relative = transform.InverseTransformPoint(target.position);
            float a = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;

            if (a > 1f)
            {   // Vector large than 1 degree
                tankMover.m_TurnInputValue = 1f;
            }
            else if (a < -1f)
            {   // Vector large than 1 degree
                tankMover.m_TurnInputValue = -1f;
            }
            else
            {   // Fire
                tankMover.m_TurnInputValue = 0f;
                if (!tankShooter.m_FireButton)
                {
                    timeStarted = Time.time;
                }
                tankShooter.m_FireButton = true;
            }

            if (tankShooter.m_FireButton)
            {   // Check how long we have to hold down the fire button
                int idx = (int)((distancteToTarget / sensorMaxDistance) * distanceToTime.Length) - 1;
                idx = Mathf.Clamp(idx, 0, distanceToTime.Length - 1);

                if (Time.time > (timeStarted + distanceToTime[idx]))
                {   // Release the button and wait till we have reloded
                    tankShooter.m_FireButton = false;
                    reloadingTime = Time.time + 0.5f;
                }
            }
        }

        void OrbitEnemy(Vector3 enemyPos)
        {
            tankMover.m_TurnInputValue = 0f;

            if (navMeshAgent.hasPath)
            {   // Already a path set
                if (Mathf.Approximately(tankNav.remainingDistance, lastRemainingDistance))
                {
                    nextAngle += 10;
                }
                else
                {
                    lastRemainingDistance = tankNav.remainingDistance;
                    return;
                }
            }

            Vector3 to = transform.position - enemyPos;
            float distance = distancteToTarget - 1;
            float angle = Vector3.Angle(to, Vector3.right);
            if (to.z < 0) { angle = 360 - angle; }
            angle = (angle - nextAngle) * Mathf.Deg2Rad;

            Vector3 moveTo = enemyPos + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * distance;

            if (tankNav.SetDestination(moveTo))
            {
                nextAngle = 10;
            }
            lastRemainingDistance = tankNav.remainingDistance;
        }
    }

}
