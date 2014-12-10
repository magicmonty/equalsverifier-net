using System;
using EqualsVerifier.TestHelpers;
using NUnit.Framework;

namespace EqualsVerifier.Integration.ExtraFeatures
{
    public class VersionedEntityTest : IntegrationTestBase
    {

        [Test]
        public void WhenInstanceWithAZeroIdDoesNotEqualItself_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<VersionedEntity>().Verify(),
                "object does not equal an identical copy of itself", 
                Warning.IDENTICAL_COPY.ToString());
        }

        [Test]
        public void GivenIdenticalCopyWarningIsSuppressed_WhenInstanceWithANonzeroIdEqualsItself_ThenFail()
        {
            ExpectFailure(
                () => EqualsVerifier.ForType<VersionedEntity>().Suppress(Warning.IDENTICAL_COPY).Verify(),
                "Unnecessary suppression", 
                Warning.IDENTICAL_COPY.ToString());
        }

        [Test]
        public void GivenIdenticalCopyForVersionedEntityWarningIsSuppressed_WhenInstanceWithAZeroIdDoesNotEqualItselfAndInstanceWithANonzeroIdDoes_ThenSucceed()
        {
            EqualsVerifier
                .ForType<VersionedEntity>()
                .Suppress(Warning.IDENTICAL_COPY_FOR_VERSIONED_ENTITY)
                .Verify();
        }

        public sealed class VersionedEntity
        {
            readonly long _id;

            public VersionedEntity(long id)
            {
                _id = id;
            }

            public override bool Equals(object obj)
            {
                var other = obj as VersionedEntity;
                if (other == null)
                    return false;

                return _id == 0L && other._id == 0L 
                    ? object.ReferenceEquals(this, obj) 
                    : _id == other._id;

            }

            public override int GetHashCode()
            {
                return this.GetDefaultHashCode();
            }
        }
    }
}

