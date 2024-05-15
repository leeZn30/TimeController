using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revertible : MonoBehaviour
{
    public bool isRevert = false;

    Collider2D collider;
    Collider2D draggerCollider;
    SpriteRenderer spriteRenderer;
    [SerializeField] Material detectedMaterial;
    Material originalMaterial;

    [SerializeField] float minOverlapArea = 0.4f;
    float originalSize;

    bool isActive = false;
    bool isInDrag = false;

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalMaterial = spriteRenderer.material;
        originalSize = collider.bounds.size.x * collider.bounds.size.y;
    }

    private void Start()
    {
        GetComponent<Animator>().SetTrigger("Fall");
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            if (minOverlapArea <= calculateIntersectRegion())
            {
                if (!isInDrag)
                    isInDrag = true;
            }
            else
            {
                if (isInDrag)
                    isInDrag = false;
            }
        }
        else
        {
            if (isInDrag)
                isInDrag = false;
        }

        if (isInDrag)
        {
            if (Input.GetMouseButton(0))
            {
                DoRewind();
            }
        }
    }

    private void LateUpdate()
    {
        if (isInDrag)
        {
            if (spriteRenderer.material != detectedMaterial)
                spriteRenderer.material = detectedMaterial;
        }
        else
        {
            if (spriteRenderer.material != originalMaterial)
                spriteRenderer.material = originalMaterial;

        }
    }

    float calculateIntersectRegion()
    {
        if (draggerCollider != null)
        {
            Bounds bounds = collider.bounds;
            Vector3 min = Vector3.Max(bounds.min, draggerCollider.bounds.min);
            Vector3 max = Vector3.Min(bounds.max, draggerCollider.bounds.max);
            Vector3 overlapSize = max - min;
            float overlapArea = overlapSize.x * overlapSize.y;

            return overlapArea / originalSize;
        }
        else
        {
            return 0;
        }
    }

    public void DoRewind()
    {
        if (isInDrag && !isRevert)
        {
            GetComponent<Animator>().SetTrigger("Climb");

            Character.Instance.FinishRewind();

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("RewindDragger"))
        {
            isActive = true;
            draggerCollider = other;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Equals("RewindDragger"))
        {
            isActive = false;
            draggerCollider = null;
        }
    }

}
