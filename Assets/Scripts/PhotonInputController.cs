using UnityEngine;

namespace DefaultNamespace
{
    public class PhotonInputController : MonoBehaviour, IRacketInput
    {
        public bool HasInput => throw new System.NotImplementedException();

        public float Input => throw new System.NotImplementedException();
    }
}
