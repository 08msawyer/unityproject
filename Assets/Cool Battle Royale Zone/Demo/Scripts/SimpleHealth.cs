using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CoolBattleRoyaleZone
{
	/// <summary>
	/// Class which controls health of simple characters
	/// </summary>

	public class SimpleHealth : MonoBehaviour
	{

		public float Health = 100; // Default amount of health
		public Text  HealthText;   // Text for showing current amount of health

		private bool _wait;

		private void Start ( )
		{
			// Setup default color to text
			if ( HealthText ) HealthText.text = "Health: <color=green>" + Health + "</color>";
		}

		private void Update ( )
		{
			// Getting zone current safe zone values
			var zonePos    = Zone.Instance.CurrentSafeZone.Position;
			var zoneRadius = Zone.Instance.CurrentSafeZone.Radius;
			// Checking distance between player and circle
			var dstToZone = Vector3.Distance ( new Vector3 ( transform.position.x , zonePos.y , transform.position.z ) ,
											   zonePos );
			// Checking if we inner of circle or not by radius and if not, start applying damage to health
			if ( dstToZone > zoneRadius && !_wait ) StartCoroutine ( DoDamageCoroutine ( ) );
		}

		// Method for waiting time between applying damage
		private IEnumerator DoDamageCoroutine ( )
		{
			_wait = true;
			DoDamage ( );
			yield return new WaitForSeconds ( 1 ); // Waiting between damages.
			_wait = false;
		}

		// Method for applying damage to health
		private void DoDamage ( )
		{
			Health -= Zone.Instance.CurStep + 1; // Applying damage based on current step index
			// Then choose text color : red if health less than 25 and green if greater than 25
			var hpColor = Health > 25 ? "<color=green>" : "<color=red>";
			if ( HealthText )
				HealthText.text = "Health: " + hpColor + Health + "</color>"; // Then setup this color to text
			if ( Health <= 0 )
				Destroy ( gameObject ); // And if health amount is zero,destroying the simple player
		}
	}
}
