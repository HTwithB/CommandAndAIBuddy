using UnityEngine;
using UnityEngine.AI;

public class Robo : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;

    [SerializeField] private Transform _target;

    private void Update()
    {
        _agent.SetDestination(_target.position);
    }
}