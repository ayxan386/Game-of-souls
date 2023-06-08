using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiceRotationManager : MonoBehaviour
{
    [SerializeField] private Transform diceCube;
    [SerializeField] private Vector2Int rollRange;
    [SerializeField] private Vector3 rotationSpeed;
    [SerializeField] private float rotationDuration;
    [SerializeField] private List<DiceSideAnimation> sideAnimations;

    public static DiceRotationManager Instance { get; private set; }
    public static event Action<int> OnDiceRolled;

    private void Awake()
    {
        Instance = this;
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(850, 100, 200, 90), "Roll the dice"))
        {
            RollDice();
        }
    }

    public void RollDice()
    {
        var diceRoll = Random.Range(rollRange.x, rollRange.y);
        StartCoroutine(RotationAnimation(diceRoll));
    }

    private IEnumerator RotationAnimation(int diceRoll)
    {
        diceCube.rotation = Quaternion.identity;
        //Fast roll rotation
        for (var startTime = Time.time; Time.time - startTime <= rotationDuration;)
        {
            diceCube.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
            yield return null;
        }

        //Find side animation
        var diceSideAnimation = sideAnimations.Find(animation => animation.sideNumber == diceRoll);
        //Rotate to required side
        var startRotation = diceCube.localEulerAngles;
        for (int i = 0; i < diceSideAnimation.numberOfFrames; i++)
        {
            diceCube.localEulerAngles = Vector3.Lerp(startRotation, diceSideAnimation.desiredRotation,
                (1f * i / diceSideAnimation.numberOfFrames));
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