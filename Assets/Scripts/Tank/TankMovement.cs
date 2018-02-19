using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;         
    public float m_Speed = 12f;            
    public float m_TurnSpeed = 180f;       
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;

    private string m_MovementAxisName;     
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;         
    private float m_MovementInputValue;    
    private float m_TurnInputValue;        
    private float m_OriginalPitch;

    private bool forward;

    public float smooth;

    
    private void Awake()
    {
        
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        m_MovementAxisName = "Vertical" + m_PlayerNumber;//vertical1 or vertical2
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;//horizontal1 or horizontal2

        m_OriginalPitch = m_MovementAudio.pitch;
    }
    

    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        EngineAudio();
        ManageAcceleration();
      

        //Debug.Log(m_Rigidbody.velocity);
    }

    private void ManageAcceleration()
    {
       if (Input.GetAxisRaw(m_MovementAxisName) == 1)
        {
            forward = true;
        }
       else if (Input.GetAxisRaw (m_MovementAxisName) == -1)
        {
            forward = false;
        }

    }

    

    private void EngineAudio()
    {
        if (Mathf.Abs(m_MovementInputValue) >= 0.1f || Mathf.Abs(m_TurnInputValue) >= 0.1f)//if there is any movement higher than 0.1
        {
            if (m_MovementAudio.clip == m_EngineIdling)//if it is idling
            {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();//play movement audio
            }      
        }
        else //if both types of movement (movement and turn) are lower than 0.1
        {
            if (m_MovementAudio.clip == m_EngineDriving)//if it is driving
            {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();//play idling audio
            }     
        }
    }


    private void FixedUpdate()
    {
        // Move and turn the tank.
        Move();
        Turn(); 
    }


    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
       
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement );

    }


    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
        
    }

    
}