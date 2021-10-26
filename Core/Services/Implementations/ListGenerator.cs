using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Entities;

namespace Core.Services.Implementations
{
    // List<T> - parametrised
    public class ListGenerator : IValueGenerator
    {
        public object Generate(GeneratorContext context)
        {
            // Get parametrised type List<string> -> string type
            var listInnerType = context.TargetType.GetGenericArguments().First();
            // try to create list -> make it generic using type above -> call empty constructor
            var list = (IList) typeof(List<>)
                .MakeGenericType(listInnerType)
                .GetConstructor(Type.EmptyTypes)?
                .Invoke(null);

            // We should use faker
            // public create <T> in the compile time
            // that's why we use private creation
            // GOAL - private object Create(Type type)
            var method = context.Faker.GetType()
                .GetMethod("Create", BindingFlags.NonPublic | BindingFlags.Instance);

            // listSize 2^3 - 2^6
            var listSize = context.Random.Next(1 << 3, 1 << 6);
            // fill the list
            for (var i = 0; i < listSize; i++) list.Add(method.Invoke(context.Faker, new object[] {listInnerType}));

            // return the list
            return list;
        }

        public bool CanGenerate(Type type)
        {
            return type.IsGenericType &&
                   type.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }
    }
}