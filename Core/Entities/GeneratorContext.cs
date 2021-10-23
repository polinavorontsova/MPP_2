using System;
using Core.Services;

namespace Core.Entities
{
    public class GeneratorContext
    {
        public GeneratorContext(Random random, Type targetType, IFaker faker)
        {
            Random = random;
            TargetType = targetType;
            Faker = faker;
        }

        public Random Random { get; }

        public Type TargetType { get; }

        public IFaker Faker { get; }
    }
}