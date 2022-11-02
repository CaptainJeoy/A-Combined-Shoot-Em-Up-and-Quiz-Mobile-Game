using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
class ChoiceOption
{
    public Button Button;
    public Image ImageBut;
    public Text OptionText;

    [HideInInspector] public bool IsSelected;
}

public class QuizManager : MonoBehaviour
{
    enum QuizType
    {
        MultipleChoice,
        Theory
    }

    [SerializeField] private Audio audioObjFail;
    [SerializeField] private Audio audioObjWin;

    [SerializeField] private GameObject HudCanvas;

    [SerializeField] private GameObject QuizPanel, AnswerOption, AnswerInput, PowerUpPanel;

    [SerializeField] private Color defaultButtonColour, selectedButtonColour;

    [Space]
    [SerializeField] private Color defaultBoxColour, correctColour, failedColour;

    [SerializeField] private float lerpSpeed = 3f, delayForChoice = 3f, /*delayForInput = 7f,*/ delayforDisplay = 1.5f;

    [SerializeField] private Image Loader, BoxImage;

    [SerializeField] private Text QuestionText;

    [SerializeField] private ChoiceOption[] choiceOptions;

    [Space]
    [SerializeField] private QuizOptionSO[] quizOptionSOs;

    [Space]
    [SerializeField] private QuizInputSO[] quizInputSOs;

    private Vector3 qpCurrScale, qpDesiScale;

    private float quizTimeSpan, delayForQuiz = 0f, displayAnswerSpan;

    private QuizType quizType;

    private bool IsPanelOpened = false, IsAnyButtonSelected = false, timeElasped = false, isSubmitClicked = false;

    private QuizOptionSO quizOptionSO;

    private ChoiceOption choiceOption, correctChoiceOption;

    private HorizontalLayoutGroup layoutGroup;


    private void Start()
    {
        qpCurrScale = qpDesiScale = Vector3.zero;
        layoutGroup = AnswerOption.GetComponent<HorizontalLayoutGroup>();
        layoutGroup.enabled = false;

        displayAnswerSpan = delayforDisplay;
    }

    private void Update()
    {
        qpCurrScale = Vector3.Lerp(qpCurrScale, qpDesiScale, Time.unscaledDeltaTime * lerpSpeed);
        QuizPanel.transform.localScale = qpCurrScale;

        
        if (IsPanelOpened && !isSubmitClicked)
            Loader.fillAmount = Mathf.Clamp01(((delayForQuiz += Time.unscaledDeltaTime) / quizTimeSpan));

        
        if (IsPanelOpened && Loader.fillAmount >= 1f || isSubmitClicked)
        {
            timeElasped = true;

            ScoreQuiz();
        }
    }

    private void ScoreQuiz()
    {
        switch (quizType)
        {
            case QuizType.MultipleChoice:
                if (CheckIfOptionSelectedIsCorrect())
                {
                    //audioObjWin.PlayGun();
                    CorrectAnswerDisplay();
                    displayAnswerSpan -= Time.unscaledDeltaTime;

                    if (displayAnswerSpan <= 0f)
                    {
                        displayAnswerSpan = delayforDisplay;

                        ClosePanel();
                        GameManager.Instance.RandomSpawnPowerUp();
                    }
                }
                else
                {
                    //audioObjFail.PlayGun();
                    FailedAnswerDisplay();
                    displayAnswerSpan -= Time.unscaledDeltaTime;

                    if (displayAnswerSpan <= 0f)
                    {
                        displayAnswerSpan = delayforDisplay;

                        ClosePanel();
                    }
                }
                break;
            case QuizType.Theory:
                ClosePanel();
                break;
        }
    }

    public void SumbitButton()
    {
        isSubmitClicked = true;
    }

    public void OpenPanel()
    {
        /*
        if (Random.Range(0, 5) >= 4)
        {
            quizType = QuizType.Theory;
            AnswerOption.SetActive(false);
            AnswerInput.SetActive(true);

            quizTimeSpan = delayForInput;
        }
        else
        {
            quizType = QuizType.MultipleChoice;
            AnswerOption.SetActive(true);
            AnswerInput.SetActive(false);
            RandomizeButtons();

            quizTimeSpan = delayForChoice;
        }
        */

        GameManager.Instance.DeactivateQuizButton();
        AudioSingle.Instance.Play();

        quizType = QuizType.MultipleChoice;
        AnswerOption.SetActive(true);
        AnswerInput.SetActive(false);
        RandomizeButtons();

        quizTimeSpan = delayForChoice;

        LoadQuiz();

        QuizPanel.SetActive(true);
        HudCanvas.SetActive(false);
        PowerUpPanel.SetActive(false);

        qpDesiScale = Vector3.one;

        IsPanelOpened = true;
        displayAnswerSpan = delayforDisplay;
        isSubmitClicked = false;

        TimeManager.Instance.FreezeTime();
    }

