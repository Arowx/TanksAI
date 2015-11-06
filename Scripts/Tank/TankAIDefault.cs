using UnityEngine;
using System.Collections;

public class TankAIDefault : Complete.TankAI {

	void Update() {
        tankShooter.m_FireButton = false;

        Vector3 pos = transform.position;
        

        if (target)
        {

            Vector3 enemyPos = target.position;

            float sqrRange = (pos - enemyPos).sqrMagnitude;

            if (sqrRange > 144f)
            {
                if ((destination - enemyPos).sqrMagnitude > 4f)
                {
                    destination = enemyPos;

                    if ((pos - destination).sqrMagnitude > 16f)
                    {
						if (!tankNav.SetDestination(destination))
						{
							destination.y = -2;
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

                if (destination.y > -1f)
                {
                    if (tankNav.remainingDistance < 2f)
                    {
                        //Debug.Log(name + " path complete");
                        destination.y = -2f;
                    }
                }

                if (destination.y < -1f)
                {
                    //Debug.Log(name + " getting random position");
                    if (RandomMove(out destination))
                    {
						tankNav.SetDestination(destination);
                    }
                    else
                    {
                        destination.y = -2f;
                    }
                }
            }

        }
        
    }

}
