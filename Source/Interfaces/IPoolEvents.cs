using System;

namespace Angar
{
    public interface IPoolEvents<TType>
    {
        event Action<object, TType> OnGenerate;
        event Action<object, TType> OnPop;
        event Action<object, TType> OnPush;
    }
}
