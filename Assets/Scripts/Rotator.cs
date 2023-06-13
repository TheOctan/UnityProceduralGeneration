using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OctanGames
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed = 10;
        [SerializeField] private Vector3 _axes = Vector3.one;

        private void Update()
        {
            Quaternion rotationX = Quaternion.AngleAxis(Time.deltaTime * _rotationSpeed * _axes.x, Vector3.right);
            Quaternion rotationY = Quaternion.AngleAxis(Time.deltaTime * _rotationSpeed * _axes.y, Vector3.up);
            Quaternion rotationZ = Quaternion.AngleAxis(Time.deltaTime * _rotationSpeed * _axes.z, Vector3.forward);
            transform.rotation *= rotationX * rotationY * rotationZ;
        }
    }
}
