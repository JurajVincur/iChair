using UnityEngine;

public class FixationSwipeStateMachine : MonoBehaviour
{
    [Header("References")]
    public Transform middleButton;
    public Transform sideButton;
    public Transform keyboardTransform;
    public WriteToDisplay writeToDisplay;

    [Header("Control Buttons")]
    public Transform topControlButton;
    public Transform bottomControlButton;
    public Transform leftControlButton;
    public Transform rightControlButton;

    [Header("Actual Key Transforms (7 per direction)")]
    public Transform[] topKeyTransforms = new Transform[7];
    public Transform[] bottomKeyTransforms = new Transform[7];
    public Transform[] leftKeyTransforms = new Transform[7];
    public Transform[] rightKeyTransforms = new Transform[7];

    [Header("Letters")]
    public string[] topLetters = new string[7];
    public string[] bottomLetters = new string[7];
    public string[] leftLetters = new string[7];
    public string[] rightLetters = new string[7];

    [Header("Key Groups")]
    public GameObject topKeys;
    public GameObject bottomKeys;
    public GameObject leftKeys;
    public GameObject rightKeys;

    [Header("Line Groups")]
    public GameObject topLines;
    public GameObject bottomLines;
    public GameObject leftLines;
    public GameObject rightLines;

    [Header("Settings")]
    public float requiredDistancePercent = 0.8f;
    public float state2RequiredDistancePercent = 0.8f;
    public float middleButtonActivationDistance = 0.05f;
    public float state2MaxDistancePercent = 1.2f;
    public float state1MaxDistancePercent = 1.2f;

    [Header("Debug")]
    public int currentState = 0;

    private Vector3 firstFixationPoint;
    private Vector3 secondFixationPoint;

    private bool hasFirstFixation = false;
    private bool hasSecondFixation = false;

    private string selectedDirection = "None";
    private Vector3 state1VectorWorld;

    public void OnFixationStartedHandler(Vector3 origin, Vector3 direction, Vector3 fixationPoint)
    {
        if (middleButton == null || sideButton == null || keyboardTransform == null)
        {
            Debug.LogError("Assign all references.");
            return;
        }

        Transform middle = middleButton.Find("ButtonContent") ?? middleButton;
        Vector3 middlePosition = middle.position;

        float distanceToMiddle = Vector3.Distance(fixationPoint, middlePosition);

        if (distanceToMiddle <= middleButtonActivationDistance)
        {
            currentState = 1;
            selectedDirection = "None";

            firstFixationPoint = fixationPoint;
            secondFixationPoint = Vector3.zero;

            hasFirstFixation = true;
            hasSecondFixation = false;

            Debug.Log("Fixation near middle button. Forced transition to state 1.");
            Debug.Log("First fixation reset to: " + firstFixationPoint);

            return;
        }

        if (currentState == 1)
        {
            HandleState1(fixationPoint);
            return;
        }

        if (currentState == 2)
        {
            HandleState2(fixationPoint);
            return;
        }
    }

    private void HandleState1(Vector3 fixationPoint)
    {
        if (!hasFirstFixation)
        {
            firstFixationPoint = fixationPoint;
            hasFirstFixation = true;

            Debug.Log("First fixation point: " + firstFixationPoint);
            return;
        }

        if (!hasSecondFixation)
        {
            secondFixationPoint = fixationPoint;
            hasSecondFixation = true;

            Debug.Log("Second fixation point: " + secondFixationPoint);

            EvaluateState1Movement();
        }
    }

    private void EvaluateState1Movement()
    {
        Transform middle = middleButton.Find("ButtonContent") ?? middleButton;
        Transform side = sideButton.Find("ButtonContent") ?? sideButton;

        Vector3 referenceVectorWorld = side.position - middle.position;
        float referenceDistance = referenceVectorWorld.magnitude;

        Vector3 fixationVectorWorld = secondFixationPoint - firstFixationPoint;
        float fixationDistance = fixationVectorWorld.magnitude;

        float minDistance = referenceDistance * requiredDistancePercent;
        float maxDistance = referenceDistance * state1MaxDistancePercent;

        Debug.Log("Reference distance: " + referenceDistance.ToString("F3"));
        Debug.Log("Min required distance: " + minDistance.ToString("F3"));
        Debug.Log("Max allowed distance: " + maxDistance.ToString("F3"));
        Debug.Log("Fixation distance: " + fixationDistance.ToString("F3"));

        if (fixationDistance < minDistance || fixationDistance > maxDistance)
        {
            Debug.Log("State 1 movement outside allowed range. Stay in state 1.");
            hasSecondFixation = false;
            secondFixationPoint = Vector3.zero;
            return;
        }

        Vector3 fixationVectorLocal = keyboardTransform.InverseTransformDirection(fixationVectorWorld);
        float angle = Mathf.Atan2(fixationVectorLocal.y, fixationVectorLocal.x) * Mathf.Rad2Deg;

        string direction = GetDirectionFromAngle(angle);

        Debug.Log("Angle: " + angle.ToString("F2"));
        Debug.Log("Direction: " + direction);

        if (direction == "None")
        {
            Debug.Log("Invalid direction. Stay in state 1.");
            ResetFixations();
            currentState = 0;
            return;
        }

        selectedDirection = direction;
        state1VectorWorld = fixationVectorWorld;

        ActivateKeyGroup(direction);

        currentState = 2;
        Debug.Log("State changed to 2");

        firstFixationPoint = secondFixationPoint;
        hasFirstFixation = true;
        hasSecondFixation = false;
    }

