using System;
using Core.Entities;
using Core.Services;

namespace BooleanGenerator
{
    public class BooleanGenerator : IValueGenerator
    {
        public object Generate(GeneratorContext context)
        {
            return context.Random.Next(1 << 1) == 1;
        }

        public bool CanGenerate(Type type)
        {
            return type == typeof(bool);
        }
    }
}