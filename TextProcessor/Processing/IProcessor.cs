using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextProcessor.Processing
{
    public interface IProcessor
    {
        IEnumerable<TEntity> Process<TEntity>(String fileName) where TEntity : class;
    }
}
