using UnityEngine.UI;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] float[] speeds = new float[1];
    [SerializeField] Image[] backgrounds = new Image[1];

    private void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // for (int i = 0; i < backgrounds.Length; i++)
        // {
        //     Material material = backgrounds[i].material;
        //     float offset = Time.deltaTime * speeds[i] * 0.01f;
        //     material.mainTextureOffset += new Vector2(offset, 0);
        // }
    }

    public void BackgroundScroll(float direction)
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            Material material = backgrounds[i].material;
            float offset = Time.deltaTime * speeds[i] * 0.1f * direction;
            material.mainTextureOffset += new Vector2(offset, 0);
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
