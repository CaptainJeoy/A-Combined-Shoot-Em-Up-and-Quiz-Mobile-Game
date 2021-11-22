using UnityEngine;

[System.Serializable]
public class Choices
{
    public string choice;
    public bool isCorrectChoice;
}

[CreateAssetMenu()]
public class QuizOptionSO : ScriptableObject
{
    public string Question;

    public string[] FalseOptionsList;

    public Choices[] choices;

    public string GetCorrectChoice()
    {
        string ret = null;

        for (int i = 0; i < choices.Length; i++)
        {
            if (choices[i].isCorrectChoice)
            {
                ret = choices[i].choice;
                break;
            }
        }

        return ret;
    }

    public void SetRandomizedFalseOptions()
    {
        int skipNum = 10000;

        for (int i = 0; i < choices.Length; i++)
        {
            if (!choices[i].isCorrectChoice)
            {
                int rand = GenerateRandomNumberBetweenWithIgnoreNum(0, FalseOptionsList.Length, skipNum);
                choices[i].choice = FalseOptionsList[rand];
                skipNum = rand;
            }
        }   
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
}
