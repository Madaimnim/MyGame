public class GameEventsAdapter : IGameEvents
{
    public void RaiseBattleStart() => GameEventSystem.Instance.Event_BattleStart?.Invoke();
}