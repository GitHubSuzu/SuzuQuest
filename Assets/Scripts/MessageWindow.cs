//MessageWindow.cs 選択肢に対応するように修正
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MessageWindow : MonoBehaviour
{
    public const string YES_NO_MENU_LINE_TEXT = "<YESNO>";

    public string Message = "";
    public float TextSpeedPerChar = 1000/10f;
    [Min(1)] public float SpeedUpRate = 3f;
    [Min(1)] public int MaxLineCount = 4;

    public bool IsEndMessage { get; private set; } = true;

    public YesNoMenu YesNoMenu;
    public string[] Params { get; set; }

    Transform TextRoot;
    Text TextTemplate;

    private void Awake()
    {
        TextRoot = transform.Find("Panel");
        TextTemplate = TextRoot.Find("TextTemplate").GetComponent<Text>();
        TextTemplate.gameObject.SetActive(false);
        gameObject.SetActive(false);

        YesNoMenu.gameObject.SetActive(false);
    }

    public void StartMessage(string message)
    {
        Message = message;
        StopAllCoroutines();
        gameObject.SetActive(true);
        StartCoroutine(MessageAnimation());
    }

    public void Close()
    {
        StopAllCoroutines();
        Params = null;
        IsEndMessage = true;
        YesNoMenu.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    IEnumerator MessageAnimation()
    {
        YesNoMenu.gameObject.SetActive(false);
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

            if(line == YES_NO_MENU_LINE_TEXT)
            {
                YesNoMenu.Open();

                yield return new WaitWhile(() => YesNoMenu.DoOpen);
            }
            else
            {
                for(var i=0; i<line.Length; ++i)
                {
                    if(line[i] == '#' && i+1 < line.Length)
                    {
                        if(char.IsDigit(line[i+1]))
                        {
                            var index = line[i+1] - '0';
                            var paramText = index < Params.Length ? Params[index] : $"#{line[i+1]}";

                            foreach(var ch in paramText)
                            {
                                lineText.text += ch;
                                var speed = TextSpeedPerChar / (Input.anyKey ? SpeedUpRate : 1);
                                yield return new WaitForSeconds(speed);
                            }
                        }
                        else if(line[i+1] == '#')
                        {
                            lineText.text += "#";
                            var speed = TextSpeedPerChar / (Input.anyKey ? SpeedUpRate : 1);
                            yield return new WaitForSeconds(speed);
                        }
                        else
                        {
                            lineText.text += line[i+1];
                            var speed = TextSpeedPerChar / (Input.anyKey ? SpeedUpRate : 1);
                            yield return new WaitForSeconds(speed);
                        }
                        ++i;
                    }
                    else
                    {
                        lineText.text += line[i];
                        var speed = TextSpeedPerChar / (Input.anyKey ? SpeedUpRate : 1);
                        yield return new WaitForSeconds(speed);
                    }
                }
            }
        }

        yield return new WaitUntil(() => Input.anyKeyDown);
        Params = null;
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