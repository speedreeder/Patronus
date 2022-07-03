using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Microsoft.EntityFrameworkCore;
using Patronus.DAL;

namespace Patronus.API.Test
{
    public class Customizations
    {
        public class AutoMoqDataAttribute : AutoDataAttribute
        {
            public AutoMoqDataAttribute()
                : base(GetFixture)
            {

            }

            private static IFixture GetFixture()
            {
                return new Fixture().Customize(new TestCustomization());
            }
        }

        public class AutoMoqInlineAutoDataAttribute : CompositeDataAttribute
        {
            public AutoMoqInlineAutoDataAttribute(params object[] arguments)
                : base(new InlineDataAttribute(arguments), new AutoMoqDataAttribute())
            {

            }
        }

        public class DataContextCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                var options = new DbContextOptionsBuilder<PatronusContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

                fixture.Register(() =>
                    new PatronusContext(options));
            }
        }

        public class TestCustomization : DefaultCustomization { }

        public class DefaultCustomization : CompositeCustomization
        {
            public DefaultCustomization(params ICustomization[] customizations)
                : base(new ICustomization[]
                {
                new AutoMoqCustomization(),
                new DataContextCustomization(),
                }.Union(customizations))
            {
            }
        }
    }
}
