using FullSerializer;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FirebaseRestClient.Helper
{
    public class SerializerHelper
    {
        public static string Serialize(Type type, object value)
        {
            fsSerializer _serializer = new fsSerializer();
            // serialize the data
            fsData data;
            _serializer.TrySerialize(type, value, out data).AssertSuccessWithoutWarnings();

            // emit the data via JSON
            return fsJsonPrinter.CompressedJson(data);
        }
    }
}
