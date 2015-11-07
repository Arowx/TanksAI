using UnityEngine;
using System.Collections;
using System;

namespace Complete
{
    public class TankAIFSM : TankAI
    {

        public bool targetFoundBefore;
        public bool attackMode;
        public bool canAttack = true;
        public bool attackModeCooldownComplete = true;
        public float targetDist;
        public float circleEnemyRadius = 11;
        public float currentAngleOnCircle = 0;
        public float CurrentAngleOnCircle
        {
            get
            {
                return currentAngleOnCircle;
            }
            set
            {
                if (currentAngleOnCircle < 0)
                    currentAngleOnCircle = 0;
                else
                    currentAngleOnCircle = value;
            }
        }

        public void Awake()
        {
            tankName = "Samuel411";
            moveToMarker.position = RandomMove();
        }

        // Your TankAI should mainly be controlled from the Update() function

        // Use the tankNav, tankMover and tankShooter to manouver and battle

        // tankShooter.m_Fire - true power up / false to shoot

        void Update()
        {
            // Reset "method"
            if (target)
                targetFoundBefore = true;
            if (targetFoundBefore && !target)
            {
                targetFoundBefore = false;
                currentAngleOnCircle = 0;
                circleEnemyRadius = 11;
                canAttack = true;
                attackModeCooldownComplete = true;
                attackMode = false;
                moveToMarker.position = transform.position;
            }

            // Set destination to marker
            tankNav.SetDestination(moveToMarker.position);

            // Movements
            if (!attackMode)
            {
                if (target)
                {
                    // Make circles around enemy!
                    CurrentAngleOnCircle += 0.5f * Time.deltaTime;

                    moveToMarker.position = target.position;
                }
                else
                {
                    // Enemy not found, Lets look for him!
                    if (tankNav.remainingDistance < 2f)
                    {
                        moveToMarker.position = RandomMove();
                    }
                }
            }

            // Find Target!
            if (target == null)
            {
                target = tankSensor.GetNearestEnemy();
                return;
            }

            Vector3 relative = transform.InverseTransformPoint(target.position);
            float a = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;

            Vector3 targetDistVector = transform.position - target.position;
            targetDist = targetDistVector.magnitude;

            // Attack if we can turn quickly
            // Complete the attack quickly and swiftly then continue
            if (!attackMode && canAttack && attackModeCooldownComplete && targetDist < 15)
            {
                if (a < 70f)
                {
                    attackMode = true;
                }
                else if (a > -70f)
                {
                    attackMode = true;
                }
                else
                {
                    attackMode = false;
                }
            }

            // Make sure we can attack
            if (!canAttack)
            {
                attackMode = false;
            }

            // Attack functions
            if (attackMode && targetDist < 15)
            {
                tankNav.Stop();
                if (a > 2f)
                {
                    tankMover.m_TurnInputValue = 0.2f;
                }
                else if (a < -2f)
                {
                    tankMover.m_TurnInputValue = -0.2f;
                }
                else
                {
                    tankMover.m_TurnInputValue = 0f;
                    if (tankSensor.LOS(target))
                    {
                        tankShooter.m_FireButton = !tankShooter.m_FireButton;
                    }
                }
            }
            else
            {
                tankNav.Resume();
            }
        }

        /// <summary>
        /// Script that returns a position in a circular shape around the inputed Vector
        /// </summary>
        /// <param name="enemyPos">Enemy Position</param>
        /// <returns>Position in circle around EnemyPos</returns>
        Vector3 CircleEnemy(Vector3 enemyPos)
        {
            Vector3 returnVector;
            returnVector.y = transform.position.y;
            // Get X pos on our circle
            returnVector.x = enemyPos.x + circleEnemyRadius * Mathf.Cos(CurrentAngleOnCircle);
            // Get Z pos on our circle
            returnVector.z = enemyPos.z + circleEnemyRadius * Mathf.Sin(CurrentAngleOnCircle);
            // Send the coordinates! FIRE!
            return returnVector;
        }

        /// <summary>
        /// Returns a random position around the player's position.
        /// </summary>
        /// <returns>Random Position Near Tank</returns>
        Vector3 RandomMove()
        {
            Vector3 returnVector;
            returnVector = transform.position + UnityEngine.Random.insideUnitSphere * 8;
            returnVector.y = transform.position.y;
            //Debug.Log(returnVector);
            return returnVector;
        }
    }
}
