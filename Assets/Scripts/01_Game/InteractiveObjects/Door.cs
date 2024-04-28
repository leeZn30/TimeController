using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour
{
    public string PairKeyName = "";
    [SerializeField] string NextScene;
    public bool isGainItem = false;
    bool isPlayerIn = false;

    [SerializeField] GameObject NoItemPfb;
    [SerializeField] GameObject HaveItemPfb;
    [SerializeField] GameObject interactPfb;

    protected Canvas Fixedcanvas;
    protected Collider2D collider;

    GameObject interactUI;
    GameObject HaveItemUI;
    GameObject NoItemUI;

    Vector3 ItemPosition;
    Vector3 InteractPosition;

    protected void Awake()
    {
        Fixedcanvas = GameObject.Find("FixedCanvas").GetComponent<Canvas>();
        collider = GetComponent<Collider2D>();

        ItemPosition = collider.bounds.center;
        InteractPosition = collider.bounds.center + new Vector3(0, collider.bounds.size.y / 2 + interactPfb.transform.localScale.y / 2, 0);

        ItemData data = GameData.ItemDatas.Find(x => x.ID == PairKeyName);
        if (data != null)
        {
            if (data.IsGain)
            {
                isGainItem = true;
            }
        }
        if (isGainItem)
            HaveItemUI = Instantiate(HaveItemPfb, ItemPosition, Quaternion.identity, Fixedcanvas.transform);
        else
            NoItemUI = Instantiate(NoItemPfb, ItemPosition, Quaternion.identity, Fixedcanvas.transform);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPlayerIn)
        {
            interact();
        }
    }

    protected void interact()
    {
        if (isGainItem)
        {
            // 문 한번 열면 저장
            if (!PairKeyName.Equals(""))
            {
                GameData.ItemDatas.Find(x => x.ID == PairKeyName).IsGain = true;
            }

            GameData.NowGhosts = GhostManager.Instance.ghostCount;
            SceneChanger.LoadSceneByDoor(NextScene, gameObject.name);
        }
        else
        {
            StartCoroutine(Vibrate());
        }
    }

    IEnumerator Vibrate()
    {
        Vector3 startPosition = transform.position;
        float vibrationStrength = 0.05f;
        float vibrationSpeed = 20f;

        float duration = 0f;
        while (duration < 0.5f)
        {
            duration += Time.deltaTime;

            // 진동 강도와 속도에 따라 새로운 위치 계산
            float newPositionX = startPosition.x + Mathf.Sin(Time.time * vibrationSpeed) * vibrationStrength;
            float newPositionY = startPosition.y + Mathf.Sin(Time.time * vibrationSpeed * 1.1f) * vibrationStrength;
            float newPositionZ = startPosition.z + Mathf.Sin(Time.time * vibrationSpeed * 1.2f) * vibrationStrength;

            // 새로운 위치 적용
            transform.position = new Vector3(newPositionX, newPositionY, newPositionZ);
            yield return null;
        }

        transform.position = startPosition;
    }

    public virtual void Unlock()
    {
        isGainItem = true;
        Destroy(NoItemUI);
        HaveItemUI = Instantiate(HaveItemPfb, ItemPosition, Quaternion.identity, Fixedcanvas.transform);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            interactUI = Instantiate(interactPfb, InteractPosition, Quaternion.identity, Fixedcanvas.transform);
            isPlayerIn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            isPlayerIn = false;

            if (interactUI != null)
                Destroy(interactUI);
        }
    }

}
