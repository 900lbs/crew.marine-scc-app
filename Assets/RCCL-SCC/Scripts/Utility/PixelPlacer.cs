/*
 * © 900lbs of Creative
 * Creation Date: DATE HERE
 * Date last Modified: MOST RECENT MODIFICATION DATE HERE
 * Name: AUTHOR NAME HERE
 * 
 * Description: DESCRIPTION HERE
 * 
 * Scripts referenced: LIST REFERENCED SCRIPTS HERE
 */


using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;

public class PixelPlacer : MonoBehaviour 
{
    public Texture2D deckMapGuide;

    public int pixelSize;

    public float multiplier;

    public Transform img;

    public Vector2 StartPoint = new Vector2(0, 0);

    public Button startButton;

    private void Start()
    {
        startButton.onClick.AddListener(StartItUp);

        multiplier = img.GetComponent<RectTransform>().rect.size.x / deckMapGuide.width;
    }

    public void StartItUp()
    {
        GenerateIcons();
    }

    public void GenerateIcons()
    {
        for (int x = 0; x < deckMapGuide.width; x += pixelSize)
        {
            for (int y = 0; y < deckMapGuide.height; y += pixelSize)
            {
                var pixelColor = deckMapGuide.GetPixel(x, y);

                var objectName = GetGameObject(pixelColor);

                if(objectName == null)
                {
                    continue;
                }

                var tilePoint = TileToPoint(x, y);

                var obj = Resources.Load(objectName, typeof(GameObject));
                GameObject pixel = Instantiate(obj, img) as GameObject;
                pixel.transform.localPosition = tilePoint;

            }
        }
    }

    private Vector2 TileToPoint(int x, int y)
    {
        if (x == 0 && y == 0)
        {
            return StartPoint;
        }

        var posX = x * multiplier;
        var posY = y * multiplier;

        var newPoint = StartPoint;
        newPoint.x += posX;
        newPoint.y += posY;
        return newPoint;
    }

    private string GetGameObject(Color color)
    {
        var colorHex = ColorUtility.ToHtmlStringRGB(color);
        switch (colorHex)
        {
            case "00FF2A": // green - green dot
                {
                    return "Prefabs/Green Dot";
                }
            case "FF0000": // red - red dot
                {
                    return "Prefabs/Red Dot";
                }
            case "FFB600": // yellow - yellow dot
                {
                    return "Prefabs/Yellow Dot";
                }
            default:
                return null;
        }
    }
}
