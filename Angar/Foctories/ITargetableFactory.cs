using UnityEngine;

namespace Angar.Factory
{
    public interface ITargetableFactory<TType>
    {
        TType Create(GameObject target);
    }

    public interface ITargetableFactory
    {
        void Create(GameObject target);
    }
}
