using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;

public class ProgramManager : MonoBehaviour
{
    public EmissionController emissionController;
    public QuestionnaireManager questionnaireManager;
    public TextMeshProUGUI guideText;
    public Image timeFeedback;
    public GameObject timeFeedbackUI;
    public GameObject finishUI;

    private Phase currentPhase = Phase.RightOnLeftOff;
    private State currentState = State.Waiting;
    private float timer = 0f;
    private float phaseDuration = 10f;
    private float coolDownDuration = 40f;

    void Start()
    {
        guideText.text = "Please put both hands inside the heaters.";
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Waiting:
                HandleWaitingState();
                break;
            case State.CoolDown:
                HandleCoolDownState();
                break;
            case State.Questionnaire:
                HandleQuestionnaireState();
                break;
        }
    }

    private void HandleWaitingState()
    {
        if (emissionController.AreBothHandsIn())
        {
            timer += Time.deltaTime;
            timeFeedback.fillAmount = timer / phaseDuration;
            guideText.text = "Do not take your hands out until the time passes.";

            if (timer >= phaseDuration)
            {
                timer = 0f;
                currentState = State.Questionnaire;
                guideText.text = "Take your hands out and pick up the controller.";
            }
        }
    }

    private void HandleCoolDownState()
    {
        timer += Time.deltaTime;
        timeFeedback.fillAmount = timer / coolDownDuration;

        if (timer >= coolDownDuration)
        {
            timer = 0f;
            currentPhase++;
            if (currentPhase == Phase.Finished)
            {
                questionnaireManager.ResetQuestionnaire();
                finishUI.SetActive(true);
                timeFeedbackUI.SetActive(false);
            }
            else
            {
                guideText.text = "Please put both hands inside the heaters.";
                currentState = State.Waiting;
                questionnaireManager.ResetQuestionnaire();
            }
        }
    }

    private void HandleQuestionnaireState()
    {
        if (emissionController.AreBothHandsOut())
        {
            questionnaireManager.ShowQuestionnaire();
            currentState = State.CoolDown;
        }
    }

    public Phase GetCurrentPhase()
    {
        return currentPhase;
    }
}

public enum Phase
{
    RightOnLeftOff,
    RightOffLeftOn,
    RightOnLeftOn,
    Finished
}

public enum State
{
    Waiting,
    CoolDown,
    Questionnaire
}