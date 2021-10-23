using System;
using Core.Entities;

namespace Core.Services.Implementations
{
    public class LongGenerator : IValueGenerator
    {
        public object Generate(GeneratorContext context)
        {
            var buffer = new byte[8];
            context.Random.NextBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }

        public bool CanGenerate(Type type)
        {
            return type == typeof(long);
        }
    }
}