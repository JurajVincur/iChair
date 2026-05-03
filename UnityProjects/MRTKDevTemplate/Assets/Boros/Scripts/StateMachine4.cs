using UnityEngine;

public class StateMachine4 : MonoBehaviour
{
    [Header("References")]
    public Transform middleButton;
    public Transform sideButton;
    public Transform keyboardTransform;
    public WriteToDisplay writeToDisplay;
    public AudioSource writerAudioSource;

    [Header("Control Buttons")]
    public Transform topControlButton;
    public Transform topRightControlButton;
    public Transform rightControlButton;
    public Transform bottomRightControlButton;
    public Transform bottomControlButton;
    public Transform bottomLeftControlButton;
    public Transform leftControlButton;

    [Header("Control Button Labels")]
    public GameObject topControlLabel;
    public GameObject topRightControlLabel;
    public GameObject rightControlLabel;
    public GameObject bottomRightControlLabel;
    public GameObject bottomControlLabel;
    public GameObject bottomLeftControlLabel;
    public GameObject leftControlLabel;

    [Header("Control Button Direction Lines")]
    public GameObject topControlDirectionLine;
    public GameObject topRightControlDirectionLine;
    public GameObject rightControlDirectionLine;
    public GameObject bottomRightControlDirectionLine;
    public GameObject bottomControlDirectionLine;
    public GameObject bottomLeftControlDirectionLine;
    public GameObject leftControlDirectionLine;

    [Header("Actual Key Transforms (4 per direction)")]
    public Transform[] topKeyTransforms = new Transform[4];
    public Transform[] topRightKeyTransforms = new Transform[4];
    public Transform[] rightKeyTransforms = new Transform[4];
    public Transform[] bottomRightKeyTransforms = new Transform[4];
    public Transform[] bottomKeyTransforms = new Transform[4];
    public Transform[] bottomLeftKeyTransforms = new Transform[4];
    public Transform[] leftKeyTransforms = new Transform[4];

    [Header("Letters")]
    public string[] topLetters = new string[4];
    public string[] topRightLetters = new string[4];
    public string[] rightLetters = new string[4];
    public string[] bottomRightLetters = new string[4];
    public string[] bottomLetters = new string[4];
    public string[] bottomLeftLetters = new string[4];
    public string[] leftLetters = new string[4];

    [Header("Key Groups")]
    public GameObject topKeys;
    public GameObject topRightKeys;
    public GameObject rightKeys;
    public GameObject bottomRightKeys;
    public GameObject bottomKeys;
    public GameObject bottomLeftKeys;
    public GameObject leftKeys;

    [Header("Line Groups")]
    public GameObject topLines;
    public GameObject topRightLines;
    public GameObject rightLines;
    public GameObject bottomRightLines;
    public GameObject bottomLines;
    public GameObject bottomLeftLines;
    public GameObject leftLines;


    [Header("Suggest Buttons")]
    public GameObject suggestButton1;
    public GameObject suggestButton2;
    public GameObject suggestButton3;
    public GameObject suggestButton4;
    public GameObject suggestButtonClear;

    [Header("Settings")]
    public float requiredDistancePercent = 0.5f;
    public float state2RequiredDistancePercent = 0.5f;
    public float middleButtonActivationDistance = 0.1f;
    public float maxButtonActivationDistance = 1f;
    public float state2MaxDistancePercent = 1.5f;
    public float state1MaxDistancePercent = 1.5f;
    public float maxDirectionMatchAngle = 50f;

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

        if (currentState == 0)
        {
            DeactivateAllKeyGroups();
            DeactivateAllLineGroups();
            DeactivateAllControlButtons();
            
        }

        Transform middle = middleButton.Find("ButtonContent") ?? middleButton;
        Vector3 middlePosition = middle.position;

        float distanceToMiddle = Vector3.Distance(fixationPoint, middlePosition);

        if (distanceToMiddle >= maxButtonActivationDistance)
        {
            currentState = 0;
            selectedDirection = "None";

            ResetFixations();

            DeactivateAllKeyGroups();
            DeactivateAllLineGroups();
            DeactivateAllControlButtons();

            return;
        }

