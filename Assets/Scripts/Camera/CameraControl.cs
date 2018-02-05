using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f; //kind of smooth                
    public float m_ScreenEdgeBuffer = 4f;           
    public float m_MinSize = 6.5f;                  
    [HideInInspector] public Transform[] m_Targets; 


    private Camera m_Camera;                        
    private float m_ZoomSpeed;                      
    private Vector3 m_MoveVelocity;                 
    private Vector3 m_DesiredPosition;        //average between the targets   


    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }


    private void FixedUpdate()
    {
        Move();
        Zoom();
    }


    private void Move()
    {
        FindAveragePosition();

        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)//if the target is not active.
                continue;

            averagePos += m_Targets[i].position;//sum target's positions.
            numTargets++;//count target number +1 .
        }

        if (numTargets > 0)
            averagePos /= numTargets;//divide the sum of the target's positions by the count target number.

        averagePos.y = transform.position.y; // we dont want to change the y axis.

        m_DesiredPosition = averagePos;
    }


    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize()
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition); //local position to refeer the size of the ort. camera.. We obtain it from the desired DISTANCE position.

        float size = 0f;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)//if the target does not exist -> continue, we dont want it.
                continue;

            //else... if exists...

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);// local pos. of the target.

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;//desired local pos. to target. --- desired position respect the target

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));//max between the current size and the y's desired position respect the target.

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);//max between the upper max or the x's desired position respect the target divided by the aspect... REMEMBER SIZE = DISTANCE / SIZE
        }
        
        size += m_ScreenEdgeBuffer; //sum the offset to the size

        size = Mathf.Max(size, m_MinSize); //check if the size is lower than the minSize. In that case, choose the max value -> minSize. If it is not, check the size.

        return size;
    }


    public void SetStartPositionAndSize()//for the first time...
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}