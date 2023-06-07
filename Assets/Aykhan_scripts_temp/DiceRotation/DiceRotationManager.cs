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
    [SerializeField] private float returnDuration;
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

        //Return to original rotation
        for (var startTime = Time.time; Time.time - startTime <= returnDuration;)
        {
            diceCube.rotation = Quaternion.Lerp(diceCube.rotation, Quaternion.identity,
                (Time.time - startTime) / returnDuration);
            yield return null;
        }

        //Find side animation
        var diceSideAnimation = sideAnimations.Find(animation => animation.sideNumber == diceRoll);
        //Rotate to required side
        foreach (var rotation in diceSideAnimation.rotations)
        {
            var startAngle = diceCube.eulerAngles;
            startAngle.x %= 360;
            startAngle.y %= 360;
            startAngle.z %= 360;
            for (var startTime = Time.time; Time.time - startTime <= diceSideAnimation.eachSideDuration;)
            {
                var increment = Vector3.Lerp(Vector3.zero, rotation,
                    (Time.time - startTime) / diceSideAnimation.eachSideDuration);
                diceCube.eulerAngles = (startAngle + increment);
                var angleCopy = diceCube.eulerAngles;
                angleCopy.x %= 360;
                angleCopy.y %= 360;
                angleCopy.z %= 360;
                diceCube.eulerAngles = angleCopy;
                print("Increment is : " + increment + " angle should be " + (startAngle + increment));
                yield return null;
            }
        }

        print("Dice rolled: " + diceRoll);
        OnDiceRolled?.Invoke(diceRoll);
    }
}

[Serializable]
public class DiceSideAnimation
{
    public int sideNumber;
    public List<Vector3> rotations;
    public float eachSideDuration;
}