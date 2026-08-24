using Xunit;

namespace CRE132.Tests;

public class NarrationShapeTests
{
    [Fact]
    public void The_web_Block_mirrors_the_LessonDoc_Block_field_for_field()
    {
        var generator = typeof(CRE132.LessonDoc.Block).GetConstructors()[0].GetParameters()
            .Select(p => (p.Name, Type: p.ParameterType.FullName)).ToArray();
        var web = typeof(CRE132.Web.Block).GetConstructors()[0].GetParameters()
            .Select(p => (p.Name, Type: p.ParameterType.FullName)).ToArray();
        Assert.Equal(generator, web);
    }
}
