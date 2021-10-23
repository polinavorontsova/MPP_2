using System;
using Core.Entities;

namespace Core.Services.Implementations
{
    public class CharGenerator : IValueGenerator
    {
        public object Generate(GeneratorContext context)
        {
            return (char) context.Random.Next(char.MinValue, char.MaxValue);
        }

        public bool CanGenerate(Type type)
        {
            return type == typeof(char);
        }
    }
}