using System;
using Core.Entities;

namespace Core.Services
{
    public interface IValueGenerator
    {
        object Generate(GeneratorContext context);

        bool CanGenerate(Type type);
    }
}