    private void ClosePanel()
    {
        qpDesiScale = Vector3.zero;

        IsPanelOpened = false;

        layoutGroup.enabled = false;

        StartCoroutine(ClosingPanelProcess());
    }

    IEnumerator ClosingPanelProcess()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        Loader.fillAmount = 0f;
        delayForQuiz = 0f;
        ResetAllButtons();
        timeElasped = false;
        GameManager.Instance.ResetQuizCounter();
        QuizPanel.SetActive(false);
        HudCanvas.SetActive(true);
        PowerUpPanel.SetActive(true);
        isSubmitClicked = false;

        TimeManager.Instance.UnFreezeTime();
    }

    private void LoadQuiz()
    {
        switch (quizType)
        {
            case QuizType.MultipleChoice:
                SetRandomMultipleChoiceQuiz();
                break;
            case QuizType.Theory:
                SetRandomTheoryQuiz();
                break;
        }
    }

    private void SetRandomMultipleChoiceQuiz()
    {
        quizOptionSO = quizOptionSOs[Random.Range(0, quizOptionSOs.Length)];

        QuestionText.text = quizOptionSO.Question;
        quizOptionSO.SetRandomizedFalseOptions();

        for (int i = 0; i < choiceOptions.Length; i++)
        {
            choiceOptions[i].OptionText.text = quizOptionSO.choices[i].choice;
        }
    }

    private void SetRandomTheoryQuiz()
    {
        QuizInputSO quizInputSO = quizInputSOs[Random.Range(0, quizInputSOs.Length)];

        QuestionText.text = quizInputSO.Question;
    }

    private void RandomizeButtons()
    {
        Button[] gameObjects = AnswerOption.GetComponentsInChildren<Button>();

        AnswerOption.transform.DetachChildren();

        int skipNum = 10000;

        while (AnswerOption.transform.childCount < 3)
        {
            int rand = GenerateRandomNumberBetweenWithIgnoreNum(0, gameObjects.Length, skipNum);

            gameObjects[rand].transform.SetParent(AnswerOption.transform);
            gameObjects[rand].transform.localScale = Vector3.one;

            skipNum = rand;
        }

        layoutGroup.enabled = true;
    }

    private int GenerateRandomNumberBetweenWithIgnoreNum(int Lower, int Upper, int SkipNumber)
    {
        System.Collections.Generic.List<int> ExtractedList = new System.Collections.Generic.List<int>();

        for (int i = Lower; i < Upper; i++)
        {
            ExtractedList.Add(i);
        }

        if (ExtractedList.Contains(SkipNumber))
            ExtractedList.Remove(SkipNumber);

        return ExtractedList[Random.Range(0, ExtractedList.Count)];
    }

    private void ResetAllButtons()
    {
        for (int i = 0; i < choiceOptions.Length; i++)
        {
            choiceOptions[i].ImageBut.color = defaultButtonColour;
            choiceOptions[i].OptionText.color = Color.black;
            choiceOptions[i].IsSelected = false;
        }

        IsAnyButtonSelected = false;
        BoxImage.color = defaultBoxColour;
    }

    public void SelectButton(Button but)
    {
        if (timeElasped)
            return;

        IsAnyButtonSelected = true;

        but.GetComponent<Image>().color = selectedButtonColour;
        but.GetComponentInChildren<Text>().color = Color.white;

        AudioSingle.Instance.Play();

        for (int i = 0; i < choiceOptions.Length; i++)
        {
            if (but != choiceOptions[i].Button)
            {
                choiceOptions[i].ImageBut.color = defaultButtonColour;
                choiceOptions[i].OptionText.color = Color.black;
                choiceOptions[i].IsSelected = false;
            }
            else
            {
                choiceOptions[i].IsSelected = true;
                choiceOption = choiceOptions[i];
            }
        }
    }

    private bool CheckIfOptionSelectedIsCorrect()
    {
        //know the correct option
        for (int i = 0; i < choiceOptions.Length; i++)
        {
            if (choiceOptions[i].OptionText.text == quizOptionSO.GetCorrectChoice())
            {
                correctChoiceOption = choiceOptions[i];
                break;
            }
        }

        if (!IsAnyButtonSelected)
            return false;
       
        bool ret = false;

        for (int i = 0; i < quizOptionSO.choices.Length; i++)
        {
            if (quizOptionSO.choices[i].isCorrectChoice && choiceOption.OptionText.text == quizOptionSO.choices[i].choice)
            {
                ret = true;
                break;
            }
        }

        return ret;
    }

    private void CorrectAnswerDisplay()
    {
        BoxImage.color = correctColour;

        correctChoiceOption.ImageBut.color = Color.green;
        correctChoiceOption.OptionText.color = Color.white;
    }

    private void FailedAnswerDisplay()
    {
        BoxImage.color = failedColour;

        correctChoiceOption.ImageBut.color = Color.green;
        correctChoiceOption.OptionText.color = Color.white;
    }
}
