using UnityEngine;
using System.Collections;
using System;

namespace Complete
{
    public class TankAIDQ : TankAI {


        protected override void SetName()
        {            
            tankName = "DanielQuick";
        }

        // Your TankAI should mainly be controlled from the Update() function

        // Use the tankNav, tankMover and tankShooter to manouver and battle

        // tankShooter.m_Fire - true power up / false to shoot

        void Update() {

            if (target == null)
            {
                target = tankSensor.GetNearestEnemy();
                return;
            }

            Vector3 relative = transform.InverseTransformPoint(target.position);
            float a = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;

            if (a > 1f)
            {
                tankMover.m_TurnInputValue = 0.2f;
            }
            else if (a < -1f)
            {
                tankMover.m_TurnInputValue = -0.2f;
            }

            if (tankSensor.LOS(target))
            {
                tankShooter.m_FireButton = !tankShooter.m_FireButton;
            }

        }        
        
    }
}
