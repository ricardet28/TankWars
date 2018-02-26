using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlackTankMovement : MonoBehaviour {

    private GameManager _gameManager;
    private NavMeshAgent _navMeshAgent;
    private Rigidbody _rb;

    private TankManager closest;

    private bool m_Fired = true;
    public float launchForce = 2000f;
    public Rigidbody m_Shell;
    public Transform m_FireTransform;
    public float distanceToStop = 10f;

    public AudioSource m_ShootingAudio;
    public AudioClip m_FireClip;
    public float m_PitchRange = 0.2f;
    private float m_OriginalPitch;
    public AudioSource m_MovementAudio;
    public AudioClip m_EngineDriving;

    private float currentTimer;
    public float maxTimer = 1f;

    private void Awake()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();

        m_MovementAudio.clip = m_EngineDriving;
        m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
        m_MovementAudio.Play();//play idling audio
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (m_Fired)
        {
            currentTimer += Time.deltaTime;
            if (currentTimer >= maxTimer)
            {
                m_Fired = false;
                currentTimer = 0f;
            }
        }

        TankManager closestTank = ClosestTank();

        if (Vector3.Distance(closestTank.m_Instance.transform.position, this.transform.position) > distanceToStop)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(closestTank.m_Instance.transform.position);
        }

        else
        {
            
            _navMeshAgent.isStopped = true;
            if (!m_Fired)
            {
                Fire();
            }
            
            
        }
        
        //this.transform.LookAt(closestTank.m_Instance.transform);

	}
    
    private TankManager ClosestTank()
    {
        
        TankManager[] listOfTanks = _gameManager.m_Tanks;
        closest = listOfTanks[0];
        float minDistanceToTank = Vector3.Distance(this.gameObject.transform.position, listOfTanks[0].m_Instance.transform.position);
        //ponemos de mas cercano al primero.
        for (int i = 0; i < _gameManager.m_Tanks.Length; i++)
        {
            float distanceToTank = Vector3.Distance(this.gameObject.transform.position, listOfTanks[i].m_Instance.gameObject.transform.position);
            if (distanceToTank <= minDistanceToTank)
            {
                closest = listOfTanks[i];
            }
            //Debug.Log(distanceToTank);

        }

        return closest;
    }



    private void Fire()
    {
        // Instantiate and launch the shell.
        m_Fired = true;
        Rigidbody shellInstance = (Rigidbody)Instantiate(m_Shell, m_FireTransform.transform.position, m_FireTransform.transform.rotation);


        shellInstance.velocity = m_FireTransform.forward * Vector3.Distance(closest.m_Instance.transform.position, this.transform.position);
        
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
        
    }


}
