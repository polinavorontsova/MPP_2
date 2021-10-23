using System;
using Core.Entities;
using Core.Services;

namespace DateGenerator
{
    public class DateGenerator : IValueGenerator
    {
        public object Generate(GeneratorContext context)
        {
            return DateTime.FromBinary(context.Random.Next());
        }

        public bool CanGenerate(Type type)
        {
            return type == typeof(DateTime);
        }
    }
}