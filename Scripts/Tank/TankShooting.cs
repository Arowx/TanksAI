using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class TankShooting : MonoBehaviour
    {
        public int m_PlayerNumber = 1;              // Used to identify the different players.
		[SerializeField]
        private Rigidbody m_Shell;                   // Prefab of the shell.
		[SerializeField]
        private Transform m_FireTransform;           // A child of the tank where the shells are spawned.
		[SerializeField]
        private Slider m_AimSlider;                  // A child of the tank that displays the current launch force.
		[SerializeField]
        private AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
		[SerializeField]
        private AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
		[SerializeField]
        private AudioClip m_FireClip;                // Audio that plays when each shot is fired.
		
        public const float m_MinLaunchForce = 10f;        // The force given to the shell if the fire button is not held.		
        public const float m_MaxLaunchForce = 30f;        // The force given to the shell if the fire button is held for the max charge time.		
        public const float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.

        public bool m_FireButton;                // The input axis that is used for launching shells.

        private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.

        // How fast the launch force increases, based on the max charge time.
        private const float m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;                
                    
        private bool m_Fired;                       // Whether or not the shell has been launched with this button press.

        private bool m_Reloading;

		[SerializeField]
        private float m_ShotDelay = 0.5f;

        private void OnEnable()
        {
            // When the tank is turned on, reset the launch force and the UI
            m_CurrentLaunchForce = m_MinLaunchForce;
            m_AimSlider.value = m_MinLaunchForce;
        }


        private void Start ()
        {
            // The rate that the launch force charges up is the range of possible forces by the max charge time.            

            Reload();
            
        }

        public void Reset()
        {
            m_Fired = true;
            m_FireButton = false;
            m_Reloading = false;
        }


        private void LateUpdate ()
        {
            // The slider should have a default value of the minimum launch force.
            m_AimSlider.value = m_MinLaunchForce;

            if (!m_Reloading)
            {

                // If the max force has been exceeded and the shell hasn't yet been launched...
                if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
                {
                    // ... use the max force and launch the shell.
                    m_CurrentLaunchForce = m_MaxLaunchForce;
                    Fire();
                }
                // Otherwise, if the fire button has just started being pressed...
                if (m_FireButton && m_Fired)
                {
                    // ... reset the fired flag and reset the launch force.
                    m_Fired = false;
                    m_CurrentLaunchForce = m_MinLaunchForce;

                    // Change the clip to the charging clip and start it playing.
                    m_ShootingAudio.clip = m_ChargingClip;
                    m_ShootingAudio.Play();
                }
                // Otherwise, if the fire button is being held and the shell hasn't been launched yet...
                if (m_FireButton && !m_Fired)
                {
                    // Increment the launch force and update the slider.
                    m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

                    m_AimSlider.value = m_CurrentLaunchForce;
                }
                // Otherwise, if the fire button is released and the shell hasn't been launched yet...
                if (!m_FireButton && !m_Fired)
                {
                    // ... launch the shell.
                    Fire();
                }
            }
        }

        private void Reload()
        {
            m_Reloading = true;
            Invoke("Reloaded", m_ShotDelay);
        }

        private void Fire ()
        {
            // Set the fired flag so only Fire is only called once.
            

            Reload();

            m_Fired = true;
            m_FireButton = false;

            // Create an instance of the shell and store a reference to it's rigidbody.
            Rigidbody shellInstance =
                Instantiate (m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

            // Set the shell's velocity to the launch force in the fire position's forward direction.
            shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward; ;

            // Change the clip to the firing clip and play it.
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play ();

            // Reset the launch force.  This is a precaution in case of missing button events.
            m_CurrentLaunchForce = m_MinLaunchForce;
        }

        private void Reloaded()
        {
            m_Reloading = false;
        }
    }
}