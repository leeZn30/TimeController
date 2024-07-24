using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;

public abstract class Revertible : MonoBehaviour
{
    public bool isRevertible = false;
    protected bool isDoneInitChanged = false;
    bool isActive = false;
    bool isInDrag = false;
    [SerializeField] float returnTime = 5f;
    [SerializeField] float minOverlapArea = 0.4f;
    float originalSize;

    protected Collider2D collider;
    Collider2D draggerCollider;
    SpriteRenderer spriteRenderer;
    [SerializeField] Material detectedMaterial;
    Material originalMaterial;
    Animator anim;

    protected virtual void Awake()
    {
        collider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        detectedMaterial = Resources.Load<Material>("Materials/RevertMeterial");
        anim = GetComponent<Animator>();

        originalMaterial = spriteRenderer.material;
        originalSize = collider.bounds.size.x * collider.bounds.size.y;
    }

    // Update is called once per frame
    protected virtual void Update()
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

        if (!isDoneInitChanged)
            checkChangeCondition();

        if (Input.GetMouseButton(0))
        {
            DoRewind();
        }
    }

    private void LateUpdate()
    {
        if (isInDrag && isRevertible)
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

    public virtual void Change()
    {
        if (!isDoneInitChanged)
            isDoneInitChanged = true;
    }

    protected void OnChanged()
    {
        isRevertible = true;
    }

    protected abstract void checkChangeCondition();

    protected abstract void Rewind();

    void OnReverted()
    {
        isRevertible = false;
    }

    virtual protected void DoRewind()
    {
        if (isInDrag && isRevertible)
        {
            isRevertible = false;
            Rewind();
            // StartCoroutine(returnAfterRewind());
            Character.Instance.FinishRewind();
        }
    }

    // rewind 후에 부르기
    protected virtual IEnumerator returnAfterRewind()
    {
        yield return new WaitForSeconds(returnTime);

        Change();
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
