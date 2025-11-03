using Lister.Lists.Domain.ValueObjects;

namespace Lister.Lists.Tests.ValueObjects;

[TestFixture]
public class ColumnTests
{
    [TestCase("Name", "name")]
    [TestCase("GPA", "gpa")]
    [TestCase("URL Slug", "urlSlug")]
    [TestCase("API URL", "apiUrl")]
    [TestCase("123 Count", "123Count")]
    [TestCase("Status_Code", "statusCode")]
    [TestCase("E-mail Address", "eMailAddress")]
    public void Property_ComputesCamelCaseName(string displayName, string expected)
    {
        var column = new Column { Name = displayName };

        Assert.That(column.Property, Is.EqualTo(expected));
    }
}
