using System;
using UnityEngine;

public class InteractionData
{
    public Item item;
    public Transform examinationPoint;
    public Vector3 playerPosition;
    public IInteractionHandler handler;
    public Vector2 input;
    public Vector2 inputRaw;
    public bool isExitRequested;
    public bool isHoldingInteraction;
    public Action forceExitCallback;
}

public class InteractionControlData
{
    public Transform objectToFollow;
    public Transform objectToLookAt;
    public Vector2? lookConstraints;
    public bool preventExitOnRequest;
    public bool freezeMovement;
    public bool freezeRotation;
    public bool freezeItem;
}