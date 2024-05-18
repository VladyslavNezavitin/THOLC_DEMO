using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ladder))]
public class LadderSound : MonoBehaviour
{
    [SerializeField] private SoundPlaybackData[] _stepSounds;

    private SoundManager SoundManager => ProjectContext.Instance.SoundManager;
    private Ladder _ladder;

    private void Awake() => _ladder = GetComponent<Ladder>();
    private void OnEnable() => _ladder.Step += Ladder_OnStep;
    private void OnDestroy() => _ladder.Step -= Ladder_OnStep;

    private void Ladder_OnStep(Vector3 stepPosition) =>
        SoundManager.PlayRandomSound(_stepSounds, stepPosition);
}