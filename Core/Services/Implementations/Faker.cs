using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Core.Entities;
using Core.Services.Exceptions;
using Core.Utilities;

namespace Core.Services.Implementations
{
    public class Faker : IFaker
    {
        private readonly IEnumerable<IValueGenerator> _generators;

        public Faker()
        {
            _generators = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetInterfaces().Contains(typeof(IValueGenerator)) && type.IsClass)
                .Select(type => (IValueGenerator) Activator.CreateInstance(type));
        }

        public T Create<T>()
        {
            return (T) Create(typeof(T));
        }

        private object Create(Type type)
        {
            foreach (var generator in _generators)
                if (generator.CanGenerate(type))
                    return generator.Generate(new GeneratorContext(new Random(), type, this));

            var constructors = GetTypeConstructors(type);
            var instantiatedObject = InstantiateObject(type, constructors);
            InstantiateUninitializedFields(type, instantiatedObject);
            InstantiateProperties(type, instantiatedObject);
            return instantiatedObject;
        }

        private void InstantiateProperties(Type type, object instantiatedObject)
        {
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(instantiatedObject);
                var propertyType = property.PropertyType;
                if (Equals(propertyValue, ReflectionUtilities.GetDefaultValue(propertyType)))
                    property.SetValue(instantiatedObject, Create(propertyType));
            }
        }

        private void InstantiateUninitializedFields(Type type, object instantiatedObject)
        {
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                var fieldValue = field.GetValue(instantiatedObject);
                var fieldType = field.FieldType;
                if (Equals(fieldValue, ReflectionUtilities.GetDefaultValue(fieldType)))
                    field.SetValue(instantiatedObject, Create(fieldType));
            }
        }

        private IEnumerable<ConstructorInfo> GetTypeConstructors(Type type)
        {
            return type.GetConstructors()
                .ToImmutableList()
                .Sort((firstConstructor, secondConstructor) =>
                    secondConstructor.GetParameters().Length - firstConstructor.GetParameters().Length);
        }

        private object InstantiateObject(Type type, IEnumerable<ConstructorInfo> constructors)
        {
            foreach (var constructor in constructors)
            {
                try
                {
                    var parameters = constructor.GetParameters()
                        .Select(parameter => parameter.ParameterType)
                        .Select(Create)
                        .ToArray();
                    return constructor.Invoke(parameters);
                }
                catch
                {
                }
            }

            throw new ObjectInstantiationException("Can't instantiate an object by any of its constructors.");
        }
    }
}