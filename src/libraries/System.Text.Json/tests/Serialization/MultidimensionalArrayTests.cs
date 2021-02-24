// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class MultidimensionalArrayTests
    {
        [Fact]
        public static void ReadPrimitiveMultidimensional2dArray()
        {
            string json = "[[1,2,3],[4,5,6]]";
            int[,] i = JsonSerializer.Deserialize<int[,]>(json);

            Assert.Equal(1, i[0, 0]);
            Assert.Equal(2, i[0, 1]);
            Assert.Equal(3, i[0, 2]);
            Assert.Equal(4, i[1, 0]);
            Assert.Equal(5, i[1, 1]);
            Assert.Equal(6, i[1, 2]);
        }

        [Fact]
        public static void ReadClassWithMultidimensionalArray()
        {
            string json = "{\"Array\":[[1,2],[3,4]]}";
            ClassWithArray classWithArray = JsonSerializer.Deserialize<ClassWithArray>(json);

            Assert.Equal(1, classWithArray.Array[0, 0]);
            Assert.Equal(2, classWithArray.Array[0, 1]);
            Assert.Equal(3, classWithArray.Array[1, 0]);
            Assert.Equal(4, classWithArray.Array[1, 1]);
        }

        [Fact]
        public static void ReadClassWithInnerMultidimensionalArray()
        {
            string json = "{\"Inner\":{\"Array\":[[1,2],[3,4]]}}";
            ClassWithPropertyToClassWithArray classWithInnerArray
                = JsonSerializer.Deserialize<ClassWithPropertyToClassWithArray>(json);

            Assert.Equal(1, classWithInnerArray.Inner.Array[0, 0]);
            Assert.Equal(2, classWithInnerArray.Inner.Array[0, 1]);
            Assert.Equal(3, classWithInnerArray.Inner.Array[1, 0]);
            Assert.Equal(4, classWithInnerArray.Inner.Array[1, 1]);
        }

        [Fact]
        public static void ReadClassWithDictionaryMultidimensionalArray()
        {
            string json = "{\"Dictionary\":{\"first\":[[1,2],[3,4]],\"second\":[[5,6],[7,8]]}}";
            ClassWithDictionary classWithDictionaryArray
                = JsonSerializer.Deserialize<ClassWithDictionary>(json);

            Assert.Equal(1, classWithDictionaryArray.Dictionary["first"][0, 0]);
            Assert.Equal(2, classWithDictionaryArray.Dictionary["first"][0, 1]);
            Assert.Equal(3, classWithDictionaryArray.Dictionary["first"][1, 0]);
            Assert.Equal(4, classWithDictionaryArray.Dictionary["first"][1, 1]);

            Assert.Equal(5, classWithDictionaryArray.Dictionary["second"][0, 0]);
            Assert.Equal(6, classWithDictionaryArray.Dictionary["second"][0, 1]);
            Assert.Equal(7, classWithDictionaryArray.Dictionary["second"][1, 0]);
            Assert.Equal(8, classWithDictionaryArray.Dictionary["second"][1, 1]);
        }

        [Fact]
        public static void WritePrimitiveIntMultidimensionalArray()
        {
            int[,] input = new int[2, 2];
            input[0, 0] = 1;
            input[0, 1] = 2;
            input[1, 0] = 3;
            input[1, 1] = 4;

            string json = JsonSerializer.Serialize(input);
            Assert.Equal("[[1,2],[3,4]]", json);
        }

        [Fact]
        public static void WritePrimitiveStringMultidimensionalArray()
        {
            string[,] input = new string[2, 2];
            input[0, 0] = "1";
            input[0, 1] = "2";
            input[1, 0] = "3";
            input[1, 1] = "4";

            string json = JsonSerializer.Serialize(input);
            Assert.Equal("[[\"1\",\"2\"],[\"3\",\"4\"]]", json);
        }

        [Fact]
        public static void WritePrimitiveIntThreeDimensionalArray()
        {
            int[,,] input = new int[2, 2, 2];
            input[0, 0, 0] = 1;
            input[0, 0, 1] = 2;
            input[0, 1, 0] = 3;
            input[0, 1, 1] = 4;
            input[1, 0, 0] = 5;
            input[1, 0, 1] = 6;
            input[1, 1, 0] = 7;
            input[1, 1, 1] = 8;

            string json = JsonSerializer.Serialize(input);
            Assert.Equal("[[[1,2],[3,4]],[[5,6],[7,8]]]", json);
        }

        [Fact]
        public static void WriteClassWithMultidimensionalArray()
        {
            ClassWithArray classWithArray = new ClassWithArray();
            classWithArray.Array = new int[2, 2];
            classWithArray.Array[0, 0] = 1;
            classWithArray.Array[0, 1] = 2;
            classWithArray.Array[1, 0] = 3;
            classWithArray.Array[1, 1] = 4;

            string json = JsonSerializer.Serialize(classWithArray);
            Assert.Equal("{\"Array\":[[1,2],[3,4]]}", json);
        }

        [Fact]
        public static void WriteClassWithInnerMultidimensionalArray()
        {
            ClassWithPropertyToClassWithArray classWithInnerArray
                = new ClassWithPropertyToClassWithArray();
            classWithInnerArray.Inner.Array = new int[2, 2];
            classWithInnerArray.Inner.Array[0, 0] = 1;
            classWithInnerArray.Inner.Array[0, 1] = 2;
            classWithInnerArray.Inner.Array[1, 0] = 3;
            classWithInnerArray.Inner.Array[1, 1] = 4;

            string json = JsonSerializer.Serialize(classWithInnerArray);
            Assert.Equal("{\"Inner\":{\"Array\":[[1,2],[3,4]]}}", json);
        }

        [Fact]
        public static void WriteClassWithDictionaryMultidimensionalArray()
        {
            ClassWithDictionary classWithDictionaryArray
                = new ClassWithDictionary();
            classWithDictionaryArray.Dictionary = new Dictionary<string, int[,]>();
            classWithDictionaryArray.Dictionary["first"] = new int[2, 2];
            classWithDictionaryArray.Dictionary["second"] = new int[2, 2];
            classWithDictionaryArray.Dictionary["first"][0, 0] = 1;
            classWithDictionaryArray.Dictionary["first"][0, 1] = 2;
            classWithDictionaryArray.Dictionary["first"][1, 0] = 3;
            classWithDictionaryArray.Dictionary["first"][1, 1] = 4;
            classWithDictionaryArray.Dictionary["second"][0, 0] = 5;
            classWithDictionaryArray.Dictionary["second"][0, 1] = 6;
            classWithDictionaryArray.Dictionary["second"][1, 0] = 7;
            classWithDictionaryArray.Dictionary["second"][1, 1] = 8;

            string json = JsonSerializer.Serialize(classWithDictionaryArray);
            Assert.Equal("{\"Dictionary\":{\"first\":[[1,2],[3,4]],\"second\":[[5,6],[7,8]]}}", json);
        }

        [Fact]
        public static void ReadPrimitiveMultidimensionalArrayFail()
        {
            // Mismatch between JSON array rank and type to deserialize.
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int[,]>(Encoding.UTF8.GetBytes(@"[[[1,2],[3,4]],[[5,6],[7,8]]]")));

            // Mismatch in dimension lenghts (should be all the same size).
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int[,]>(Encoding.UTF8.GetBytes(@"[[1,2],[4,5,6]]")));
        }

        [Fact]
        public static void NullRootOnRead()
        {
            Assert.Null(JsonSerializer.Deserialize<int[,]>("null"));
        }

        [Fact]
        public static void NullRootOnWrite()
        {
            int[,] value = null;
            string json = JsonSerializer.Serialize(value);
            Assert.Equal("null", json);
        }

        [Fact]
        public static void NullPropertyOnRead()
        {
            string json = "{\"Array\":null}";
            ClassWithArray value = JsonSerializer.Deserialize<ClassWithArray>(json);
            Assert.NotNull(value);
            Assert.Null(value.Array);
        }

        [Fact]
        public static void NullPropertyOnWrite()
        {
            ClassWithArray value = new ClassWithArray();
            string json = JsonSerializer.Serialize(value);
            Assert.Equal("{\"Array\":null}", json);
        }

        private class ClassWithArray
        {
            public int[,] Array { get; set; }
        }

        private class ClassWithDictionary
        {
            public Dictionary<string, int[,]> Dictionary { get; set; }
        }

        private class ClassWithPropertyToClassWithArray
        {
            public ClassWithArray Inner { get; set; } = new ClassWithArray();
        }
    }
}
