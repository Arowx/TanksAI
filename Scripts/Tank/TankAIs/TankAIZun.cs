using UnityEngine;
using System.Collections;
using System;
 
namespace Complete
{
    public class TankAIZun : TankAI
    {
        private float chargingTime;

        protected override void SetName()
        {
            tankName = "Zuntatos";
        }

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
                float range = fromEnemy.magnitude;

                Vector3 relative = transform.InverseTransformPoint(enemyPos);
                float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;

                bool hasLoS = tankSensor.LOS(target);

                tankMover.m_TurnInputValue = Mathf.Clamp(0.1f * Math.Sign(angle) * angle * angle, -1f, 1f);

                if (hasLoS & Mathf.Abs(angle) > 2f)
                {
                    tankNav.Stop();
                }
                else if (hasLoS)
                {
                    tankNav.Stop();
                    if (range > 15f)
                    {
                        chargingTime += Time.deltaTime;
                        float desiredCharging = Mathf.Min(Mathf.Pow((range - 15f) / 15f, 2.0f), 0.5f);
                        if (chargingTime < desiredCharging)
                        {
                            tankShooter.m_FireButton = true; // Long Range Shots
                        }
                        else
                        {
                            tankShooter.m_FireButton = !tankShooter.m_FireButton;
                            chargingTime = 0f;
                        }
                    }
                    else
                    {
                        chargingTime = 0f;
                        tankShooter.m_FireButton = !tankShooter.m_FireButton; // Short Range Shots
                    }
                }
                else
                {
                    Vector3 goal = target.position - target.forward * 10f;
                    tankNav.SetDestination(goal);                    
                }
            }

            if (target == null)
            { // no target so random move
                if (tankNav.remainingDistance < 3f)
                {
                    RandomMove(out moveTo);
                    tankNav.SetDestination(moveTo);                    
                }
            }
        }
    }
}