using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiceRotationManager : MonoBehaviour
{
    [SerializeField] private Transform diceCube;
    [SerializeField] private Vector2Int rollRange;
    [SerializeField] private Vector2Int numberOfRotationsRange;
    [SerializeField] private int rotationDuration;
    [SerializeField] private int rotationSpeedFactor;
    [SerializeField] private List<DiceSideAnimation> sideAnimations;

    [SerializeField] private bool fixedRoll;
    [SerializeField] private int roll;

    public static DiceRotationManager Instance { get; private set; }
    public static event Action<int> OnDiceRolled;
    public bool CanRoll { get; set; }

    private void Awake()
    {
        Instance = this;
    }


    [ContextMenu("Roll dice")]
    public void RollDice()
    {
        if (!CanRoll) return;

        var diceRoll = Random.Range(rollRange.x, rollRange.y);
        if (fixedRoll)
        {
            diceRoll = roll;
        }
        StartCoroutine(RotationAnimation(diceRoll));
    }

    private IEnumerator RotationAnimation(int diceRoll)
    {
        diceCube.rotation = Quaternion.identity;
        //Fast roll rotation
        var numberOfFrames = rotationDuration / rotationSpeedFactor;
        var numberOfRotations = Random.Range(numberOfRotationsRange.x, numberOfRotationsRange.y);
        for (int j = 0; j < numberOfRotations; j++)
        {
            var randomRoll = Random.Range(rollRange.x, rollRange.y);
            var diceSideAnimation = sideAnimations.Find(animation => animation.sideNumber == randomRoll);
            var startRotation = diceCube.localEulerAngles;
            for (int i = 0; i < numberOfFrames; i++)
            {
                diceCube.localEulerAngles = Vector3.Lerp(startRotation, diceSideAnimation.desiredRotation,
                    (1f * i / numberOfFrames));
                yield return null;
            }
        }

        //Find side animation
        var chosenDiceSideAnimation = sideAnimations.Find(animation => animation.sideNumber == diceRoll);
        //Rotate to required side
        var initialRotation = diceCube.localEulerAngles;
        numberOfFrames = chosenDiceSideAnimation.numberOfFrames / rotationSpeedFactor;
        for (int i = 0; i <= numberOfFrames; i++)
        {
            diceCube.localEulerAngles = Vector3.Lerp(initialRotation, chosenDiceSideAnimation.desiredRotation,
                (1f * i / numberOfFrames));
            yield return null;
        }

        print("Dice rolled: " + diceRoll);
        OnDiceRolled?.Invoke(diceRoll);
    }
}

[Serializable]
public class DiceSideAnimation
{
    public int sideNumber;
    public Vector3 desiredRotation;
    public int numberOfFrames;
}