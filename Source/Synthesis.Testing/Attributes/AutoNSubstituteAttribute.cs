using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace Synthesis.Testing.Attributes
{
    public class AutoNSubstituteAttribute : AutoDataAttribute
    {
        public AutoNSubstituteAttribute() : base(() => new Fixture().Customize(new AutoNSubstituteCustomization()))
        {

        }
    }
}