using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Complete
{
    public class NPCTankShooting : MonoBehaviour
    {

        public Rigidbody m_Shell;                   // Prefab of the shell.
        public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
        public Slider m_AimSlider;                  // A child of the tank that displays the current launch force.
        public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
        public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
        public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
        public float m_MinLaunchForce = 15f;        // The force given to the shell if the fire button is not held.
        public float m_MaxLaunchForce = 30f;        // The force given to the shell if the fire button is held for the max charge time.
        public float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.

        private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
        private float m_ChargeSpeed = 10f;                // How fast the launch force increases, based on the max charge time.
        private bool m_Fired;                       // Whether or not the shell has been launched with this button press.

        private NPCTankMovement _NPCTankMovement;
        private float currentTime = 0f;
        public float timeRecoil = 1f;
       


        private void Awake()
        {
            _NPCTankMovement = GetComponent<NPCTankMovement>();
        }

        private void OnEnable()
        {
            // When the tank is turned on, reset the launch force and the UI
            m_CurrentLaunchForce = m_MinLaunchForce;
            m_AimSlider.value = m_MinLaunchForce;
        }


        private void Start()
        {
           
        }


        private void Update()
        {
            // The slider should have a default value of the minimum launch force.

            Vector3 direction = _NPCTankMovement.ourTurret.transform.forward;
            int layer = LayerMask.GetMask("Players");
            direction.y = 0;
            RaycastHit hit;
            
            
            Debug.Log("Time = " + CheckTimeElapsed() + ", canShoot = " + _NPCTankMovement.canShoot + ", raycast: " + Physics.SphereCast(transform.position, 3f, direction, out hit, Mathf.Infinity, layer));
          
            if (CheckTimeElapsed() && Physics.SphereCast(transform.position, 3f, direction, out hit, Mathf.Infinity, layer) && _NPCTankMovement.canShoot)
            {
                    float desiredForce = Vector3.Distance(this.transform.position, _NPCTankMovement._nearestTank.m_Instance.transform.position);
                    m_CurrentLaunchForce += Time.deltaTime * m_ChargeSpeed;
                    m_AimSlider.value = m_CurrentLaunchForce;
                    if (m_CurrentLaunchForce >= desiredForce)
                    {
                        Fire();
                        currentTime = 0f;
                        m_AimSlider.value = 0;
                        m_CurrentLaunchForce = 0;
                        desiredForce = 0;
                        _NPCTankMovement.canShoot = false;
                    }         
            } 
        }

        private void Fire()
        {
            
            // Create an instance of the shell and store a reference to it's rigidbody.
            Rigidbody shellInstance =
                Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

            // Set the shell's velocity to the launch force in the fire position's forward direction.
            shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

            // Change the clip to the firing clip and play it.
            //m_ShootingAudio.clip = m_FireClip;
            //m_ShootingAudio.Play();

            // Reset the launch force.  This is a precaution in case of missing button events.
        }

        private bool CheckTimeElapsed()
        {
            currentTime += Time.deltaTime;
            if (currentTime >= timeRecoil)
            {
                return true;
            }
            return false;
        }
    }
}