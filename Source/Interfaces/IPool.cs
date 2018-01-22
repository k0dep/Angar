namespace Angar
{
    public interface IPool<TType> : IPoolEvents<TType>
    {
        int MaxSize { get; set; }
        int MinSize { get; set; }
        int CurrentSize { get; }

        TType Prototype { get; set; }

        TType Pop();
        void Push(TType obj);
    }
}