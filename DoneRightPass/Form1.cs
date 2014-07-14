using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Crypto.Asym;
using Crypto.Sym;
using Crypto.Utils;
using DoneRightPass.Model;
using Org.BouncyCastle.Crypto.Parameters;
using Random = Crypto.Utils.Random;

namespace DoneRightPass
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
			List<Package> packages =
						new List<Package>
                        { new Package { Company = "Coho Vineyard", Weight = 25.2, TrackingNumber = 89453312L },
                          new Package { Company = "Lucerne Publishing", Weight = 18.7, TrackingNumber = 89112755L },
                          new Package { Company = "Wingtip Toys", Weight = 6.0, TrackingNumber = 299456122L },
                          new Package { Company = "Adventure Works", Weight = 33.8, TrackingNumber = 4665518773L } };

			// Create a Dictionary of Package objects, 
			// using TrackingNumber as the key.
			Dictionary<long, Package> dictionary =
				packages.ToDictionary(p => p.TrackingNumber);


            var rnd = Random.GetSecureRandom();
            var alice = KeyGen.GenerateKeyPair(rnd);
            const string monkeysAndABanana = "40 000 monkeys and a banana";
            var password = Encoding.UTF8.GetBytes(monkeysAndABanana);
            

            var randomKey = rnd.GenerateSeed(32); //256 random bits
            var randomIv = rnd.GenerateSeed(16); //128 random bits
            var encryptedPassword = AES.Process(password, randomKey, randomIv, true);
            var tempPair = KeyGen.GenerateKeyPair(rnd);
            var commonSecret = ECDH.CalculateCommonSecret(tempPair.Private, alice.Public); // needed to encrypt random key

            var randomIvForKey = rnd.GenerateSeed(16); //128 random bits for key encryption
            var encryptedRandomKey = AES.Process(randomKey, commonSecret, randomIvForKey, true);

            //Ok, now go back. we have 1) Alice's key pair, 2) temp key public (throw private away) 3) encrypted key 4) IV for encrypted key 5) encrypted password 6) IV for encrypted password
            
            //1. We have Alice's private & temp's public. Let's calculate common
            var newCommon = ECDH.CalculateCommonSecret(alice.Private, tempPair.Public);

            //2. Decrypt random key
            var decryptedRandomKey = AES.Process(encryptedRandomKey, newCommon, randomIvForKey, false);

            //3. Decrypt the password
            var decryptedPassword = Encoding.UTF8.GetString(AES.Process(encryptedPassword, decryptedRandomKey, randomIv, false));

            Console.WriteLine(decryptedPassword == monkeysAndABanana);

            TestSharing();

        }

        private void TestSharing()
        {
            var rnd = Random.GetSecureRandom();



            for (int i = 0; i < 100000; i++)
            {
                try
                {
                    var alice = KeyGen.GenerateKeyPair(rnd); // Alice creates new group
                    var groupPair = KeyGen.GenerateKeyPair(rnd); // group key
                    var bob = KeyGen.GenerateKeyPair(rnd); // Bob does not know group's private key
                    // create new password and encrypt it with a random key
                    const string monkeysAndABanana = "40 000 monkeys and a banana";
                    var password = Encoding.UTF8.GetBytes(monkeysAndABanana);

                    var passwordKey = rnd.GenerateSeed(32); //256 random bits
                    var encryptedPassword = Utils.EncryptData(password, passwordKey, rnd);

                    //Alice encrypts password's key with group key

                    var keyEncryptedForGroup = Utils.EncryptDataKeyForPublicKey(groupPair.Public, passwordKey, rnd);

                    //Test 2. Sharing a password for a group

                    var serializedGroupPrivate = EcKeySerializer.SerializeEcPrivateKey(groupPair.Private);
                    var randomKeyForGroupPrivate = rnd.GenerateSeed(32); //256 random bits
                    var encryptedGroupPrivateKey = Utils.EncryptData(serializedGroupPrivate, randomKeyForGroupPrivate, rnd);

                    //2. Encrypt random key for group private key for myself
                    var encryptedGroupRandomKey = Utils.EncryptDataKeyForPublicKey(alice.Public, randomKeyForGroupPrivate, rnd);

                    //3. Decrypt group private
                    var decryptedGroupRandomKey = Utils.DecryptDataKeyWithPrivateKey(alice.Private, encryptedGroupRandomKey);
                    var decryptedGroupPrivateKeyAlice = EcKeySerializer.DeserealizeEcPrivateKey(Utils.DecryptData(encryptedGroupPrivateKey, decryptedGroupRandomKey));

                    //Encrypt group private for bob
                    serializedGroupPrivate = EcKeySerializer.SerializeEcPrivateKey(decryptedGroupPrivateKeyAlice);
                    randomKeyForGroupPrivate = rnd.GenerateSeed(32); //256 random bits
                    encryptedGroupPrivateKey = Utils.EncryptData(serializedGroupPrivate, randomKeyForGroupPrivate, rnd);
                    encryptedGroupRandomKey = Utils.EncryptDataKeyForPublicKey(bob.Public, randomKeyForGroupPrivate, rnd); // by decrypting this key, bob will be available to decrypt the group's private with it

                    // bob decrypts group private
                    decryptedGroupRandomKey = Utils.DecryptDataKeyWithPrivateKey(bob.Private, encryptedGroupRandomKey);
                    var decryptedGroupPrivateKeyBob = EcKeySerializer.DeserealizeEcPrivateKey(Utils.DecryptData(encryptedGroupPrivateKey, decryptedGroupRandomKey));

                    var decryptedKeyForPassword = Utils.DecryptDataKeyWithPrivateKey(decryptedGroupPrivateKeyBob, keyEncryptedForGroup);
                    var decryptedPassword = Encoding.UTF8.GetString(Utils.DecryptData(encryptedPassword, decryptedKeyForPassword));
                    Console.WriteLine(decryptedPassword);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
	class Package
	{
		public string Company { get; set; }
		public double Weight { get; set; }
		public long TrackingNumber { get; set; }
	}
}
