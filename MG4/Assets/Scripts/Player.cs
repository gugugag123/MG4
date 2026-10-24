using System;
using TMPro;
using UnityEngine;

public enum EquationType
{
    Add, Subtract, Multiply
}

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject _equationObject;
    [SerializeField] private TMP_Text _equationText;
    [SerializeField] private TMP_InputField _equationInputField;
    [SerializeField] private TMP_Text _pointsText;
    [SerializeField] private FishMovement _movement;
    [SerializeField] private GameObject _correctUI;
    [SerializeField] private GameObject _incorrectUI;
    [SerializeField] private float _incorrectBackwardsMovement;
    [SerializeField] private AudioSource _audio;

    private GameObject _currentFood;
    private EquationType _currentEquationType;
    private int _number1;
    private int _number2;
    private int _points;

    private void OnTriggerEnter(Collider other)
    {
        // Ignore scenery and don't replace an unanswered question with another.
        if (_currentFood != null || !(other.CompareTag("Add") ||
            other.CompareTag("Subtract") || other.CompareTag("Multiply"))) return;
        string operatorCharacter = "";
        if (other.gameObject.tag.Equals("Add"))
        {
            _currentEquationType = EquationType.Add;
            operatorCharacter = "+";
        }
        else if (other.gameObject.tag.Equals("Subtract"))
        {
            _currentEquationType = EquationType.Subtract;
            operatorCharacter = "-";
        }
        else if (other.gameObject.tag.Equals("Multiply"))
        {
            _currentEquationType = EquationType.Multiply;
            operatorCharacter = "x";
        }

        _currentFood = other.gameObject;

        _number1 = UnityEngine.Random.Range(0, 10);
        _number2 = UnityEngine.Random.Range(0, 10);

        _equationText.text = string.Format("{0} {1} {2} = ", _number1, operatorCharacter, _number2);

        _equationObject.SetActive(true);
        _movement.enabled = false;
        _equationInputField.text = "";
        _equationInputField.ActivateInputField();

        // STEP 6 -------------------------------------------------------------
        // Inside this method, we've detected a collision with fish food.
        // We want to make the food stop playing its floating animation when we
        //      collide with it.
        // To do so, GET the FloatAnimation Component on the GameObject we
        //      collided with, and CALL the StopAnimation method on this
        //      Component.
        //
        // Feature 5 of the rubric should work after this.


        other.GetComponent<FloatAnimation>().StopAnimation();
        // STEP 6 -------------------------------------------------------------
    }

    public void OnPlayerInput()
    {
        if (_currentFood == null) return;
        Debug.Log("Player wrote: " + _equationInputField.text);
        _equationObject.SetActive(false);
        _movement.enabled = true;

        bool correct = false;
        if (Int32.TryParse(_equationInputField.text, out int number))
        {
            switch (_currentEquationType)
            {
                case EquationType.Add:
                    // UNCOMMENT AFTER STEP 3
                    correct = CheckIfAdditionCorrect(number);
                    break;
                case EquationType.Subtract:
                    // UNCOMMENT AFTER STEP 4
                    correct = CheckIfSubtractionCorrect(number);
                    break;
                case EquationType.Multiply:
                    // UNCOMMENT AFTER STEP 5
                    correct = CheckIfMultiplicationCorrect(number);
                    break;
            }
        }

        Debug.Log("correct: " + correct);

        if (correct)
        {
            _audio.Play();

            _points += 1;
            _pointsText.text = "points: " + _points;

            _correctUI.SetActive(true);

            Destroy(_currentFood);
        }
        else
        {
            transform.position -= transform.forward * _incorrectBackwardsMovement;

            _incorrectUI.SetActive(true);
        }
        _currentFood = null;
    }

    // STEP 3 -----------------------------------------------------------------
    // Write a method named CheckIfAdditionCorrect to check if the answer a player
    //      gave for an addition question was correct or not.
    //
    // INPUT: an integer number 
    // OUTPUT: a true or false value
    //
    // The _number1 and _number2 member variables contain the two numbers in the
    //      question. For example, if the question displayed to the screen was
    //      5 + 2 = __, then _number1 will be 5 and _number2 will be 2. This is
    //      already coded for you :)
    //
    // You will need to fill in the body of the method to check IF _number1 plus
    //      _number2 is equal to the answer the player gave. The answer the player
    //      gave should be the sole PARAMETER for this method.
    //
    // After you write this method, uncomment the line above that says
    //      "UNCOMMENT AFTER STEP 3".
    //
    // Feature 4 of the rubric should work after you finish all of the 3 methods
    //      in this file.

    private bool CheckIfAdditionCorrect(int answer)
    {
        return _number1 + _number2 == answer;
    }

    // STEP 3 -----------------------------------------------------------------



    // STEP 4 -----------------------------------------------------------------
    // Write a method named CheckIfSubtractionCorrect that works almost exactly
    //      the same as CheckIfAdditionCorrect, but uses subtraction instead.
    //
    // INPUT: an integer number 
    // OUTPUT: a true or false value
    //
    // After you write this method, uncomment the line above that says
    //      "UNCOMMENT AFTER STEP 4".

    private bool CheckIfSubtractionCorrect(int answer)
    {
        return _number1 - _number2 == answer;
    }

    // STEP 4 -----------------------------------------------------------------



    // STEP 5 -----------------------------------------------------------------
    // Write a method named CheckIfMultiplicationCorrect that works almost exactly
    //      the same as CheckIfAdditionCorrect, but uses multiplication instead.
    //
    // INPUT: an integer number 
    // OUTPUT: a true or false value
    //
    // After you write this method, uncomment the line above that says
    //      "UNCOMMENT AFTER STEP 5".

    private bool CheckIfMultiplicationCorrect(int answer)
    {
        return _number1 * _number2 == answer;
    }

    // STEP 5 -----------------------------------------------------------------

}
