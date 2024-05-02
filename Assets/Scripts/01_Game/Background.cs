using UnityEngine.UI;
using UnityEngine;
using System.Linq.Expressions;
using System.Collections.Generic;

public class Background : Singleton<Background>
{
    [SerializeField] bool isStuck;
    [SerializeField] float[] speeds = new float[1];
    [SerializeField] Image[] backgrounds = new Image[1];
    public int BackgroundCnt => backgrounds.Length;

    public void BackgroundScroll(float direction)
    {
        if (!isStuck)
        {
            for (int i = 0; i < backgrounds.Length; i++)
            {
                Material material = backgrounds[i].material;
                float offsetX = Time.deltaTime * speeds[i] * direction;
                material.mainTextureOffset += new Vector2(offsetX, 0);
            }
        }
    }

    public void resetMaterial()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            Material material = backgrounds[i].material;
            material.mainTextureOffset = Vector2.zero;
        }
    }

    public void ChangeBackground(List<Sprite> newBGs)
    {
        for (int i = 0; i < newBGs.Count; i++)
        {
            backgrounds[i].sprite = newBGs[i];
        }
    }

}
