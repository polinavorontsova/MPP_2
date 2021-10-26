using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Services;
using Core.Services.Exceptions;
using Core.Services.Implementations;
using NUnit.Framework;

namespace FakerTests
{
    public class FakerTests
    {
        private IFaker _faker;

        [SetUp]
        public void Setup()
        {
            LoadAssemblies();
            _faker = new Faker();
        }

        private void LoadAssemblies()
        {
            Assembly.LoadFrom("../../../../BooleanGenerator/bin/Debug/net5.0/BooleanGenerator.dll");
            Assembly.LoadFrom("../../../../DateGenerator/bin/Debug/net5.0/DateGenerator.dll");
        }

        [TestCase('c')]
        [TestCase(5)]
        [TestCase(5L)]
        [TestCase(5.0d)]
        [TestCase("string")]
        public void Create_BasicValueTypesSuccess<T>(T type)
        {
            Assert.DoesNotThrow(() => _faker.Create<T>());
        }

        [Test]
        public void Create_LoadedFromOtherAssemblyGeneratorSuccess()
        {
            Assert.DoesNotThrow(() =>
            {
                _faker.Create<DateTime>();
                _faker.Create<bool>();
            });
        }

        [Test]
        public void Create_ClassWithNoExplicitConstructorAndField()
        {
            Assert.DoesNotThrow(() =>
            {
                var result = _faker.Create<ClassWithNoExplicitConstructor>();
                Assert.NotNull(result.property);
            });
        }

        [Test]
        public void Create_ClassWithExplicitConstructorAndFieldAndProperty()
        {
            Assert.DoesNotThrow(() =>
            {
                var result = _faker.Create<ClassWithExplicitConstructorAndWriteableProperty>();
                Assert.NotNull(result.property);
                Assert.NotNull(result.Property);
            });
        }

        [Test]
        public void Create_ClassWithPublicButNowWriteableProperty()
        {
            Assert.DoesNotThrow(() =>
            {
                var result = _faker.Create<ClassWithPublicButNotWriteableProperty>();
                Assert.Null(result.Property);
            });
        }

        [Test]
        public void Create_ClassWithPrivateConstructorFail()
        {
            Assert.Throws<ObjectInstantiationException>(() => _faker.Create<ClassWithPrivateConstructor>());
        }

        [Test]
        public void Create_StructWithPrivateConstructorSuccess()
        {
            Assert.DoesNotThrow(() =>
            {
                var result = _faker.Create<StructValueTypeWithPrivateConstructor>();
                Assert.NotNull(result.name);
            });
        }

        [Test]
        public void Create_EnumValueTypeCreationSuccess()
        {
            Assert.DoesNotThrow(() => _faker.Create<SampleEnum>());
        }

        [Test]
        public void Create_NestedListsCreationSuccess()
        {
            Assert.DoesNotThrow(() =>
            {
                var result = _faker.Create<List<List<string>>>();
                Assert.NotZero(result.Count);
                foreach (var list in result)
                {
                    Assert.NotZero(list.Count);
                    foreach (var item in list) Assert.NotNull(item);
                }
            });
        }

        [Test]
        public void Create_ClassWithInnerDependenciesSuccess()
        {
            Assert.DoesNotThrow(() =>
            {
                var result = _faker.Create<OuterClass>();
                Assert.NotNull(result.field);
                Assert.NotNull(result.Field);
                Assert.NotNull(result.inner);
                Assert.NotNull(result.inner.time);
            });
        }

        [Test]
        public void Create_ClassWithTwoLevelDependenciesAndCircularDependencySuccess()
        {
            Assert.DoesNotThrow(() =>
            {
                var result = _faker.Create<A>();
                Assert.NotNull(result.b);
                Assert.NotZero(result.b.cList.Count);
                foreach (var item in result.b.cList)
                {
                    Assert.NotNull(item.stringSample);
                    Assert.Null(item.a);
                }
            });
        }

        private class ClassWithNoExplicitConstructor
        {
            public string property;
        }

        private class ClassWithExplicitConstructorAndWriteableProperty
        {
            public readonly string property;

            public ClassWithExplicitConstructorAndWriteableProperty(string property)
            {
                this.property = property;
            }

            public string Property { get; set; }
        }

        private class ClassWithPublicButNotWriteableProperty
        {
            public string Property { get; }
        }

        private class ClassWithPrivateConstructor
        {
            private ClassWithPrivateConstructor()
            {
            }
        }

        private struct StructValueTypeWithPrivateConstructor
        {
            public readonly string name;
            public bool isDrunk;

            private StructValueTypeWithPrivateConstructor(string name, bool isDrunk)
            {
                this.name = name;
                this.isDrunk = isDrunk;
            }
        }

        private enum SampleEnum
        {
            ONE,
            TWO
        }

        private class OuterClass
        {
            public string field;

            public InnerClass inner;

            public OuterClass(string field)
            {
                Field = field;
            }

            public string Field { get; }
        }

        private class InnerClass
        {
            public DateTime time;
        }

        private class A
        {
            public double volume;
            public B b { get; set; }
        }

        private class B
        {
            public long power { set; get; }
            public List<C> cList { get; set; }
        }

        private class C
        {
            public string stringSample;
            public A a { get; set; }
        }
    }
}