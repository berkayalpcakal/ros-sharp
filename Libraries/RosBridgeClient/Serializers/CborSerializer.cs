/*
© Siemens AG, 2020
Author: Berkay Alp Cakal (berkay_alp.cakal.ct@siemens.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

//using Dahomey.Cbor.ObjectModel;
//using Dahomey.Cbor;
using System;
using PeterO.Cbor;
using Dahomey.Cbor;
using Dahomey.Cbor.ObjectModel;

namespace RosSharp.RosBridgeClient
{
    class PeterOCborSerializer : ISerializer
    {
        public DeserializedObject Deserialize(byte[] rawData)
        {
            CBORObject cborObject = CBORObject.DecodeFromBytes(rawData);
            return new PeterOCborDeserializedObject(cborObject);
        }

        public DeserializedObject Deserialize(string json)
        {
            throw new NotImplementedException();
        }

        public T Deserialize<T>(string JsonString)
        {
            throw new NotImplementedException();
        }

        public byte[] Serialize<T>(T obj)
        {
            throw new NotImplementedException();
        }
    }

    internal class PeterOCborDeserializedObject : DeserializedObject
    {
        private CBORObject cborObject;

        internal PeterOCborDeserializedObject(CBORObject _cborObject)
        {
            cborObject = _cborObject;
        }

        internal override string GetProperty(string property)
        {
            CBORObject propertyObject = cborObject[property];
            string json = propertyObject.AsString();
            return json;
        }

        internal override string GetPropertyAsJSON(string property)
        {
            CBORObject propertyObject = cborObject[property];
            string json = propertyObject.ToJSONString();
            return json;
        }
    }


    class DahomeyCborSerializer : ISerializer
    {

        public DeserializedObject Deserialize(string json)
        {
            throw new NotImplementedException();
        }

        public T Deserialize<T>(string JsonString)
        {
            throw new NotImplementedException();
        }

        public byte[] Serialize<T>(T obj)
        {
            throw new NotImplementedException();
        }

        public DeserializedObject Deserialize(byte[] rawData)
        {
            ReadOnlySpan<byte> buffer = new ReadOnlySpan<byte>(rawData);
            CborObject cborObject = Cbor.Deserialize<CborObject>(buffer);
            return new DahomeyCborDeserializedObject(cborObject);
        }
    }

    internal class DahomeyCborDeserializedObject : DeserializedObject
    {
        private CborObject cborObject;

        internal DahomeyCborDeserializedObject(CborObject _cborObject)
        {
            cborObject = _cborObject;
        }

        internal override string GetProperty(string property)
        {
            cborObject.TryGetValue(property, out CborValue cborValue);
            string value = cborValue.Value<string>();
            return value;
        }

        internal override string GetPropertyAsJSON(string property)
        {
            cborObject.TryGetValue(property, out CborValue cborValue);
            string json = cborValue.ToString();
            return json;
        }
    }
}
