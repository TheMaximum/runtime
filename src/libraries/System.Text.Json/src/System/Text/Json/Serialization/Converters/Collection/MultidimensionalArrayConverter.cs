// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Text.Json.Serialization.Converters
{
    /// <summary>
    /// Converter for multi-dimensional <cref>System.Array</cref>.
    /// </summary>
    /// <typeparam name="TCollection">Type of the multi-dimensional array.</typeparam>
    /// <typeparam name="TElement">Type of the elements inside the array.</typeparam>
    internal sealed class MultidimensionalArrayConverter<TCollection, TElement> : JsonConverter<TCollection>
        where TCollection : IEnumerable
    {
        /// <summary>
        /// Reads the serialized multi-dimensional array from the JSON reader.
        /// </summary>
        /// <param name="reader">JSON reader to deserialize array from.</param>
        /// <param name="typeToConvert">Type to convert serialized array to.</param>
        /// <param name="options">JSON serializer options.</param>
        /// <returns></returns>
        public override TCollection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            TCollection? value = default(TCollection);

            int dimensions = typeof(TCollection).GetArrayRank();
            Type jaggedArrayType = DetermineJaggedArrayType(dimensions);

            object? deserializedArray = JsonSerializer.Deserialize(ref reader, jaggedArrayType);
            if (deserializedArray != null)
            {
                Array jaggedArray = (Array)deserializedArray!;
                int[] dimensionLengths = new int[1] { jaggedArray.Length };
                DetermineDimensionLengths(jaggedArray, dimensions, ref dimensionLengths);

                if (dimensionLengths.Length != dimensions)
                {
                    // Exception: found array dimensions do not match provided TCollection type.
                    ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(typeof(TCollection));
                }

                Array instance = Array.CreateInstance(typeof(TElement), dimensionLengths);
                ReadArrayDimension(jaggedArray, instance, dimensionLengths, Array.Empty<int>());

                value = (TCollection?)(IEnumerable)instance!;
            }

            return value;
        }

        /// <summary>
        /// Writes the provided multi-dimensional array to the JSON writer.
        /// </summary>
        /// <param name="writer">JSON writer to serialize array to.</param>
        /// <param name="value">Multi-dimensional array to serialize.</param>
        /// <param name="options">JSON serializer options.</param>
        public override void Write(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options)
        {
            Array array = (Array)(IEnumerable)value;
            WriteArrayDimension(writer, array, Array.Empty<int>());
        }

        /// <summary>
        /// Reads the provided jagged array into the multi-dimensional array.
        /// </summary>
        /// <param name="jaggedArray">Jagged array to convert.</param>
        /// <param name="value">Multi-dimensional target array.</param>
        /// <param name="dimensionLengths">Array demension lengths.</param>
        /// <param name="indices">Current array dimension indices.</param>
        private void ReadArrayDimension(Array jaggedArray, Array value, int[] dimensionLengths, int[] indices)
        {
            int currentDimension = indices.Length;
            int[] expandedIndices = new int[(indices.Length + 1)];
            Array.Copy(indices, expandedIndices, indices.Length);

            bool inDataDimension = (currentDimension + 1) == value.Rank;

            if (jaggedArray.Length != dimensionLengths[currentDimension])
            {
                // Exception: all items in the same dimension should have the same length.
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(typeof(TCollection));
            }

            for (int i = 0; i < jaggedArray.Length; i++)
            {
                expandedIndices[currentDimension] = i;

                if (inDataDimension)
                {
                    value.SetValue(jaggedArray.GetValue(i), expandedIndices);
                }
                else
                {
                    object? nextDimension = jaggedArray.GetValue(i);
                    if (nextDimension != null && nextDimension.GetType().IsArray)
                    {
                        Array nextDimensionArray = (Array)jaggedArray.GetValue(i)!;
                        ReadArrayDimension(nextDimensionArray, value, dimensionLengths, expandedIndices);
                    }
                }
            }
        }

        /// <summary>
        /// Writes the provided array dimension.
        /// Used to loop through the dimensions to find the data and serialize it.
        /// </summary>
        /// <param name="writer">JSON writer used to serialize the array.</param>
        /// <param name="value">Array to serialize.</param>
        /// <param name="indices">Current array dimension indices.</param>
        private void WriteArrayDimension(Utf8JsonWriter writer, Array value, int[] indices)
        {
            int currentDimension = indices.Length;
            int dimensionLength = value.GetLength(currentDimension);

            int[] expandedIndices = new int[(indices.Length + 1)];
            Array.Copy(indices, expandedIndices, indices.Length);

            bool inDataDimension = (currentDimension + 1) == value.Rank;

            writer.WriteStartArray();

            for (int i = 0; i < dimensionLength; i++)
            {
                expandedIndices[currentDimension] = i;

                if (inDataDimension)
                {
                    object? dataValue = value.GetValue(expandedIndices);
                    TElement elementValue = (TElement)dataValue!;
                    JsonSerializer.Serialize(writer, elementValue, typeof(TElement));
                }
                else
                {
                    WriteArrayDimension(writer, value, expandedIndices);
                }
            }

            writer.WriteEndArray();
        }

        /// <summary>
        /// Determines the jagged array type with the provided dimensions.
        /// </summary>
        /// <param name="dimensions">Dimension count.</param>
        /// <returns>Jagged array type.</returns>
        private Type DetermineJaggedArrayType(int dimensions)
        {
            Type elementType = typeof(TElement);

            for (int dimension = 0; dimension < dimensions; dimension++)
            {
                elementType = elementType.MakeArrayType();
            }

            return elementType;
        }

        /// <summary>
        /// Determines the dimension lengths, based on the provided jagged array.
        /// </summary>
        /// <param name="jaggedArray">Jagged array to determine dimension length with.</param>
        /// <param name="dimensions">Amount of expected dimensions.</param>
        /// <param name="dimensionLengths">Dimension lengths array to fill.</param>
        private void DetermineDimensionLengths(Array jaggedArray, int dimensions, ref int[] dimensionLengths)
        {
            if (jaggedArray.Length == 0)
            {
                return;
            }

            object? firstElement = jaggedArray.GetValue(0);
            if (firstElement != null && firstElement.GetType().IsArray)
            {
                Array firstArray = (Array)firstElement;

                int[] expandedDimensionLengths = new int[(dimensionLengths.Length + 1)];
                Array.Copy(dimensionLengths, expandedDimensionLengths, dimensionLengths.Length);
                expandedDimensionLengths[dimensionLengths.Length] = firstArray.Length;

                dimensionLengths = expandedDimensionLengths;
                DetermineDimensionLengths(firstArray, dimensions, ref dimensionLengths);
            }
        }
    }
}
