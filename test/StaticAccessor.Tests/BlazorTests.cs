using BlazorServerApp.Components.Pages;
using Bunit;
using Xunit;

namespace StaticAccessor.Tests
{
    public class BlazorTests : TestContext
    {
        [Fact]
        public void Test()
        {
            var counter = RenderComponent<Counter>();
            var span1 = counter.Find("span:nth-of-type(1)");
            var span2 = counter.Find("span:nth-of-type(2)");

            span1.TextContent.MarkupMatches("True");
            span2.TextContent.MarkupMatches("False");

            counter.Find("button").Click();

            span1.TextContent.MarkupMatches("True");
            span2.TextContent.MarkupMatches("True");
        }

        [Fact]
        public void OwningTest()
        {
            var counter = RenderComponent<OwningCounter>();
            var span1 = counter.Find("span:nth-of-type(1)");
            var span2 = counter.Find("span:nth-of-type(2)");

            counter.Find("button").Click();

            span1.TextContent.MarkupMatches("False");
            span2.TextContent.MarkupMatches("True");
        }
    }
}
