using System;
using Core.Entities;

namespace Core.Services.Implementations
{
    public class DoubleGenerator : IValueGenerator
    {
        public object Generate(GeneratorContext context)
        {
            return context.Random.NextDouble();
        }

        public bool CanGenerate(Type type)
        {
            return type == typeof(double);
        }
    }
}