using UnityEngine;
using System.Collections.Generic;
using System;

public class TankSensor : MonoBehaviour {

    private List<Transform> enemyTanksList = new List<Transform>();

	[SerializeField]
    private LayerMask losMask;

    public void Reset()
    {
        enemyTanksList.Clear();
    }

    public Transform GetNearestEnemy()
    {
        Transform nearest = null;

        if ( enemyTanksList.Count > 0 )
        {
            if (enemyTanksList.Count == 1)
            {
                nearest = enemyTanksList[0];
            }
            else
            {
                float distance = 100f;

                for (int i = 0; i < enemyTanksList.Count; i++)
                {
                    if (LOS(enemyTanksList[i]))
                    {
                        float d = (enemyTanksList[i].position - transform.position).sqrMagnitude;
                        if (d < distance)
                        {
                            nearest = enemyTanksList[i];
                            distance = d;
                        }
                    }
                }
            }
        }
        return nearest;
    }

	public void OnTriggerEnter( Collider other )
    {
        Transform otherTank = other.transform;

        if ((otherTank != transform) &&
            !enemyTanksList.Contains(otherTank))
        {
            enemyTanksList.Add(otherTank);
        }
    }

    public void OnTriggerExit( Collider other )
    {
        Transform otherTank = other.transform;

        if (enemyTanksList.Contains(otherTank))
        {
            enemyTanksList.Remove(otherTank);
        }
    }

    public bool LOS(Transform target)
    {
        Vector3 pos = transform.position + Vector3.up;
        Vector3 targetPos = target.position + Vector3.up;

        Vector3 direction = targetPos - pos;

        direction.Normalize();

        pos += direction;

        RaycastHit hitInfo;
        
        if (Physics.Raycast(pos,  direction, out hitInfo, 50f, losMask))
        {
            if (hitInfo.collider.transform == target)
            {
                Debug.DrawLine(pos, pos + direction * hitInfo.distance, Color.red, 10f);
                return true;
            }
        }
        return false;
    }
}
