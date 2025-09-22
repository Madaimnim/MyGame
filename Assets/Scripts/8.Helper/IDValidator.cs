public static class IDValidator
{
    public const int PlayerIDMin = 1000;
    public const int PlayerIDMax = 1999;
    public const int EnemyIDMin = 2000;
    public const int EnemyIDMax = 9999;

    public static bool IsPlayerID(int id) => id >= PlayerIDMin && id <= PlayerIDMax;
    public static bool IsEnemyID(int id) => id >= EnemyIDMin && id <= EnemyIDMax;
}