    private void HandleState2(Vector3 fixationPoint)
    {
        if (!hasSecondFixation)
        {
            secondFixationPoint = fixationPoint;
            hasSecondFixation = true;

            Debug.Log("State 2 new fixation: " + secondFixationPoint);

            EvaluateState2Selection();
        }
    }

    private void EvaluateState2Selection()
    {
        Transform controlButton = GetActiveControlButton(selectedDirection);
        Transform[] activeKeys = GetActiveKeyTransforms(selectedDirection);

        if (controlButton == null || activeKeys == null || activeKeys.Length != 7)
        {
            Debug.LogError("Assign control button and exactly 7 key transforms for each direction.");
            ResetFixations();
            currentState = 0;
            return;
        }

        if (!AreKeyTransformsValid(activeKeys, selectedDirection))
        {
            ResetFixations();
            currentState = 0;
            return;
        }

        Transform control = controlButton.Find("ButtonContent") ?? controlButton;
        Transform firstAssignedKey = activeKeys[0].Find("ButtonContent") ?? activeKeys[0];

        Vector3 state2VectorWorld = secondFixationPoint - firstFixationPoint;
        float state2VectorDistance = state2VectorWorld.magnitude;

        float controlToKeyDistance = Vector3.Distance(control.position, firstAssignedKey.position);
        float minState2Distance = controlToKeyDistance * state2RequiredDistancePercent;
        float maxState2Distance = controlToKeyDistance * state2MaxDistancePercent;

        Debug.Log("State 2 control-to-key distance: " + controlToKeyDistance.ToString("F3"));
        Debug.Log("State 2 min required distance: " + minState2Distance.ToString("F3"));
        Debug.Log("State 2 max allowed distance: " + maxState2Distance.ToString("F3"));
        Debug.Log("State 2 fixation distance: " + state2VectorDistance.ToString("F3"));

        if (state2VectorDistance < minState2Distance || state2VectorDistance > maxState2Distance)
        {
            Debug.Log("State 2 movement outside allowed distance range. Stay in state 2.");
            hasSecondFixation = false;
            secondFixationPoint = Vector3.zero;
            return;
        }

        Vector3 vector1Local = keyboardTransform.InverseTransformDirection(state1VectorWorld);
        Vector3 vector2Local = keyboardTransform.InverseTransformDirection(state2VectorWorld);

        float fixationRelativeAngle = Vector3.SignedAngle(vector1Local, vector2Local, Vector3.forward);

        int selectedIndex = GetNearestKeyIndexFromActualKeys(
            controlButton,
            activeKeys,
            vector1Local,
            fixationRelativeAngle
        );

        string selectedLetter = GetLetterForDirectionAndIndex(selectedDirection, selectedIndex);

        Debug.Log("Selected direction: " + selectedDirection);
        Debug.Log("Fixation relative angle: " + fixationRelativeAngle.ToString("F2"));
        Debug.Log("Selected key index: " + selectedIndex);
        Debug.Log("Selected letter: " + selectedLetter);

        if (writeToDisplay != null && !string.IsNullOrEmpty(selectedLetter))
        {
            if (selectedLetter == "-")
            {
                writeToDisplay.Backspace();
            }
            else
            {
                writeToDisplay.SetLetter(selectedLetter);
                writeToDisplay.Write();
            }
        }
        else
        {
            Debug.LogWarning("WriteToDisplay is not assigned or selected letter is empty.");
        }

        ResetFixations();
        currentState = 0;
        Debug.Log("Returned to state 0");
    }

