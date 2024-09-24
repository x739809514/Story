using System.Collections;
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

    private float typeSpeed = 0.2f;
    private WaitForSeconds typeTime;
    
    public void UpdateDialog(SinglePersonChat data,string personName)
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
        }else if (pos == RolePos.Right)
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
        while (curPos<content.Length)
        {
            yield return typeTime;
            sb.Append(content[curPos]);
            dialogContent.text = sb.ToString();
            curPos++;
        }

        dialogContent.text = content;
    }
}