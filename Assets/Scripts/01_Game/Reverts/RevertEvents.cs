using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevertEvents
{
    // 이벤트 델리게이트 정의
    public delegate void OnMove();
    public static event OnMove MoveEvent;

    public delegate void OnAnimated();
    public static event OnAnimated AnimatedEvent;


    // 이벤트 호출 메서드 정의
    public static void TriggerMove()
    {
        MoveEvent?.Invoke();
    }
}
