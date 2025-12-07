namespace Core.Events
{
    public delegate void GameEvent();

    public delegate void GameEventInteger(int val);

    public delegate void GameEventWith2Ints(int index, int val);
}
