using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;              


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Find all the colliders in an area around the shell and damage them.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for (int i = 0; i<colliders.Length; i++)
        {
            //Take the rigidbodys of these colliders.
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            if (!targetRigidbody)
                continue;
            //Add a explosion force to them.
            targetRigidbody.AddExplosionForce(m_ExplosionForce, this.transform.position, m_ExplosionRadius);

            //Take the targetHealth script
            TankHealth targetHealth = targetRigidbody.gameObject.GetComponent<TankHealth>();

            if (!targetHealth)
                continue;
            //Calculate damage to do.
            float damage = CalculateDamage(targetRigidbody.position);

            //Apply the damage.
            targetHealth.TakeDamage(damage);

            //..the same as...
            //targetRigidbody.gameObject.GetComponent<TankHealth>().TakeDamage(damage);

        }

        m_ExplosionParticles.transform.parent = null;
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        ParticleSystem.MainModule mainModule = m_ExplosionParticles.main;
        Destroy(m_ExplosionParticles.gameObject, mainModule.duration);
        Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        Vector3 explosionToTarget = targetPosition - transform.position;

        float explosionDistance = explosionToTarget.magnitude;

        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius; //the factor to multiply the damage... between 0 and 1... depending on how big is explosionDistance...

        float damage = relativeDistance * m_MaxDamage;

        damage = Mathf.Max(0f, damage);

        return damage;


    }
}