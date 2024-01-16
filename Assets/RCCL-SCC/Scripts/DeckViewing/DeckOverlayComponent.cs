using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckOverlayComponent : MonoBehaviour
{
    public DeckOverlay[] decks;

    public GameObject[] overlays;

    public bool toggled;

    public Button overlayButton;

    public string overlayType;

    // Use this for initialization
    void Start()
    {
        overlayButton = this.GetComponent<Button>();

        overlayButton.onClick.AddListener(delegate
        {
            ToggleOverlays(overlays, toggled, overlayType);
            toggled = !toggled;
        });
    }

    public void ToggleOverlays(GameObject[] overlays, bool toggle, string fieldName)
    {
        if (!toggle)
        {
            int overlayNum = 0;

            for (int i = 0; i < decks.Length; i++)
            {
                GameObject newOverlay = Instantiate(overlays[overlayNum], decks[i].transform);

                newOverlay.name = fieldName;

                int newIndex = decks[i].transform.Find("LineHolder").GetSiblingIndex();

                newOverlay.transform.SetSiblingIndex(newIndex);

                overlayNum++;
            }
        }

        else
        {
            for (int i = 0; i < decks.Length; i++)
            {
                Destroy(decks[i].transform.Find(fieldName).gameObject);
            }

            Resources.UnloadUnusedAssets();
        }

    }
}
