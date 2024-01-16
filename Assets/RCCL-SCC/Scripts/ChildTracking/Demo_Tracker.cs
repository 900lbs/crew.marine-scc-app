using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Demo_Tracker : MonoBehaviour
{
    public Image trackerImg;

    public Color foundCol;
    public Color lostCol;

    public bool missing;

    public float scaleTime = 0.5f;
    public float scaleSize = 1.25f;

    Sequence pulseSeq;

    void Start()
    {
        pulseSeq = DOTween.Sequence();
        pulseSeq.Append(trackerImg.transform.DOScale(scaleSize, scaleTime));
        pulseSeq.Insert(0, trackerImg.DOFade(0, scaleTime));
        pulseSeq.SetLoops(-1, DOTween.defaultLoopType);
        pulseSeq.Pause();
    }


    // Update is called once per frame
    void Update()
    {
        if(missing)
        {
            pulseSeq.Play();
        }

        if(!missing && trackerImg.transform.localScale.x != 1)
        {
            pulseSeq.Pause();
            trackerImg.transform.localScale = Vector3.one;
            trackerImg.color = foundCol;
        }
    }

    public void ToggleColor()
    {
        missing = !missing;

        if(missing)
        {
            trackerImg.color = lostCol;
        }

        else
        {
            trackerImg.color = foundCol;
        }
    }
}
