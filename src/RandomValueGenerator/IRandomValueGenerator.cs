using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomValueGenerator
{
    public interface IRandomValueGenerator
    {
        RandomGeneratorType Type { get; set; }
        int GetNextGenerationTime();
    }
}