    private int GetNearestKeyIndexFromActualKeys(
        Transform controlButton,
        Transform[] keyTransforms,
        Vector3 vector1Local,
        float fixationRelativeAngle)
    {
        Transform control = controlButton.Find("ButtonContent") ?? controlButton;
        Vector3 controlWorld = control.position;

        float bestDifference = float.MaxValue;
        int bestIndex = 0;

        for (int i = 0; i < keyTransforms.Length; i++)
        {
            Transform rawKey = keyTransforms[i];

            if (rawKey == null)
            {
                Debug.LogWarning("Key transform at index " + i + " is null. Skipping.");
                continue;
            }

            Transform key = rawKey.Find("ButtonContent") ?? rawKey;

            Vector3 keyVectorWorld = key.position - controlWorld;
            Vector3 keyVectorLocal = keyboardTransform.InverseTransformDirection(keyVectorWorld);

            float keyAngle = Vector3.SignedAngle(vector1Local, keyVectorLocal, Vector3.forward);
            float diff = Mathf.Abs(Mathf.DeltaAngle(fixationRelativeAngle, keyAngle));

            Debug.Log(
                "Key " + i +
                " [" + rawKey.name + "] angle: " + keyAngle.ToString("F2") +
                " diff: " + diff.ToString("F2")
            );

            if (diff < bestDifference)
            {
                bestDifference = diff;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    private bool AreKeyTransformsValid(Transform[] keys, string direction)
    {
        if (keys == null || keys.Length != 7)
        {
            Debug.LogError("Key transform array for direction " + direction + " must contain exactly 7 entries.");
            return false;
        }

        for (int i = 0; i < keys.Length; i++)
        {
            if (keys[i] == null)
            {
                Debug.LogError("Missing key transform at index " + i + " for direction " + direction + ".");
                return false;
            }
        }

        return true;
    }

    private string GetLetterForDirectionAndIndex(string direction, int index)
    {
        string[] letters = null;

        if (direction == "Top") letters = topLetters;
        if (direction == "Bottom") letters = bottomLetters;
        if (direction == "Left") letters = leftLetters;
        if (direction == "Right") letters = rightLetters;

        if (letters == null || letters.Length != 7)
        {
            Debug.LogError("Letter array for direction " + direction + " must contain exactly 7 entries.");
            return "";
        }

        if (index < 0 || index >= letters.Length)
        {
            Debug.LogError("Selected index out of range: " + index);
            return "";
        }

        return letters[index];
    }

    private Transform GetActiveControlButton(string direction)
    {
        if (direction == "Top") return topControlButton;
        if (direction == "Bottom") return bottomControlButton;
        if (direction == "Left") return leftControlButton;
        if (direction == "Right") return rightControlButton;

        return null;
    }

    private Transform[] GetActiveKeyTransforms(string direction)
    {
        if (direction == "Top") return topKeyTransforms;
        if (direction == "Bottom") return bottomKeyTransforms;
        if (direction == "Left") return leftKeyTransforms;
        if (direction == "Right") return rightKeyTransforms;

        return null;
    }

    private string GetDirectionFromAngle(float angle)
    {
        if (angle >= -45f && angle <= 45f)
        {
            return "Right";
        }

        if (angle > 45f && angle <= 135f)
        {
            return "Top";
        }

        if (angle < -45f && angle >= -135f)
        {
            return "Bottom";
        }

        if ((angle > 135f && angle <= 180f) || (angle >= -180f && angle < -135f))
        {
            return "Left";
        }

        return "None";
    }

    private void ActivateKeyGroup(string direction)
    {
        DeactivateAllKeyGroups();
        DeactivateAllLineGroups();

        if (direction == "Top")
        {
            if (topKeys != null) topKeys.SetActive(true);
            if (topLines != null) topLines.SetActive(true);
            Debug.Log("Top keys and lines activated");
        }
        else if (direction == "Bottom")
        {
            if (bottomKeys != null) bottomKeys.SetActive(true);
            if (bottomLines != null) bottomLines.SetActive(true);
            Debug.Log("Bottom keys and lines activated");
        }
        else if (direction == "Left")
        {
            if (leftKeys != null) leftKeys.SetActive(true);
            if (leftLines != null) leftLines.SetActive(true);
            Debug.Log("Left keys and lines activated");
        }
        else if (direction == "Right")
        {
            if (rightKeys != null) rightKeys.SetActive(true);
            if (rightLines != null) rightLines.SetActive(true);
            Debug.Log("Right keys and lines activated");
        }
    }

    private void DeactivateAllKeyGroups()
    {
        if (topKeys != null) topKeys.SetActive(false);
        if (bottomKeys != null) bottomKeys.SetActive(false);
        if (leftKeys != null) leftKeys.SetActive(false);
        if (rightKeys != null) rightKeys.SetActive(false);
    }

    private void DeactivateAllLineGroups()
    {
        if (topLines != null) topLines.SetActive(false);
        if (bottomLines != null) bottomLines.SetActive(false);
        if (leftLines != null) leftLines.SetActive(false);
        if (rightLines != null) rightLines.SetActive(false);
    }

    public void ResetStateMachine()
    {
        currentState = 0;
        selectedDirection = "None";
        ResetFixations();
        DeactivateAllKeyGroups();
        DeactivateAllLineGroups();
        Debug.Log("Reset to state 0");
    }

    private void ResetFixations()
    {
        hasFirstFixation = false;
        hasSecondFixation = false;
        firstFixationPoint = Vector3.zero;
        secondFixationPoint = Vector3.zero;
    }
}
