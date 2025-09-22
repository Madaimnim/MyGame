public interface IRuntime
{
    int CurrentHp { get; }
    void TakeDamage(int damage);
}
