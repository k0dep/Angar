using System.Collections.Generic;
using Angar.Data;
using Angar.Factory;

namespace Angar.Importing
{
    public interface IImporter
    {
        IList<string> Errors { get; }
        IList<string> Warnings { get; }
        IList<string> Infos { get; }

        bool Validate();
        IPoolDataSet Import(IPoolDataSetFactory dataSetFactory, IPoolDataSetItemFactory dataSetItemFactory);
    }
}