﻿using EnvCrypt.Core.EncryptionAlgo.Rsa;
using EnvCrypt.Core.EncryptionAlgo.Rsa.Utils;
using EnvCrypt.Core.Key.Rsa;
using FluentAssertions;
using NUnit.Framework;

namespace EnvCrypt.Core.UnitTest.EncryptionAlgo.Rsa.Utils
{
    [TestFixture]
    public class RsaMaxEncryptionCalcTest
    {
        [Test]
        public void Given_ValidKeyWithOAEP_When_GetMaxBytes_CorrectMaxBytesReturned(
            [Values(384, 2048, 2056)] int keySize)
        {
            // Arrange
            var generator = new RsaKeyGenerator();
            var aNewKey = generator.GetNewKey(new RsaKeyGenerationOptions()
            {
                KeySize = keySize,
                UseOaepPadding = true,
                NewKeyName = "test"
            });

            // Act
            var actualMaxBytes = new RsaMaxEncryptionCalc().GetMaxBytesThatCanBeEncrypted(aNewKey);

            // Assert
            actualMaxBytes.Should().Be(((keySize - 384) / 8) + 7);
        }


        [Test]
        public void Given_ValidKeyWithoutOAEP_When_GetMaxBytes_CorrectMaxBytesReturned(
            [Values(384, 2048, 2056)] int keySize)
        {
            // Arrange
            var generator = new RsaKeyGenerator();
            var aNewKey = generator.GetNewKey(new RsaKeyGenerationOptions()
            {
                KeySize = keySize,
                UseOaepPadding = false,
                NewKeyName = "test"
            });

            // Act
            var actualMaxBytes = new RsaMaxEncryptionCalc().GetMaxBytesThatCanBeEncrypted(aNewKey);

            // Assert
            actualMaxBytes.Should().Be(((keySize - 384) / 8) + 37);
        }
    }
}