using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomValueGenerator
{
    public class UniformValueGenerator : IRandomValueGenerator
    {
        protected int _a = 0;
        protected int _b = 0;

        public UniformValueGenerator(int a, int b)
        {
            if (a > b)
                throw new ArgumentException($"{a} must be less or equal than ${b}");
            _a = a;
            _b = b;
        }
        public RandomGeneratorType Type { get; set; }

        public int GetNextGenerationTime()
        {
            var random = new Random(DateTime.Now.Millisecond);
            return random.Next(_a, _b + 1);
        }
    }
}
