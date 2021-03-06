﻿using HTC.UnityPlugin.Vive;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Teleportable : MonoBehaviour
    , IPointerExitHandler
{
    public enum TeleportButton
    {
        Trigger,
        Pad,
        Grip,
    }

    public Transform target;
    public Transform pivot;
    public float fadeDuration = 0.3f;

    public TeleportButton teleportButton = TeleportButton.Trigger;

    private Coroutine teleportCoroutine;

    public ControllerButton teleportViveButton
    {
        get
        {
            switch (teleportButton)
            {
                case TeleportButton.Pad:
                    return ControllerButton.Pad;

                case TeleportButton.Grip:
                    return ControllerButton.Grip;

                case TeleportButton.Trigger:
                default:
                    return ControllerButton.Trigger;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // skip if it was teleporting
        if (teleportCoroutine != null) { return; }

        VivePointerEventData viveEventData;
        if (!eventData.TryGetViveButtonEventData(out viveEventData)) { return; }

        // don't teleport if it was not releasing the button
        if (viveEventData.eligibleForClick) { return; }

        if (viveEventData.viveButton != teleportViveButton) { return; }

        var hitResult = eventData.pointerCurrentRaycast;
        if (!hitResult.isValid) { return; }

        var headVector = Vector3.ProjectOnPlane(pivot.position - target.position, target.up);
        var targetPos = hitResult.worldPosition - headVector;

        teleportCoroutine = StartCoroutine(StartTeleport(targetPos, fadeDuration));
    }

    public IEnumerator StartTeleport(Vector3 position, float duration)
    {
        var halfDuration = Mathf.Max(0f, duration * 0.5f);

        if (!Mathf.Approximately(halfDuration, 0f))
        {
            SteamVR_Fade.Start(new Color(0f, 0f, 0f, 1f), halfDuration);
            yield return new WaitForSeconds(halfDuration);
            yield return new WaitForEndOfFrame(); // to avoid from rendering guideline in wrong position
            target.position = position;
            SteamVR_Fade.Start(new Color(0f, 0f, 0f, 0f), halfDuration);
            yield return new WaitForSeconds(halfDuration);
        }
        else
        {
            yield return new WaitForEndOfFrame(); // to avoid from rendering guideline in wrong position
            target.position = position;
        }

        teleportCoroutine = null;
    }
}