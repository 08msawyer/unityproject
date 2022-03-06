using UnityEngine;
using UnityEngine.UI;

namespace CoolBattleRoyaleZone
{
    /// <summary>
    /// Class which controls showing the timer of zone
    /// </summary>

    public class ShowZoneTime : MonoBehaviour
    {

        public Text TimeText;

        // Method for showing shrinking timer
        public void ShowShrinkingTime ( Zone.Timer timer )
        {
            TimeText.text = "<color=red>"                                           + "ZONE SHRINKING: \n <b>" +
                            ( timer.EndTime - timer.CurrentTime ).ToString ( "F0" ) + "</b></color>";
        }

        // Method for clearing text on end of last circle/step
        public void EndOfLastStep ( Zone.Timer timer ) { TimeText.text = ""; }

        // Method for showing waiting timer before shrinking
        public void ShowWaitingTime ( Zone.Timer timer )
        {
            TimeText.text = "ZONE SHRINKS IN: \n"                                   + "<b><color=red>" +
                            ( timer.EndTime - timer.CurrentTime ).ToString ( "F0" ) + "</color></b>";
        }
    }
}
