//MessageWindow.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MessageWindow : MonoBehaviour
{
    public string Message = "";
    public float TextSpeedPerChar = 1000/10f;
    [Min(1)] public float SpeedUpRate = 3f;
    [Min(1)] public int MaxLineCount = 4;

    public bool IsEndMessage { get; private set; } = true;
    Transform TextRoot;
    Text TextTemplate;

    private void Awake()
    {
        TextRoot = transform.Find("Panel");
        TextTemplate = TextRoot.Find("TextTemplate").GetComponent<Text>();
        TextTemplate.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void StartMessage(string message)
    {
        Message = message;
        StopAllCoroutines();
        gameObject.SetActive(true);
        StartCoroutine(MessageAnimation());
    }

    IEnumerator MessageAnimation()
    {
        IsEndMessage = false;
        DestroyLineText();

        var lines = Message.Split('\n');
        var lineCount = 0;
        var textObjs = new List<Text>();

        foreach(var line in lines)
        {
            lineCount++;
            if(lineCount >= MaxLineCount)
            {
                Object.Destroy(textObjs[0].gameObject);
                textObjs.RemoveAt(0);
            }
            var lineText = Object.Instantiate(TextTemplate, TextRoot);
            lineText.gameObject.SetActive(true);
            lineText.text = "";
            textObjs.Add(lineText);

            for(var i=0; i<line.Length; ++i)
            {
                lineText.text += line[i];
                var speed = TextSpeedPerChar / (Input.anyKey ? SpeedUpRate : 1);
                yield return new WaitForSeconds(speed);
            }
        }

        yield return new WaitUntil(() => Input.anyKeyDown);
        IsEndMessage = true;
        gameObject.SetActive(false);
    }

    void DestroyLineText()
    {
        foreach(var text in TextRoot.GetComponentsInChildren<Text>().Where(_t => _t != TextTemplate))
        {
            Object.Destroy(text.gameObject);
        }
    }
}