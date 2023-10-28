using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;

public class TutBase : MonoBehaviour
{
    [SerializeField] protected RectTransform hand;
    [SerializeField] protected GameObject focus;
    [SerializeField] protected Button btnAction;
    [SerializeField] protected Button btnNext;
    [SerializeField] protected TextMeshProUGUI txtDialog;

    private UnityAction currentAction;
    private UnityAction nextAction;
    private bool focusShown;

    private Coroutine dialogCoro, showHandCoro;
    private string currentText;
    private bool textDone = false;

    protected virtual void Start()
    {
        btnAction.onClick.AddListener(() =>
               {
                   if (currentAction != null)
                   {
                       currentAction.Invoke();
                       //currentAction = null; 

                   }
                   else Debug.Log("nulllllllllllllll");
               });

        btnNext.onClick.AddListener(() => OnClickNextText());
    }

    public void SetSkipAction(UnityAction cb)
    {
        nextAction = cb;
    }

    protected void ResetAllEffect()
    {
        StopAllCoroutines();
        if (hand.gameObject.activeSelf)
            hand.gameObject.SetActive(false);
    }

    public void ShowFocus(Vector2 pos, float handAngle, UnityAction callback)
    {
        currentAction = callback;
        ResetAllEffect();
        StartCoroutine(_ShowFocus(pos, handAngle));
    }

    IEnumerator _ShowFocus(Vector2 pos, float handAngle)
    {
        yield return new WaitForSeconds(.2f);
        focusShown = true;
        Debug.Log("show focus");
        focus.gameObject.SetActive(true);
        focus.transform.position = pos;

        hand.transform.rotation = Quaternion.identity;
        hand.transform.rotation = Quaternion.AngleAxis(handAngle, Vector3.forward);

        while (focusShown)
        {
            HandTouch(pos);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void HideFocus()
    {
        focusShown = false;
        focus.gameObject.SetActive(false);
        HideHand();
    }

    protected void HideHand()
    {
        focusShown = false;
        hand.gameObject.SetActive(false);
        if (showHandCoro != null)
        {
            StopCoroutine(showHandCoro);
            showHandCoro = null;
        }
        hand.transform.rotation = Quaternion.identity;
    }

    void HandTouch(Vector2 pos)
    {
        hand.gameObject.SetActive(true);
        //Vector2 posAnim = pos + new Vector2(0.5f, -0.5f);
        hand.transform.position = pos;
        //hand.transform.DOMove(pos, 0.2f);
    }

    protected void ShowOnlyHand(Transform tf, float angle, bool followTf, float scale = 1)
    {
        hand.transform.localScale = new Vector3(scale, scale, 1);
        hand.transform.rotation = Quaternion.identity;
        hand.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        if (showHandCoro != null)
        {
            StopCoroutine(showHandCoro);
            showHandCoro = null;
        }

        if (followTf)
        {
            showHandCoro = StartCoroutine(ShowOnlyHandIE());
        }
        else
            HandTouch(tf.position);
        IEnumerator ShowOnlyHandIE()
        {
            HandTouch(tf.position);

            while (true)
            {
                hand.transform.position = tf.position;
                yield return null;
            }
        }
    }

    protected void ShowOnlyHand(Vector3 pos, float angle)
    {
        if (showHandCoro != null)
        {
            StopCoroutine(showHandCoro);
            showHandCoro = null;
        }

        hand.transform.rotation = Quaternion.identity;
        hand.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        HandTouch(pos);
    }

    public void ShowText(string msg, UnityAction cb = null)
    {
        nextAction = cb;
        btnNext.gameObject.SetActive(true);

        // if(showAnim)
        // dialogAnim.SetTrigger("Show");
        if (dialogCoro != null)
        {
            StopCoroutine(dialogCoro);
            dialogCoro = null;
        }
        if (!string.IsNullOrEmpty(msg))
            dialogCoro = StartCoroutine(_ShowText(msg));
        else txtDialog.text = "";
    }

    public void HideText()
    {
        btnNext.gameObject.SetActive(false);
        // dialogAnim.SetTrigger("Hide");

    }

    IEnumerator _ShowText(string msg)
    {
        textDone = false;
        currentText = msg;
        txtDialog.text = "";

        yield return new WaitForSeconds(0.5f);
        foreach (char c in msg.ToCharArray())
        {
            txtDialog.text += c;
            AudioManager.instance?.PlaySound(SoundName.text_type_tut);
            yield return new WaitForSeconds(0.01f);
        }
        textDone = true;
    }

    public void OnClickNextText()
    {
        if (textDone)
        {
            //HideText();
            if (nextAction != null)
            {
                nextAction.Invoke();
                //nextAction = null;
            }
        }
        else
        {
            if (dialogCoro != null)
            {
                StopCoroutine(dialogCoro);
                dialogCoro = null;
                txtDialog.text = currentText;
                textDone = true;
            }
        }
    }

}
