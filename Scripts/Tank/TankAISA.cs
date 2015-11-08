using UnityEngine;
using System.Collections;
using System;

namespace Complete
{
    public class TankAISA : TankAI
    {
        public string ourTankName = "Samuel411";
        public bool targetFoundBefore;
        public bool targetNowFound = false;
        private bool foundTargetTime;
        public bool attackMode;
        public bool canAttack = true;
        public float powerUpTank = 0;
        public float targetDist;
        public float circleEnemyRadius = 20;
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

        protected override void SetName()
        {
            tankName = ourTankName;
        }

        public void Awake()
        {
            base.Awake();
            MoveTo(RandomMove());
        }

        void ResetNew()
        {
            targetNowFound = false;
            targetFoundBefore = false;
            canAttack = true;
            attackMode = false;
            currentAngleOnCircle = 0;
            circleEnemyRadius = 20;
            powerUpTank = 0;
            MoveTo(RandomMove());
        }

        void FindTarget()
        {
            currentAngleOnCircle = Mathf.Atan2(transform.position.x, transform.position.z) * Mathf.Rad2Deg;
        }

        void Update()
        {
            // Reset "method"
            if (target)
                targetFoundBefore = true;
            if (targetFoundBefore && !target)
            {
                ResetNew();
            }


            // Found Target "Method"
            if (!targetNowFound && target)
            {
                targetNowFound = true;
                FindTarget();
            }

            // Set destination to marker
            tankNav.SetDestination(moveTo);

            // Movements
            if (!attackMode)
            {
                if (!target)
                {
                    // Enemy not found, Lets look for him!
                    if (tankNav.remainingDistance < 2f)
                    {
                        MoveTo(RandomMove());
                    }
                }
            }

            // Find Target!
            if (target == null)
            {
                target = tankSensor.GetNearestEnemy();
                return;
            }

            MoveTo(CircleEnemy(target.position));
            CurrentAngleOnCircle += 13f * Time.deltaTime;

            Vector3 relative = transform.InverseTransformPoint(target.position);
            float a = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;

            Vector3 targetDistVector = transform.position - target.position;
            targetDist = targetDistVector.magnitude;

            // Attack if we can turn quickly
            // Complete the attack quickly
            if (!attackMode && canAttack && target)
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
            if (attackMode)
            {
                if (a > 1)
                {
                    tankMover.m_TurnInputValue = 1f;
                }
                else if (a < -3f)
                {
                    tankMover.m_TurnInputValue = -1f;
                }
                else
                {
                    tankMover.m_TurnInputValue = 0f;
                    if (tankSensor.LOS(target))
                    {
                        tankShooter.m_FireButton = true;
                        if (!foundTargetTime)
                        {
                            foundTargetTime = true;
                            powerUpTank = Time.time + TargetPower(targetDist);
                        }
                        if (Time.time > powerUpTank)
                        {
                            powerUpTank = 0;
                            foundTargetTime = false;
                            tankShooter.m_FireButton = false;
                        }
                    }
                }
            }
            else
            {
                powerUpTank = 0;
                if (a > 2f)
                {
                    tankMover.m_TurnInputValue = 1f;
                }
                else if (a < -2f)
                {
                    tankMover.m_TurnInputValue = -1f;
                }
                else
                {
                    tankMover.m_TurnInputValue = 0f;
                }
            }
        }

        /// <summary>
        /// Gets how much power we should give the projectile before shooting it based on enemy distance
        /// </summary>
        /// <param name="dist">The distance from the target</param>
        /// <returns>Power Target Needed</returns>
        float TargetPower(float dist)
        {
            float targetPower = 0;
            targetPower = (dist * 2) / (dist * dist);
            targetPower = targetPower / TankShooting.m_MaxChargeTime;
            targetPower = targetPower * 3f;
            return targetPower;
        }

        /// <summary>
        /// Move our Marker to the given Vector3 Position
        /// </summary>
        /// <param name="movePos">Target Move Positiion</param>
        void MoveTo(Vector3 movePos)
        {
            moveTo = movePos;
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
            returnVector.x = enemyPos.x + (circleEnemyRadius * Mathf.Cos(currentAngleOnCircle * Mathf.PI / 180f));
            // Get Z pos on our circle
            returnVector.z = enemyPos.z + (circleEnemyRadius * Mathf.Sin(currentAngleOnCircle * Mathf.PI / 180f));
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
            returnVector = transform.position + UnityEngine.Random.insideUnitSphere * 20;
            returnVector.y = transform.position.y;
            return returnVector;
        }
    }
}