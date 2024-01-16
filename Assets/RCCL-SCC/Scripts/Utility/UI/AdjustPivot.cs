using UnityEngine;

public class AdjustPivot : MonoBehaviour
{
    private RectTransform deckHolderPivot;

    public bool screenIs4K;
    public bool screenIs1080;

	void Start ()
    {
        deckHolderPivot = this.GetComponent<RectTransform>();
        ChangePosition();
	}

    private void Update()
    {
        if(screenIs1080 && Screen.currentResolution.width != 1920)
        {
            if(Input.GetKeyDown(KeyCode.U))
            {
                ChangeResolution(1920, 1080);
            }
        }

        if (screenIs4K && Screen.currentResolution.width != 3840)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                ChangeResolution(3840, 2160);
            }
        }
    }

    public void ChangeResolution(int width, int height)
    {
        Screen.SetResolution(width, height, true);
    }

    public void ChangePosition()
    {
        if(Screen.width == 3840f)
        {
            //deckHolderPivot.anchoredPosition = new Vector2(0f, 0f);
            deckHolderPivot.localScale = Vector3.one;
            screenIs4K = true;
            screenIs1080 = false;
        }

        else if(Screen.width == 1920f)
        {
            deckHolderPivot.anchoredPosition = new Vector2(0f, 0f);
            deckHolderPivot.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            screenIs4K = false;
            screenIs1080 = true;
        }
    }
}
