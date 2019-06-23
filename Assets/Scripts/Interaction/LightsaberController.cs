using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class LightsaberController : VRTK_InteractableObject {

    public override void StartUsing(GameObject usingObject)
    {
        base.StartUsing(usingObject);

        ToggleLightsaberOnOff();
        
        
    }

    protected void Start()
    {
    }

    private void ToggleLightsaberOnOff()
    {
        // find the lightsaber
        Lightsaber lightsaber = GetComponentInChildren<Lightsaber>();
        
        // toggle activation
        lightsaber.ToggleLightsaberOnOff();

        // Debug.Log("Toggle lightsaber activation: " + lightsaber);

    }
}
