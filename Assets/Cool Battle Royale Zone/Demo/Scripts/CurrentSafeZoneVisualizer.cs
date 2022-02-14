using UnityEngine;


namespace CoolBattleRoyaleZone
{
    /// <summary>
    /// Class which controls visualizing of current safe zone circle cylinder or something your own
    /// </summary>

    public class CurrentSafeZoneVisualizer : MonoBehaviour
    {

        // Update is called once per frame
        public void UpdateZone ( Zone.Timer timer )
        {
            transform.position   = Zone.Instance.CurrentSafeZone.Position;
            transform.localScale = Vector3.one * Zone.Instance.CurrentSafeZone.Radius * 2;
        }
    }
}
