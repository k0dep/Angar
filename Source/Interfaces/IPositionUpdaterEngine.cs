using System.Collections.Generic;
using Angar.Data;

namespace Angar.PositionEngine
{
    public interface IPositionUpdaterEngine : IPositionUpdaterBase
    {
        void Initialize(IPoolDataSet dataset, IPostionTargetSource postionTargetSource);
        Queue<int> LoadQueue { get; set; }
        Queue<int> UnloadQueue { get; set; }
    }
}
