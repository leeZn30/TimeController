using UnityEngine.UI;
using UnityEngine;
using System.Linq.Expressions;

public class Background : Singleton<Background>
{
    [SerializeField] float[] speeds = new float[1];
    [SerializeField] Image[] backgrounds = new Image[1];

    private void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BackgroundScroll(float direction)
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            Material material = backgrounds[i].material;
            float offsetX = Time.deltaTime * speeds[i] * direction;
            // float offsetY = Time.deltaTime * speeds[i] * direction.y;
            material.mainTextureOffset += new Vector2(offsetX, 0);
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
