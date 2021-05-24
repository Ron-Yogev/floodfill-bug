using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class paintingColor : MonoBehaviour
{
    //Texture to store the valid format inorder to allow changing in pixel color
    Texture2D readble;

    /*
     * Picture format changing in order to be in the right format, to be able to edit it in run time.
     */
    void duplicateTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        readble = new Texture2D(source.width, source.height);
        readble.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readble.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
    }


    /*
     * Function that get the correct coordinate and preform the floodfill algorithm with target color.
     */
    public void floodfillAlgo(Vector2 dat, Texture2D pic, Color targetColor)
    {
        int x = 0;
        int y = 0;
        if(getCorrectPixelMouseClick(dat, pic, ref x, ref y)){
            floodFill4Stack(targetColor, x, y);
            readble.Apply();
            gameObject.GetComponent<Image>().overrideSprite = Sprite.Create(readble, new Rect(0.0f, 0.0f, readble.width, readble.height), new Vector2(0.5f, 0.5f), 100.0f);
        }

    }

    /*
     * This funuction change the mouse click to the correct mouse position related to canvas,
     * and return true if we can preform the floodfill algorithm with user chosen color.
     */
    bool getCorrectPixelMouseClick(Vector2 dat,Texture2D pic, ref int xPos,ref int yPos )
    {
        duplicateTexture(pic);
        Vector2 localCursor;
        var rect1 = GetComponent<RectTransform>();
        var pos1 = dat;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect1, pos1,
            null, out localCursor))
            return false;

        int xpos = (int)(localCursor.x);
        int ypos = (int)(localCursor.y);

        if (xpos < 0) xpos = xpos + (int)rect1.rect.width / 2;
        else xpos += (int)rect1.rect.width / 2;

        if (ypos > 0) ypos = ypos + (int)rect1.rect.height / 2;
        else ypos += (int)rect1.rect.height / 2;

        if (xpos < 0 || ypos < 0 || xpos > rect1.sizeDelta.x || ypos > rect1.sizeDelta.y) return false;
        int x = (int)(readble.width * (xpos / rect1.sizeDelta.x));
        int y = (int)(readble.height * (ypos / rect1.sizeDelta.y));

        if (readble.GetPixel(x, y) == new Color(0f, 0f, 0f, 1f)) return false;

        xPos = x;
        yPos = y;
        return true;
    }

    public static readonly int[] dx = { 0, 1, 0, -1 }; // relative neighbor x coordinates
    public static readonly int[] dy = { -1, 0, 1, 0 }; // relative neighbor y coordinates

    /*
     * Floodfill algorithm using our own hashset
     */
    void floodFill4Stack(Color targetColor, int x, int y )
    {
        if (!CheckValidity(x, y, targetColor)) return; //avoid infinite loop
    
        HashSet<Vector2> hs = new HashSet<Vector2>();
        hs.Add(new Vector2(x,y));
        while (hs.Count > 0)
        {

            Vector2 point = hs.ElementAt(hs.Count - 1);
            hs.Remove(point);
            readble.SetPixel((int)point.x, (int)point.y, targetColor);

            for (int i = 0; i < 4; i++)
            {
                int nx = (int)point.x + dx[i];
                int ny = (int)point.y + dy[i];
                if (CheckValidity(nx, ny, targetColor))
                {
                    hs.Add(new Vector2(nx, ny));
                }
            }
        }
    }

    /*
     * Function that checks if the coordinate is valid for coloring
     */
    bool CheckValidity(int x, int y, Color TargetColor)
    {
        if (readble.GetPixel(x, y) == TargetColor)
        {
            return false;
        }
        if (readble.GetPixel(x, y) == new Color(0f, 0f, 0f, 1f))
        {
            return false;
        }
        if (x <= 0 || x >= readble.width - 1 || y <= 0 || y >= readble.height - 1)
        {
            return false;
        }

        return true;
    }

}
