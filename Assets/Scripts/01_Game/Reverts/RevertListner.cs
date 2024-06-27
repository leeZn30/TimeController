using UnityEngine;

public enum Revert_Event_Type
{
    Animation_Change,
    Translation,
    Rotation,
    Scaling
}

public interface RevertListner
{
    void OnEvent(Revert_Event_Type event_Type, Component sender, object param = null);
}