        if (distanceToMiddle <= middleButtonActivationDistance)
        {
            currentState = 1;
            selectedDirection = "None";

            firstFixationPoint = fixationPoint;
            secondFixationPoint = Vector3.zero;

            hasFirstFixation = true;
            hasSecondFixation = false;

            DeactivateAllKeyGroups();
            DeactivateAllLineGroups();
            DeactivateAllSuggestButtons();
            ActivateAllControlButtons();

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
            return;
        }

        if (!hasSecondFixation)
        {
            secondFixationPoint = fixationPoint;
            hasSecondFixation = true;

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

        if (fixationDistance < minDistance || fixationDistance > maxDistance)
        {
            hasSecondFixation = false;
            secondFixationPoint = Vector3.zero;
            return;
        }

        string direction = GetDirectionFromActualButtons(fixationVectorWorld);

        if (direction == "None")
        {
            ResetFixations();
            currentState = 0;
            return;
        }

        selectedDirection = direction;
        state1VectorWorld = fixationVectorWorld;

        ActivateKeyGroup(direction);
        ActivateOnlySelectedControlButton(direction);

        currentState = 2;

        firstFixationPoint = secondFixationPoint;
        hasFirstFixation = true;
        hasSecondFixation = false;
    }

    private string GetDirectionFromActualButtons(Vector3 fixationVectorWorld)
    {
        Transform middle = middleButton.Find("ButtonContent") ?? middleButton;
        Vector3 middleWorld = middle.position;

        Vector3 fixationVectorLocal =
            keyboardTransform.InverseTransformDirection(fixationVectorWorld).normalized;

        float bestAngle = float.MaxValue;
        string bestDirection = "None";

        CheckDirection(topControlButton, "Top");
        CheckDirection(topRightControlButton, "TopRight");
        CheckDirection(rightControlButton, "Right");
        CheckDirection(bottomRightControlButton, "BottomRight");
        CheckDirection(bottomControlButton, "Bottom");
        CheckDirection(bottomLeftControlButton, "BottomLeft");
        CheckDirection(leftControlButton, "Left");

        if (bestAngle > maxDirectionMatchAngle)
        {
            return "None";
        }

        return bestDirection;

        void CheckDirection(Transform controlButton, string directionName)
        {
            if (controlButton == null) return;

            Transform control =
                controlButton.Find("ButtonContent") ?? controlButton;

            Vector3 controlVectorWorld =
                control.position - middleWorld;

            Vector3 controlVectorLocal =
                keyboardTransform.InverseTransformDirection(controlVectorWorld).normalized;

            float angle =
                Vector3.Angle(fixationVectorLocal, controlVectorLocal);

            if (angle < bestAngle)
            {
                bestAngle = angle;
                bestDirection = directionName;
            }
        }
    }

    private void HandleState2(Vector3 fixationPoint)
    {
        if (!hasSecondFixation)
        {
            secondFixationPoint = fixationPoint;
            hasSecondFixation = true;

            EvaluateState2Selection();
        }
    }

