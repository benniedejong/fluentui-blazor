using Bunit;
using Xunit;

namespace Microsoft.FluentUI.AspNetCore.Components.Tests.TextArea;
public class FluentTextAreaTests : TestBase
{
    [Fact]
    public void FluentTextArea_Default()
    {
        //Arrange
        string childContent = "<b>render me</b>";
        TextAreaResize? resize = default!;
        string form = default!;
        string dataList = default!;
        int? maxlength = default!;
        int? minlength = default!;
        int? cols = default!;
        int? rows = default!;
        bool? spellcheck = default!;
        FluentInputAppearance appearance = default!;
        var cut = TestContext.RenderComponent<FluentTextArea>(parameters => parameters
            .Add(p => p.Resize, resize)
            .Add(p => p.Form, form)
            .Add(p => p.DataList, dataList)
            .Add(p => p.Maxlength, maxlength)
            .Add(p => p.Minlength, minlength)
            .Add(p => p.Cols, cols)
            .Add(p => p.Rows, rows)
            .Add(p => p.Spellcheck, spellcheck)
            .Add(p => p.Appearance, appearance)
            .AddChildContent(childContent)
        );
        //Act

        //Assert
		cut.Verify();
    }
}






