using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
struct SpeedRange
{
    public float min;
    public float max;
}

public class TypingEffect : MonoBehaviour
{
    [SerializeField] [TextArea] private string Sentence;
    [SerializeField] private Text text;

    [SerializeField] [Range(1f, 0f)] private float typingSpeed = 0.1f;

    [SerializeField] private SpeedRange speedRange;

    [SerializeField] private bool useRandomSpeed = false;

    [SerializeField] private Audio audioObj;

    char[] charArr;

    private void OnEnable()
    {
        charArr = Sentence.ToCharArray();

        StartCoroutine(TypeTheSentence());
    }

    IEnumerator TypeTheSentence()
    {
        int i = 0;

        while (i < charArr.Length)
        {
            text.text += charArr[i];
            audioObj.PlayGun();
            i++;

            if (!useRandomSpeed)
                yield return new WaitForSecondsRealtime(typingSpeed);
            else
                yield return new WaitForSecondsRealtime(Random.Range(speedRange.min, speedRange.max));
        }
    }

    public void ClearText()
    {
        text.text = "";
    }
}


