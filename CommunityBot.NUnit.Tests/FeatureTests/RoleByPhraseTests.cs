using System;
using CommunityBot.Features.RoleAssignment;
using NUnit.Framework;

namespace CommunityBot.Tests.FeatureTests
{
    public class RoleByPhraseTests
    {
        private static RoleByPhraseSettings GetFilledSettings()
        {
            return new RoleByPhraseSettings
            {
                Phrases =
                {
                    "A",
                    "B"
                },
                RoleIds =
                {
                    111,
                    222,
                    333
                },
                Relations =
                {
                    new RoleByPhraseRelation
                    {
                        PhraseIndex = 0,
                        RoleIdIndex = 0
                    },
                    new RoleByPhraseRelation
                    {
                        PhraseIndex = 1,
                        RoleIdIndex = 0
                    },
                    new RoleByPhraseRelation
                    {
                        PhraseIndex = 0,
                        RoleIdIndex = 1
                    },
                    new RoleByPhraseRelation
                    {
                        PhraseIndex = 2,
                        RoleIdIndex = 2
                    }
                }
            };
        }

        [Test]
        public void Rbp_RemoveRelationTest()
        {
            const int expectedRelCount = 3;
            var rbps = GetFilledSettings();

            rbps.RemoveRelation(0, 0);
            var actual = rbps.Relations.Count;

            Assert.AreEqual(expectedRelCount, actual);
        }

        [Test]
        public void Rbp_RemoveRelationTest_ArgEx()
        {
            var rbps = GetFilledSettings();

            const int validIndex = 0;
            const int invalidIndex = 999;

            Assert.Throws<ArgumentException>(() => rbps.RemoveRelation(validIndex, invalidIndex));
            Assert.Throws<ArgumentException>(() => rbps.RemoveRelation(invalidIndex, validIndex));
            Assert.Throws<ArgumentException>(() => rbps.RemoveRelation(invalidIndex, invalidIndex));
        }

        [Test]
        public void Rbp_RemoveRelationTest_RelNotFoundEx()
        {
            var rbps = GetFilledSettings();
            Assert.Throws<RelationNotFoundException>(() => rbps.RemoveRelation(1,1));
        }

        [Test]
        public void Rbp_AddRelationTest()
        {
            const int expectedRelCount = 5;
            var rbps = GetFilledSettings();

            rbps.CreateRelation(0, 2);
            var acutal = rbps.Relations.Count;

            Assert.AreEqual(expectedRelCount, acutal);
        }

        [Test]
        public void Rbp_AddRelationTest_ArgEx()
        {
            var rbps = GetFilledSettings();

            const int validIndex = 0;
            const int invalidIndex = 999;

            Assert.Throws<ArgumentException>(() => rbps.CreateRelation(validIndex, invalidIndex));
            Assert.Throws<ArgumentException>(() => rbps.CreateRelation(invalidIndex, validIndex));
            Assert.Throws<ArgumentException>(() => rbps.CreateRelation(invalidIndex, invalidIndex));
        }

        [Test]
        public void Rbp_AddRelationTest_RelExists()
        {
            var rbps = GetFilledSettings();
            Assert.Throws<RelationAlreadyExistsException>(() => rbps.CreateRelation(1, 0));
        }

        [Test]
        public void Rbp_AddPhraseTest()
        {
            const int expected = 3;
            var rbps = GetFilledSettings();

            rbps.AddPhrase("CC");
            var actual = rbps.Phrases.Count;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Rbp_AddPhraseTest_InvalidPhraseEx()
        {
            var rbps = GetFilledSettings();
            Assert.Throws<InvalidPhraseException>(() => rbps.AddPhrase(string.Empty));
        }

        [Test]
        public void Rbp_AddPhraseTest_InvalidPhraseEx_TooLong()
        {
            var rbps = GetFilledSettings();
            Assert.Throws<InvalidPhraseException>(() => rbps.AddPhrase(new string('A', Constants.MaxMessageLength)));
        }

        [Test]
        public void Rbp_RemovePhraseTest()
        {
            const int expected = 1;
            var rbps = GetFilledSettings();

            rbps.RemovePhraseByIndex(1);
            var actual = rbps.Phrases.Count;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Rbp_AddRoleTest()
        {
            const int expected = 4;
            var rbps = GetFilledSettings();

            rbps.AddRole(123);
            var actual = rbps.RoleIds.Count;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Rbp_AddRoleTest_AlreadyAddedEx()
        {
            var rbps = GetFilledSettings();
            Assert.Throws<RoleIdAlreadyAddedException>(() => rbps.AddRole(111));
        }

        [Test]
        public void Rbp_RemoveRoleTest()
        {
            const int expected = 2;
            var rbps = GetFilledSettings();

            rbps.RemoveRoleIdByIndex(0);
            var actual = rbps.RoleIds.Count;

            Assert.AreEqual(expected, actual);
        }
    }
}