    private void EvaluateState2Selection()
    {
        Transform controlButton =
            GetActiveControlButton(selectedDirection);

        Transform[] activeKeys =
            GetActiveKeyTransforms(selectedDirection);

        if (controlButton == null || activeKeys == null || activeKeys.Length != 4)
        {
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

        Vector3 state2VectorWorld =
            secondFixationPoint - firstFixationPoint;

        float state2VectorDistance =
            state2VectorWorld.magnitude;

        Vector3 vector1Local =
            keyboardTransform.InverseTransformDirection(state1VectorWorld);

        Vector3 vector2Local =
            keyboardTransform.InverseTransformDirection(state2VectorWorld);

        float fixationRelativeAngle =
            Vector3.SignedAngle(vector1Local, vector2Local, Vector3.forward);

        int selectedIndex =
            GetNearestKeyIndexFromActualKeys(
                controlButton,
                activeKeys,
                vector1Local,
                fixationRelativeAngle
            );

        Transform selectedKey =
            activeKeys[selectedIndex].Find("ButtonContent") ?? activeKeys[selectedIndex];

        float firstFixationToSelectedKeyDistance =
            Vector3.Distance(firstFixationPoint, selectedKey.position);

        float minState2Distance =
            firstFixationToSelectedKeyDistance * state2RequiredDistancePercent;

        float maxState2Distance =
            firstFixationToSelectedKeyDistance * state2MaxDistancePercent;

        if (state2VectorDistance < minState2Distance ||
            state2VectorDistance > maxState2Distance)
        {
            hasSecondFixation = false;
            secondFixationPoint = Vector3.zero;
            return;
        }

        string selectedLetter =
            GetLetterForDirectionAndIndex(selectedDirection, selectedIndex);

        if (writeToDisplay != null && !string.IsNullOrEmpty(selectedLetter))
        {
            if (selectedLetter == "-")
            {
                writeToDisplay.Backspace();
                writerAudioSource.Play();
            }
            else
            {
                writeToDisplay.SetLetter(selectedLetter);
                writeToDisplay.Write();
                writerAudioSource.Play();
            }
        }

        ResetFixations();
        currentState = 0;
        DeactivateAllKeyGroups();
        DeactivateAllLineGroups();
        DeactivateAllControlButtons();
    }

    private int GetNearestKeyIndexFromActualKeys(
        Transform controlButton,
        Transform[] keyTransforms,
        Vector3 vector1Local,
        float fixationRelativeAngle)
    {
        Transform control =
            controlButton.Find("ButtonContent") ?? controlButton;

        Vector3 controlWorld = control.position;

        float bestDifference = float.MaxValue;
        int bestIndex = 0;

        for (int i = 0; i < keyTransforms.Length; i++)
        {
            Transform key =
                keyTransforms[i].Find("ButtonContent") ?? keyTransforms[i];

            Vector3 keyVectorWorld =
                key.position - controlWorld;

            Vector3 keyVectorLocal =
                keyboardTransform.InverseTransformDirection(keyVectorWorld);

            float keyAngle =
                Vector3.SignedAngle(vector1Local, keyVectorLocal, Vector3.forward);

            float diff =
                Mathf.Abs(Mathf.DeltaAngle(fixationRelativeAngle, keyAngle));

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
        if (keys == null || keys.Length != 4)
            return false;

        for (int i = 0; i < keys.Length; i++)
        {
            if (keys[i] == null)
                return false;
        }

        return true;
    }

    private string GetLetterForDirectionAndIndex(string direction, int index)
    {
        string[] letters = null;

        if (direction == "Top") letters = topLetters;
        if (direction == "TopRight") letters = topRightLetters;
        if (direction == "Right") letters = rightLetters;
        if (direction == "BottomRight") letters = bottomRightLetters;
        if (direction == "Bottom") letters = bottomLetters;
        if (direction == "BottomLeft") letters = bottomLeftLetters;
        if (direction == "Left") letters = leftLetters;

        if (letters == null || index < 0 || index >= letters.Length)
            return "";

        return letters[index];
    }

    private Transform GetActiveControlButton(string direction)
    {
        if (direction == "Top") return topControlButton;
        if (direction == "TopRight") return topRightControlButton;
        if (direction == "Right") return rightControlButton;
        if (direction == "BottomRight") return bottomRightControlButton;
        if (direction == "Bottom") return bottomControlButton;
        if (direction == "BottomLeft") return bottomLeftControlButton;
        if (direction == "Left") return leftControlButton;

        return null;
    }

    private Transform[] GetActiveKeyTransforms(string direction)
    {
        if (direction == "Top") return topKeyTransforms;
        if (direction == "TopRight") return topRightKeyTransforms;
        if (direction == "Right") return rightKeyTransforms;
        if (direction == "BottomRight") return bottomRightKeyTransforms;
        if (direction == "Bottom") return bottomKeyTransforms;
        if (direction == "BottomLeft") return bottomLeftKeyTransforms;
        if (direction == "Left") return leftKeyTransforms;

        return null;
    }

    private void ActivateKeyGroup(string direction)
    {
        DeactivateAllKeyGroups();
        DeactivateAllLineGroups();

        if (direction == "Top")
        {
            if (topKeys) topKeys.SetActive(true);
            if (topLines) topLines.SetActive(true);
        }
        else if (direction == "TopRight")
        {
            if (topRightKeys) topRightKeys.SetActive(true);
            if (topRightLines) topRightLines.SetActive(true);
        }
        else if (direction == "Right")
        {
            if (rightKeys) rightKeys.SetActive(true);
            if (rightLines) rightLines.SetActive(true);
        }
        else if (direction == "BottomRight")
        {
            if (bottomRightKeys) bottomRightKeys.SetActive(true);
            if (bottomRightLines) bottomRightLines.SetActive(true);
        }
        else if (direction == "Bottom")
        {
            if (bottomKeys) bottomKeys.SetActive(true);
            if (bottomLines) bottomLines.SetActive(true);
        }
        else if (direction == "BottomLeft")
        {
            if (bottomLeftKeys) bottomLeftKeys.SetActive(true);
            if (bottomLeftLines) bottomLeftLines.SetActive(true);
        }
        else if (direction == "Left")
        {
            if (leftKeys) leftKeys.SetActive(true);
            if (leftLines) leftLines.SetActive(true);
        }
    }

    private void DeactivateAllKeyGroups()
    {
        if (topKeys) topKeys.SetActive(false);
        if (topRightKeys) topRightKeys.SetActive(false);
        if (rightKeys) rightKeys.SetActive(false);
        if (bottomRightKeys) bottomRightKeys.SetActive(false);
        if (bottomKeys) bottomKeys.SetActive(false);
        if (bottomLeftKeys) bottomLeftKeys.SetActive(false);
        if (leftKeys) leftKeys.SetActive(false);
    }

    private void DeactivateAllLineGroups()
    {
        if (topLines) topLines.SetActive(false);
        if (topRightLines) topRightLines.SetActive(false);
        if (rightLines) rightLines.SetActive(false);
        if (bottomRightLines) bottomRightLines.SetActive(false);
        if (bottomLines) bottomLines.SetActive(false);
        if (bottomLeftLines) bottomLeftLines.SetActive(false);
        if (leftLines) leftLines.SetActive(false);
    }

    private void DeactivateAllSuggestButtons()
    {
        if (suggestButton1) suggestButton1.SetActive(false);
        if (suggestButton2) suggestButton2.SetActive(false);
        if (suggestButton3) suggestButton3.SetActive(false);
        if (suggestButton4) suggestButton4.SetActive(false);
        if (suggestButtonClear) suggestButtonClear.SetActive(false);

    }

    private void DeactivateAllControlButtons()
    {
        if (topControlButton) topControlButton.gameObject.SetActive(false);
        if (topRightControlButton) topRightControlButton.gameObject.SetActive(false);
        if (rightControlButton) rightControlButton.gameObject.SetActive(false);
        if (bottomRightControlButton) bottomRightControlButton.gameObject.SetActive(false);
        if (bottomControlButton) bottomControlButton.gameObject.SetActive(false);
        if (bottomLeftControlButton) bottomLeftControlButton.gameObject.SetActive(false);
        if (leftControlButton) leftControlButton.gameObject.SetActive(false);
        if (topControlLabel) topControlLabel.SetActive(false);
        if (topRightControlLabel) topRightControlLabel.SetActive(false);
        if (rightControlLabel) rightControlLabel.SetActive(false);
        if (bottomRightControlLabel) bottomRightControlLabel.SetActive(false);
        if (bottomControlLabel) bottomControlLabel.SetActive(false);
        if (bottomLeftControlLabel) bottomLeftControlLabel.SetActive(false);
        if (leftControlLabel) leftControlLabel.SetActive(false);
        if (topControlDirectionLine) topControlDirectionLine.SetActive(false);
        if (topRightControlDirectionLine) topRightControlDirectionLine.SetActive(false);
        if (rightControlDirectionLine) rightControlDirectionLine.SetActive(false);
        if (bottomRightControlDirectionLine) bottomRightControlDirectionLine.SetActive(false);
        if (bottomControlDirectionLine) bottomControlDirectionLine.SetActive(false);
        if (bottomLeftControlDirectionLine) bottomLeftControlDirectionLine.SetActive(false);
        if (leftControlDirectionLine) leftControlDirectionLine.SetActive(false);
    }

    private void ActivateAllControlButtons()
    {
        if (topControlButton) topControlButton.gameObject.SetActive(true);
        if (topRightControlButton) topRightControlButton.gameObject.SetActive(true);
        if (rightControlButton) rightControlButton.gameObject.SetActive(true);
        if (bottomRightControlButton) bottomRightControlButton.gameObject.SetActive(true);
        if (bottomControlButton) bottomControlButton.gameObject.SetActive(true);
        if (bottomLeftControlButton) bottomLeftControlButton.gameObject.SetActive(true);
        if (leftControlButton) leftControlButton.gameObject.SetActive(true);
        if (topControlLabel) topControlLabel.SetActive(true);
        if (topRightControlLabel) topRightControlLabel.SetActive(true);
        if (rightControlLabel) rightControlLabel.SetActive(true);
        if (bottomRightControlLabel) bottomRightControlLabel.SetActive(true);
        if (bottomControlLabel) bottomControlLabel.SetActive(true);
        if (bottomLeftControlLabel) bottomLeftControlLabel.SetActive(true);
        if (leftControlLabel) leftControlLabel.SetActive(true);
        if (topControlDirectionLine) topControlDirectionLine.SetActive(true);
        if (topRightControlDirectionLine) topRightControlDirectionLine.SetActive(true);
        if (rightControlDirectionLine) rightControlDirectionLine.SetActive(true);
        if (bottomRightControlDirectionLine) bottomRightControlDirectionLine.SetActive(true);
        if (bottomControlDirectionLine) bottomControlDirectionLine.SetActive(true);
        if (bottomLeftControlDirectionLine) bottomLeftControlDirectionLine.SetActive(true);
        if (leftControlDirectionLine) leftControlDirectionLine.SetActive(true);
    }

    private void ActivateOnlySelectedControlButton(string direction)
    {
        DeactivateAllControlButtons();

        if (direction == "Top")
        {
            if (topControlButton) topControlButton.gameObject.SetActive(true);
            if (topControlLabel) topControlLabel.SetActive(true);
            if (topControlDirectionLine) topControlDirectionLine.SetActive(true);
        }
        else if (direction == "TopRight")
        {
            if (topRightControlButton) topRightControlButton.gameObject.SetActive(true);
            if (topRightControlLabel) topRightControlLabel.SetActive(true);
            if (topRightControlDirectionLine) topRightControlDirectionLine.SetActive(true);
        }
        else if (direction == "Right")
        {
            if (rightControlButton) rightControlButton.gameObject.SetActive(true);
            if (rightControlLabel) rightControlLabel.SetActive(true);
            if (rightControlDirectionLine) rightControlDirectionLine.SetActive(true);
        }
        else if (direction == "BottomRight")
        {
            if (bottomRightControlButton) bottomRightControlButton.gameObject.SetActive(true);
            if (bottomRightControlLabel) bottomRightControlLabel.SetActive(true);
            if (bottomRightControlDirectionLine) bottomRightControlDirectionLine.SetActive(true);
        }
        else if (direction == "Bottom")
        {
            if (bottomControlButton) bottomControlButton.gameObject.SetActive(true);
            if (bottomControlLabel) bottomControlLabel.SetActive(true);
            if (bottomControlDirectionLine) bottomControlDirectionLine.SetActive(true);
        }
        else if (direction == "BottomLeft")
        {
            if (bottomLeftControlButton) bottomLeftControlButton.gameObject.SetActive(true);
            if (bottomLeftControlLabel) bottomLeftControlLabel.SetActive(true);
            if (bottomLeftControlDirectionLine) bottomLeftControlDirectionLine.SetActive(true);
        }
        else if (direction == "Left")
        {
            if (leftControlButton) leftControlButton.gameObject.SetActive(true);
            if (leftControlLabel) leftControlLabel.SetActive(true);
            if (leftControlDirectionLine) leftControlDirectionLine.SetActive(true);
        }
    }

    public void ResetStateMachine()
    {
        currentState = 0;
        selectedDirection = "None";
        ResetFixations();
        DeactivateAllKeyGroups();
        DeactivateAllLineGroups();
    }

    private void ResetFixations()
    {
        hasFirstFixation = false;
        hasSecondFixation = false;
        firstFixationPoint = Vector3.zero;
        secondFixationPoint = Vector3.zero;
    }
}
