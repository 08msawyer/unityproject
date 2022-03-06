using UnityEngine;

namespace CoolBattleRoyaleZone
{
    /// <summary>
    /// Class which controls looking object at camera
    /// </summary>

    public class LookAtCamera : MonoBehaviour
    {
        public Transform Camera;

        // Start is called before the first frame update
        private void Start ( )
        {
            if ( !Camera ) return;
            // Rotate object towards camera
            transform.rotation = Quaternion.LookRotation ( -Camera.forward );
        }
    }
}
