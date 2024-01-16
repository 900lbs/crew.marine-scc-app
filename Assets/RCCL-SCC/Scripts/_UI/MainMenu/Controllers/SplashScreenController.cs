using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using System.Threading.Tasks;

public class SplashScreenController : _MenuController
{
    public CanvasGroup LogoImage;

    public void Start()
    {
        cg.alpha = 1;

        if(LogoImage != null)
            LogoImage.alpha = 0;
    }
    public async Task BeginSplashSequence(float fadeDuration)
    {
        LogoImage.DOFade(1, fadeDuration);
        await new WaitForSeconds(3f);
        LogoImage.DOFade(0, fadeDuration);
        await new WaitForSeconds(fadeDuration);
    }

}
