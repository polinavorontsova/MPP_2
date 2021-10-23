using System;
using Core.Entities;

namespace Core.Services.Implementations
{
    public class StringGenerator : IValueGenerator
    {
        public object Generate(GeneratorContext context)
        {
            return Guid.NewGuid().ToString();
        }

        public bool CanGenerate(Type type)
        {
            return type == typeof(string);
        }
    }
}