using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MovingRevert : Revertible
{

    [SerializeField] Collider2D trigger;
    [SerializeField] MovingEvent movingEvent;
    [SerializeField] bool isChanging = false;
    [SerializeField] bool isRewinding = false;

    Collider2D playerCollider;

    Stack<Vector3> moveStack = new Stack<Vector3>();

    protected override void Awake()
    {
        base.Awake();

        playerCollider = Character.Instance.GetComponent<Collider2D>();
    }

    protected override void checkChangeCondition()
    {
        if (trigger.IsTouching(playerCollider) && !isChanging && !isRewinding)
        {
            Change();
        }
    }

    protected override void Update()
    {
        base.Update();

        if (isChanging)
        {
            rememberMoving();
        }
    }

    public override void Change()
    {
        isChanging = true;

        base.Change();

        movingEvent.callback += OnChanged;
        movingEvent.callback += () => isChanging = false;
        movingEvent.callMove();
    }


    protected override void Rewind()
    {
        StartCoroutine(moveRewind());
    }

    IEnumerator moveRewind()
    {
        if (!isRewinding)
            isRewinding = true;

        while (moveStack.Count > 0)
        {
            // 스택에서 위치 가져오기
            Vector3 targetPosition = moveStack.Pop();

            // 현재 위치 저장
            Vector3 startPosition = transform.position;

            float elapsedTime = 0f;
            float moveDurattion = 0.001f;

            while (elapsedTime < moveDurattion)
            {
                // 위치를 보간하여 이동
                transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / moveDurattion));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 최종 위치 설정
            transform.position = targetPosition;

            // 다음 위치로 이동 전 잠시 대기
            yield return null; // Optional: 각 위치에서 잠시 대기 시간
        }

        if (isRewinding)
            isRewinding = false;
    }

    void rememberMoving()
    {
        if (moveStack.Count > 0)
        {
            if (transform.position != moveStack.Peek())
                moveStack.Push(transform.position);
        }
        else
        {
            moveStack.Push(transform.position);
        }

        Debug.Log(moveStack.Peek());

    }

}
