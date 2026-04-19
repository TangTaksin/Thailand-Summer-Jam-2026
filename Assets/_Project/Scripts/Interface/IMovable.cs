using UnityEngine;

public interface IMovable
{
    void MoveToTarget(Transform target);
    float MoveSpeed { get; set; }
}
