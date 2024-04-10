using UnityEngine.UI;
using UnityEngine;
using System.Linq.Expressions;

public class Background : Singleton<Background>
{
    [SerializeField] bool isStuck;
    [SerializeField] float[] speeds = new float[1];
    [SerializeField] Image[] backgrounds = new Image[1];

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


}
