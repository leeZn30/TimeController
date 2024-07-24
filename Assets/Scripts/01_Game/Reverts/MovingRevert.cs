using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovingSnapshot
{
    public Vector3 Position { get; private set; }
    public Vector3 Scale { get; private set; }
    public Quaternion Rotation { get; private set; }

    public MovingSnapshot(Vector3 position, Vector3 scale, Quaternion rotation)
    {
        Position = position;
        Scale = scale;
        Rotation = rotation;
    }
}

public class MovingRevert : Revertible
{

    [SerializeField] Collider2D trigger;
    [SerializeField] MovingEvent movingEvent;
    [SerializeField] bool isChanging = false;
    [SerializeField] bool isRewinding = false;
    [SerializeField] Stack<MovingSnapshot> snapshots = new Stack<MovingSnapshot>();

    Collider2D playerCollider;


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
            RecordMovement();
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
        StartCoroutine(RewindMovement());
    }

    IEnumerator RewindMovement()
    {
        if (!isRewinding)
            isRewinding = true;

        while (snapshots.Count > 0)
        {
            MovingSnapshot snapshot = snapshots.Pop();
            transform.position = snapshot.Position;
            transform.localScale = snapshot.Scale;
            transform.rotation = snapshot.Rotation;
            yield return null; // 한 프레임 대기
        }

        if (isRewinding)
            isRewinding = false;

        StartCoroutine(returnAfterRewind());
    }

    void RecordMovement()
    {
        MovingSnapshot snapshot = new MovingSnapshot(
            transform.position,
            transform.localScale,
            transform.rotation);
        snapshots.Push(snapshot);
    }

    protected override IEnumerator returnAfterRewind()
    {
        snapshots.Clear();

        yield return new WaitForSeconds(3f);

        Change();
    }
}
