using UnityEngine;
using System.Collections;
using System;

namespace Complete
{
    public class TankAI : MonoBehaviour {

        [HideInInspector]
        public string tankName = "Arowx";

        public TankMovement tankMover;
        public NavMeshAgent tankNav;
        public TankSensor tankSensor;
        public TankShooting tankShooter;
        public TankHealth tankHealth;

        public Transform target;

        protected Vector3 moveTo;

        public Transform moveToMarker;

        
        public void Start() {            
            tankMover = GetComponent<TankMovement>();
            tankNav = GetComponent<NavMeshAgent>();            
            tankSensor = GetComponentInChildren<TankSensor>();
            tankShooter = GetComponent<TankShooting>();
            tankHealth = GetComponent<TankHealth>();

            moveToMarker.SetParent(null);

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

        void Update() {
            //tankMover.m_MovementInputValue = Random.Range(-1f, 1f);
            //tankMover.m_TurnInputValue = Random.Range(-1f, 1f);

            tankShooter.m_FireButton = false;

            Vector3 pos = transform.position;
            

            if (target)
            {

                Vector3 enemyPos = target.position;

                float sqrRange = (pos - enemyPos).sqrMagnitude;

                if (sqrRange > 144f)
                {
                    if ((moveTo - enemyPos).sqrMagnitude > 4f)
                    {
                        moveTo = enemyPos;

                        if ((pos - moveTo).sqrMagnitude > 16f)
                        {
                            if (!tankNav.SetDestination(moveTo))
                            {
                                moveTo.y = -2f;
                            }
                            else
                            {
                                moveToMarker.position = moveTo;
                            }
                        }
                    }

                    if (tankNav.velocity.sqrMagnitude < 1f)
                    {
                        tankNav.Resume();
                    }
                }
                else if (tankSensor.LOS(target))
                {
                    tankNav.Stop();                    

                    //float a = (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(enemyPos - pos)));

                    Vector3 relative = transform.InverseTransformPoint(target.position);
                    float a = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;

                    //Debug.Log(name + " angle " + a);

                    if (a > 1f)
                    {
                        tankMover.m_TurnInputValue = 0.2f;
                    }
                    else if (a < -1f)
                    {
                        tankMover.m_TurnInputValue = -0.2f;
                    }
                    else
                    {
                        if (UnityEngine.Random.value < 0.9f) tankShooter.m_FireButton = true;
                    }
                    
                }
                else
                {
                    if (tankNav.velocity.sqrMagnitude < 1f)
                    {
                        tankNav.Resume();
                    }
                }            
            }
            else  // no target
            {
                target = tankSensor.GetNearestEnemy();

                if (!target)
                {

                    if (moveTo.y > -1f)
                    {
                        if (tankNav.remainingDistance < 2f)
                        {
                            //Debug.Log(name + " path complete");
                            moveTo.y = -2f;
                        }
                    }

                    if (moveTo.y < -1f)
                    {
                        //Debug.Log(name + " getting random position");
                        if (RandomMove(out moveTo))
                        {
                            if (tankNav.SetDestination(moveTo))
                            {
                                moveToMarker.position = moveTo;
                            }
                        }
                        else
                        {
                            moveTo.y = -2f;
                        }
                    }
                }

            }
            
        }

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
