using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TypingEffect : MonoBehaviour
{
    [SerializeField] [TextArea] private string Sentence;
    [SerializeField] private Text text;

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

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ClearText()
    {
        text.text = "";
    }
}


