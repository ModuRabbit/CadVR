﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;
using VRTK.SecondaryControllerGrabActions;

public class GameObjectInstancer : MonoBehaviour {

    [SerializeField]
    private GameObjectList objectSource;

    private List<GameObject> instances;

    void Awake()
    {
        objectSource.OnSelected += ObjectSelectedHandler;
    }

    private void ObjectSelectedHandler(GameObject selected)
    {
        GameObject instance = Instantiate<GameObject>(selected);
        VRTK_InteractableObject interactible = instance.AddComponent<VRTK_InteractableObject>();
        VRTK_ChildOfControllerGrabAttach primary = instance.AddComponent<VRTK_ChildOfControllerGrabAttach>();
        interactible.grabAttachMechanicScript = primary;
        VRTK_AxisScaleGrabAction secondary = instance.AddComponent<VRTK_AxisScaleGrabAction>();
        interactible.secondaryGrabActionScript = secondary;
        secondary.uniformScaling = true;
        secondary.ungrabDistance = 100.0f;

        instance.transform.localScale = Vector3.one * 0.01f;
    }
}