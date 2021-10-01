using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CreatingCharacters
{
    public class DestroyOnContact : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Destroy(gameObject);
        }

    }
}

