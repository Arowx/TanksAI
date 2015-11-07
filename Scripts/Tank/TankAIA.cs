using UnityEngine;
using System.Collections;
using System;

namespace Complete
{
    public class TankAIA : TankAI {

        private float chargingTime;

        public void Awake()
        {            
            tankName = "Arowx2";
        }

        // Your TankAI should mainly be controlled from the Update() function

        // Use the tankNav, tankMover and tankShooter to manouver and battle

        // tankShooter.m_Fire - true power up / false to shoot

        void Update()
        {

            Vector3 pos = transform.position;

            if (target == null)
            {
                target = tankSensor.GetNearestEnemy();
            }

            if (target)
            {
                Vector3 enemyPos = target.position;

                Vector3 fromEnemy = pos - enemyPos;
                float sqrRange = fromEnemy.sqrMagnitude;

                Vector3 relative = transform.InverseTransformPoint(enemyPos);
                float a = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;

                bool los = tankSensor.LOS(target);

                float turn = 0.1f;

                if (a > 1f)
                {
                    if (a > 10f) turn = 1f;
                    tankMover.m_TurnInputValue = turn;
                }
                else if (a < -1f)
                {
                    if (a < -10f) turn = 1f;
                    tankMover.m_TurnInputValue = -turn;
                }

                if (los)                   
                {
                    if (sqrRange > 200f)
                    {
                        chargingTime += Time.deltaTime;

                        if (chargingTime < 0.5f)
                        {
                            tankShooter.m_FireButton = true; // Long Range Shots
                        }
                        else
                        {
                            tankShooter.m_FireButton = false;
                            chargingTime = 0f;
                        }
                    }
                    else
                    {
                        chargingTime = 0f;
                        tankShooter.m_FireButton = !tankShooter.m_FireButton; // Short Range Shots
                    }
                }

                // get and maintain maximum engagement distance from target

                //if (sqrRange > 900f) // 30 units
                //{

                if (los && sqrRange < 900f)
                {
                    tankNav.Stop();
                }
                else
                {
                    fromEnemy.Normalize();
                    fromEnemy *= 29f;

                    moveTo = enemyPos + fromEnemy;

                    if ((pos - moveTo).sqrMagnitude > 2f)
                    {
                        if (tankNav.SetDestination(moveTo))
                        {
                            moveToMarker.position = moveTo;
                        }
                    }
                    else if (!los)
                    {
                        fromEnemy = Quaternion.AngleAxis(5f, Vector3.up) * fromEnemy;
                        moveTo = enemyPos + fromEnemy;

                        if (tankNav.SetDestination(moveTo))
                        {
                            moveToMarker.position = moveTo;
                        }
                    }
                }                
                    
            }

            if (target == null) // no target so random move
            {
                if (tankNav.remainingDistance < 3f)
                {
                    RandomMove(out moveTo);
                    tankNav.SetDestination(moveTo);
                    moveToMarker.position = moveTo;
                }
            }           
        }
    }

}
