using System.IO;
using Crypto.Utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using ProtoBuf;

namespace Crypto.Asym
{
    public class EcKeySerializer
    {
        public static byte[] SerializeEcPublicKey(ICipherParameters parameters)
        {
            var ecParameters = (ECPublicKeyParameters)parameters;
            var point = ecParameters.Q;
            var x = (F2mFieldElement)point.X;
            var y = (F2mFieldElement)point.Y;

            var X = new ECPointCoordinate()
            {
                k1 = x.K1,
                k2 = x.K2,
                k3 = x.K3,
                m = x.M,
                value = x.ToBigInteger().ToByteArray()
            };

            var Y = new ECPointCoordinate()
            {
                k1 = y.K1,
                k2 = y.K2,
                k3 = y.K3,
                m = y.M,
                value = y.ToBigInteger().ToByteArray()
            };

            var stream = new MemoryStream();
            Serializer.Serialize(stream, new ECPublicKey
            {
                x = X,
                y = Y
            });

            return stream.ToArray();
        }

        public static ECPublicKeyParameters DeserializeEcPublicKey(byte[] data)
        {
            var key = Serializer.Deserialize<ECPublicKey>(new MemoryStream(data));

            var x = new F2mFieldElement(key.x.m, key.x.k1, key.x.k2, key.x.k3, new BigInteger(key.x.value));
            var y = new F2mFieldElement(key.y.m, key.y.k1, key.y.k2, key.y.k3, new BigInteger(key.y.value));

            var Q = new F2mPoint(KeyGen.DomParams.Curve, x, y);
            return new ECPublicKeyParameters(Q, KeyGen.DomParams);
        }

        public static byte[] SerializeEcPrivateKey(ICipherParameters parameters)
        {
            var ecParameters = (ECPrivateKeyParameters) parameters;
            var stream = new MemoryStream();
            Serializer.Serialize(stream,new ECPrivateKey() {D = ecParameters.D.ToByteArray()});
            return stream.ToArray();
        }

        public static ECPrivateKeyParameters DeserealizeEcPrivateKey(byte[] data)
        {
            var key = Serializer.Deserialize<ECPrivateKey>(new MemoryStream(data));

            return key.D != null ? new ECPrivateKeyParameters(new BigInteger(key.D), KeyGen.DomParams) : null;
        }
    }
}