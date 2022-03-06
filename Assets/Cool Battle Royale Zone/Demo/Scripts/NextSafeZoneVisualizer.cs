using UnityEngine;

namespace CoolBattleRoyaleZone
{
    /// <summary>
    /// Class which controls visualizing of next safe zone circle cylinder or something your own
    /// </summary>

    public class NextSafeZoneVisualizer : MonoBehaviour
    {

        // Update is called once per frame
        public void UpdateZone ( Zone.Timer timer )
        {
            transform.position   = Zone.Instance.NextSafeZone.Position;
            transform.localScale = Vector3.one * Zone.Instance.NextSafeZone.Radius * 2;
        }
    }
}
