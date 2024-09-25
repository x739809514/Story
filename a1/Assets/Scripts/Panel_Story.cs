using System.Collections;
using System.Collections.Generic;
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
    public Button btnNext;
    public TextMeshProUGUI nameLeft;
    public TextMeshProUGUI nameRight;
    public TextMeshProUGUI dialogContent;
    public Transform optionsParent;
    public Transform animParent;
    public Transform content;
    public Transform anim;
    public Button btnAnimStop;

    private float typeSpeed = 0.1f;
    private WaitForSeconds typeTime;
    private List<GameObject> optionList;
    private Coroutine typeCor;
    private string curDialogContent;
    private AnimPlayable animPlayable;

    private void Start()
    {
        StorySystem.Instance.moveToDialogEndHandler += DoMoveToDialogEnd;
        StorySystem.Instance.updateDialogHandler += UpdateDialog;
        StorySystem.Instance.updateOptionsHandler += UpdateOptions;
        StorySystem.Instance.updateBackgroundHandler += UpdateBackgroundImg;
        StorySystem.Instance.updateAnimationHandler += UpdateAnim;
        btnNext.onClick.AddListener(OnGoNextClick);
        btnAnimStop.onClick.AddListener(OnAnimStopClick);
    }

    private void UpdateDialog(SinglePersonChat data, string personName)
    {
        content.gameObject.SetActive(true);
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
            roleLeft.SetNativeSize();
            nameLeft.text = personName;
        }
        else if (pos == RolePos.Right)
        {
            roleRight.gameObject.SetActive(true);
            nameRight.gameObject.SetActive(true);
            roleRight.sprite = data.icon;
            roleRight.SetNativeSize();
            nameRight.text = personName;
        }

        typeTime = new WaitForSeconds(typeSpeed);
        typeCor = StartCoroutine(TypeWords(data.content));
    }

    IEnumerator TypeWords(string content)
    {
        var curPos = 0;
        var len = content.Length;
        curDialogContent = content;
        StringBuilder sb = new StringBuilder("");
        GameManager.audioManager.PlaySound("type");
        while (curPos < content.Length)
        {
            yield return typeTime;
            sb.Append(content[curPos]);
            dialogContent.text = sb.ToString();
            curPos++;
        }

        GameManager.audioManager.StopSound("type");
        dialogContent.text = content;
    }

    private void UpdateOptions(List<string> options)
    {
        // forbidden dialog click
        btnNext.interactable = false;
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
            go.GetComponent<Button>().onClick.AddListener(() => { OnOptionClick(i1); });
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
        btnNext.interactable = true;
        EventHandle.DispatchEvent<int>(EventName.EvtOptionClick, index);
    }

    private void UpdateBackgroundImg(Sprite sprite)
    {
        bgImg.sprite = sprite;
    }

    private void UpdateAnim(AnimationClip clip)
    {
        btnNext.interactable = false;
        bgImg.gameObject.SetActive(false);
        content.gameObject.SetActive(false);
        btnAnimStop.gameObject.SetActive(true);
        anim.gameObject.SetActive(true);
        var animator = animParent.GetComponent<Animator>();
        animPlayable = new AnimPlayable(clip, animator);
        animPlayable.PlayAnim();
    }

    private void DoMoveToDialogEnd()
    {
        if (typeCor != null)
        {
            StopCoroutine(typeCor);
        }

        dialogContent.text = curDialogContent;
        GameManager.audioManager.StopSound("type");
    }

    private void OnGoNextClick()
    {
        EventHandle.DispatchEvent(EventName.EvtDialogClick);
    }

    private void OnAnimStopClick()
    {
        btnNext.interactable = true;
        bgImg.gameObject.SetActive(true);
        content.gameObject.SetActive(true);
        btnAnimStop.gameObject.SetActive(false);
        anim.gameObject.SetActive(false);
        animPlayable.StopAnim();
        EventHandle.DispatchEvent(EventName.EvtAnimStopClick);
    }

    private void OnDestroy()
    {
        StorySystem.Instance.moveToDialogEndHandler -= DoMoveToDialogEnd;
        StorySystem.Instance.updateDialogHandler -= UpdateDialog;
        StorySystem.Instance.updateOptionsHandler -= UpdateOptions;
        StorySystem.Instance.updateBackgroundHandler -= UpdateBackgroundImg;
        btnNext.onClick.RemoveAllListeners();
    }
}