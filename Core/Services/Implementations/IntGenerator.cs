using System;
using Core.Entities;

namespace Core.Services.Implementations
{
    public class IntGenerator : IValueGenerator
    {
        public object Generate(GeneratorContext context)
        {
            return context.Random.Next();
        }

        public bool CanGenerate(Type type)
        {
            return type == typeof(int);
        }
    }
}