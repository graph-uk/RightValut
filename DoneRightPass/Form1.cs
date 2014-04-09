using System;
using System.Text;
using System.Windows.Forms;
using Crypto.Asym;
using Crypto.Sym;
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


            var alice = KeyGen.GenerateKeyPair();
            const string monkeysAndABanana = "40 000 monkeys and a banana";
            var password = Encoding.UTF8.GetBytes(monkeysAndABanana);
            var rnd = Random.GetSecureRandom();

            var randomKey = rnd.GenerateSeed(32); //256 random bits
            var randomIv = rnd.GenerateSeed(16); //128 random bits
            var encryptedPassword = AES.Process(password, randomKey, randomIv, true);
            var tempPair = KeyGen.GenerateKeyPair();
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
            var alice = KeyGen.GenerateKeyPair(); // Alice creates new group
            var groupPair = KeyGen.GenerateKeyPair(); // 1. create group key
            var bob = KeyGen.GenerateKeyPair(); // Bob does not know group's private key
            
            // create new password and encrypt it with a random key
            const string monkeysAndABanana = "40 000 monkeys and a banana";
            var password = Encoding.UTF8.GetBytes(monkeysAndABanana);
            var rnd = Random.GetSecureRandom();

            var randomKey = rnd.GenerateSeed(32); //256 random bits
            var randomIv = rnd.GenerateSeed(16); //128 random bits
            var encryptedPassword = AES.Process(password, randomKey, randomIv, true);

            //Alice encrypts password's key with group key

            var tempPairForPassword = KeyGen.GenerateKeyPair();
            var commonGroupPasswordSecret = ECDH.CalculateCommonSecret(tempPairForPassword.Private, groupPair.Public); // needed to encrypt random key
            var randomIvForGroupPasswordKey = rnd.GenerateSeed(16); //128 random bits for key encryption
            var encryptedRandomKeyForGroup = AES.Process(randomKey, commonGroupPasswordSecret, randomIvForGroupPasswordKey, true);


            //Test 2. Sharing a password for a group


            var serializedGroupPrivate = EcKeySerializer.SerializeEcPrivateKey(groupPair.Private);
            var randomKeyForGroupPrivate = rnd.GenerateSeed(32); //256 random bits
            var randomIvForGroupPrivate = rnd.GenerateSeed(16); //128 random bits
            var encryptedGroupPrivateKey = AES.Process(serializedGroupPrivate, randomKeyForGroupPrivate, randomIvForGroupPrivate, true);

            //2. Encrypt random key for group private key for myself
            var tempPair = KeyGen.GenerateKeyPair();
            var commonSecret = ECDH.CalculateCommonSecret(tempPair.Private, alice.Public);

            var randomIvForKey = rnd.GenerateSeed(16); //128 random bits for key encryption
            var encryptedGroupRandomKey = AES.Process(randomKeyForGroupPrivate, commonSecret, randomIvForKey, true);

            //3. Decrypt group private
            var newCommon = ECDH.CalculateCommonSecret(alice.Private, tempPair.Public);
            var decryptedGroupRandomKey = AES.Process(encryptedGroupRandomKey, newCommon, randomIvForKey, false);
            var decryptedGroupPrivateKeyAlice =
                EcKeySerializer.DeserealizeEcPrivateKey(AES.Process(encryptedGroupPrivateKey, decryptedGroupRandomKey,
                    randomIvForGroupPrivate, false));

            //Encrypt group private for bob
            tempPair = KeyGen.GenerateKeyPair();
            commonSecret = ECDH.CalculateCommonSecret(tempPair.Private, bob.Public);
            randomIvForKey = rnd.GenerateSeed(16); //128 random bits for key encryption
            encryptedGroupRandomKey = AES.Process(randomKeyForGroupPrivate, commonSecret, randomIvForKey, true); // by decrypting this key, bob will be available to decrypt the group's private with it

            // bob decrypts group private
            newCommon = ECDH.CalculateCommonSecret(bob.Private, tempPair.Public);
            decryptedGroupRandomKey = AES.Process(encryptedGroupRandomKey, newCommon, randomIvForKey, false);
            var decryptedGroupPrivateKeyBob =
                EcKeySerializer.DeserealizeEcPrivateKey(AES.Process(encryptedGroupPrivateKey, decryptedGroupRandomKey,
                    randomIvForGroupPrivate, false));
            //...and the password
            var commonKeyForPassword = ECDH.CalculateCommonSecret(decryptedGroupPrivateKeyBob,
                tempPairForPassword.Public);
            var decryptedKeyForPassword = AES.Process(encryptedRandomKeyForGroup, commonKeyForPassword,
                randomIvForGroupPasswordKey, false);
            var decryptedPassword = Encoding.UTF8.GetString(AES.Process(encryptedPassword, decryptedKeyForPassword, randomIv, false));
            Console.WriteLine(decryptedPassword);
        }
    }
}
