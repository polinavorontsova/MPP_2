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
        private readonly HashSet<Type> _usedTypes = new();

        public Faker()
        {
            // appdomain -> assemblies -> types -> take all generators which are classes
            // -> create real object by type using parametrless constructor-> 
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
            // check if we have a generator for type
            foreach (var generator in _generators)
                if (generator.CanGenerate(type))
                    // generate using context
                    return generator.Generate(new GeneratorContext(new Random(), type, this));

            // A -> (B, C) | B -> A (we use null) 
            // if used -> cyclic dependency
            if (_usedTypes.Contains(type)) return null;

            // add to set
            _usedTypes.Add(type);
            var constructors = GetTypeConstructors(type);
            var instantiatedObject = InstantiateObject(type, constructors);
            InstantiateUninitializedFields(type, instantiatedObject);
            InstantiateProperties(type, instantiatedObject);
            // remove from set
            _usedTypes.Remove(type);
            return instantiatedObject;
        }

        private void InstantiateProperties(Type type, object instantiatedObject)
        {
            var properties = type.GetProperties().Where(property =>
                property.CanWrite && !property.IsSpecialName);
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
            // type -> fields -> public not literal  not reserved name
            var fields = type.GetFields().Where(field =>
                field.IsPublic && !field.IsLiteral && !field.IsSpecialName);
            foreach (var field in fields)
            {
                var fieldValue = field.GetValue(instantiatedObject);
                var fieldType = field.FieldType;
                // We need to know - whether field is init'ed or not
                if (Equals(fieldValue, ReflectionUtilities.GetDefaultValue(fieldType)))
                    field.SetValue(instantiatedObject, Create(fieldType));
            }
        }

        private IEnumerable<ConstructorInfo> GetTypeConstructors(Type type)
        {
            // constructors -> list -> sort by parameters count 5parm 4 3 2
            return type.GetConstructors()
                .ToImmutableList()
                .Sort((firstConstructor, secondConstructor) =>
                    secondConstructor.GetParameters().Length - firstConstructor.GetParameters().Length);
        }

        private object InstantiateObject(Type type, IEnumerable<ConstructorInfo> constructors)
        {
            foreach (var constructor in constructors)
                try
                {
                    // constructor -> parameteres -> type -> call Create(Type type) to generate parameter
                    var parameters = constructor.GetParameters()
                        .Select(parameter => parameter.ParameterType)
                        .Select(Create)
                        .ToArray();
                    // use constructor with params
                    return constructor.Invoke(parameters);
                }
                catch
                {
                }

            // enum/struct - value-types - they have default no-args parameter
            // Enum DIGITS -> BaseType - Enum | struct Family -> BaseType - ValueType
            try
            {
                var baseType = type.BaseType;
                // try to use constructor without params
                if (baseType == typeof(ValueType) || baseType == typeof(Enum)) return Activator.CreateInstance(type);
            }
            catch
            {
            }

            throw new ObjectInstantiationException("Can't instantiate an object by any of its constructors.");
        }
    }
}