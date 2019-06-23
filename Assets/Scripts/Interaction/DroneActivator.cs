using UnityEngine;
using System.Collections;
using VRTK;


public class DroneActivator : MonoBehaviour
{
    public void Start()
    {
        GetComponent<VRTK_ControllerEvents>().TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadReleased);
    }

    private void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
    {
        Debug.Log("Touchpad pressed");

        BroadcastAll("ToggleActivation", null);

    }

    public static void BroadcastAll(string methodName, System.Object parameters)
    {
        GameObject[] gos = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
        foreach (GameObject go in gos)
        {
            if (go && go.transform.parent == null)
            {
                go.gameObject.BroadcastMessage(methodName, parameters, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

}
