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
                if ((moveTo - enemyPos).sqrMagnitude > 4f)
                {
                    moveTo = enemyPos;

                    if ((pos - moveTo).sqrMagnitude > 16f)
                    {
						if (!tankNav.SetDestination(moveTo))
						{
							moveTo.y = -2;
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
						tankNav.SetDestination(moveTo);
                    }
                    else
                    {
                        moveTo.y = -2f;
                    }
                }
            }

        }
        
    }

}
