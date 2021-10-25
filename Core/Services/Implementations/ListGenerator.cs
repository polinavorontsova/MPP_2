using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Entities;

namespace Core.Services.Implementations
{
    public class ListGenerator : IValueGenerator
    {
        public object Generate(GeneratorContext context)
        {
            var listInnerType = context.TargetType.GetGenericArguments().First();
            var list = (IList) typeof(List<>)
                .MakeGenericType(listInnerType)
                .GetConstructor(Type.EmptyTypes)?
                .Invoke(null);

            var method = context.Faker.GetType()
                .GetMethod("Create", BindingFlags.NonPublic | BindingFlags.Instance);

            var listSize = context.Random.Next(1 << 3, 1 << 6);
            for (var i = 0; i < listSize; i++) list.Add(method.Invoke(context.Faker, new object[] {listInnerType}));

            return list;
        }

        public bool CanGenerate(Type type)
        {
            return type.IsGenericType &&
                   type.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }
    }
}