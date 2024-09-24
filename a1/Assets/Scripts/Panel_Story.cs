using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Story : MonoBehaviour
{
    // UI
    public Image bgImg;
    public Image roleLeft;
    public Image roleRight;
    public TextMeshProUGUI nameLeft;
    public TextMeshProUGUI nameRight;
    public TextMeshProUGUI dialogContent;
    public Transform optionsParent;

    private float typeSpeed = 0.1f;
    private WaitForSeconds typeTime;
    private List<GameObject> optionList;

    public void UpdateDialog(SinglePersonChat data, string personName)
    {
        var pos = data.pos;
        roleLeft.gameObject.SetActive(false);
        roleRight.gameObject.SetActive(false);
        nameLeft.gameObject.SetActive(false);
        nameRight.gameObject.SetActive(false);
        if (pos == RolePos.Left)
        {
            roleLeft.gameObject.SetActive(true);
            nameLeft.gameObject.SetActive(true);
            roleLeft.sprite = data.icon;
            nameLeft.text = personName;
        }
        else if (pos == RolePos.Right)
        {
            roleRight.gameObject.SetActive(true);
            nameRight.gameObject.SetActive(true);
            roleRight.sprite = data.icon;
            nameRight.text = personName;
        }

        typeTime = new WaitForSeconds(typeSpeed);
        StartCoroutine(TypeWords(data.content));
    }

    IEnumerator TypeWords(string content)
    {
        var curPos = 0;
        var len = content.Length;
        StringBuilder sb = new StringBuilder("");
        while (curPos < content.Length)
        {
            yield return typeTime;
            sb.Append(content[curPos]);
            dialogContent.text = sb.ToString();
            curPos++;
        }

        dialogContent.text = content;
    }

    public void UpdateOptions(List<string> options)
    {
        optionsParent.gameObject.SetActive(true);
        optionList ??= new List<GameObject>();
        for (int i = 0; i < options.Count; i++)
        {
            var go = Instantiate(Resources.Load<GameObject>("perfabs/options"));
            go.transform.SetParent(optionsParent);
            go.SetActive(true);
            go.transform.Find("txt").GetComponent<TextMeshProUGUI>().text = options[i];
            go.GetComponent<Button>().onClick.RemoveAllListeners();
            
            var i1 = i;
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnOptionClick(i1);
            });
            optionList.Add(go);
        }
    }

    private void OnOptionClick(int index)
    {
        foreach (var o in optionList)
        {
            Destroy(o);
        }
        optionList.Clear();
        optionsParent.gameObject.SetActive(false);
        EventHandle.DispatchEvent<int>(EventName.EvtOptionClick, index);
    }
}