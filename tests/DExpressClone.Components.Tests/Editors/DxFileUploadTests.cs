using Bunit;
using DExpressClone.Components.Editors.DxFileUpload;
using DExpressClone.Components.Interop;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Xunit;

namespace DExpressClone.Components.Tests.Editors;

public class DxFileUploadTests : TestContext
{
    public DxFileUploadTests()
    {
        Services.AddSingleton<JsInteropService>(sp =>
            new JsInteropService(JSInterop.JSRuntime));
        JSInterop.SetupVoid("import", _ => true);
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void RendersDropZone()
    {
        var cut = RenderComponent<DxFileUpload>();

        cut.Find(".dx-fileupload").Should().NotBeNull();
        cut.Find(".dx-fileupload-dropzone").Should().NotBeNull();
    }

    [Fact]
    public void RendersDefaultPlaceholder()
    {
        var cut = RenderComponent<DxFileUpload>();

        cut.Find(".dx-fileupload-placeholder").Should().NotBeNull();
        cut.Find(".dx-fileupload-text").TextContent.Should().Be("Drag and Drop File Here");
    }

    [Fact]
    public void CustomUploadText_RendersInPlaceholder()
    {
        var cut = RenderComponent<DxFileUpload>(parameters => parameters
            .Add(p => p.UploadText, "Drop your images"));

        cut.Find(".dx-fileupload-text").TextContent.Should().Be("Drop your images");
    }

    [Fact]
    public void RendersOverlayInputFile()
    {
        var cut = RenderComponent<DxFileUpload>();

        var input = cut.Find(".dx-fileupload-input");
        input.Should().NotBeNull();
    }

    [Fact]
    public void AcceptParameter_SetsInputAccept()
    {
        var cut = RenderComponent<DxFileUpload>(parameters => parameters
            .Add(p => p.Accept, ".jpg,.png"));

        var input = cut.Find(".dx-fileupload-input");
        input.GetAttribute("accept").Should().Be(".jpg,.png");
    }

    [Fact]
    public void Multiple_WhenTrue_SetsMultipleAttribute()
    {
        var cut = RenderComponent<DxFileUpload>(parameters => parameters
            .Add(p => p.Multiple, true));

        var input = cut.Find(".dx-fileupload-input");
        input.HasAttribute("multiple").Should().BeTrue();
    }

    [Fact]
    public void NoFiles_DoesNotRenderFileList()
    {
        var cut = RenderComponent<DxFileUpload>();

        cut.FindAll(".dx-fileupload-list").Should().BeEmpty();
    }

    [Fact]
    public void CustomDropZoneTemplate_Renders()
    {
        var cut = RenderComponent<DxFileUpload>(parameters => parameters
            .Add(p => p.DropZoneTemplate, builder =>
            {
                builder.AddContent(0, "Custom drop zone");
            }));

        cut.Markup.Should().Contain("Custom drop zone");
        cut.FindAll(".dx-fileupload-placeholder").Should().BeEmpty();
    }

    [Fact]
    public void Disabled_AddsCssClass()
    {
        var cut = RenderComponent<DxFileUpload>(parameters => parameters
            .Add(p => p.Enabled, false));

        cut.Find(".dx-fileupload-dropzone").ClassList.Should().Contain("dx-state-disabled");
    }

    [Fact]
    public void CssClass_AppliedToRoot()
    {
        var cut = RenderComponent<DxFileUpload>(parameters => parameters
            .Add(p => p.CssClass, "my-custom-upload"));

        cut.Find(".dx-fileupload").ClassList.Should().Contain("my-custom-upload");
    }

    [Fact]
    public void RendersSelectFileButton()
    {
        var cut = RenderComponent<DxFileUpload>();

        var btn = cut.Find(".dx-fileupload-select-btn");
        btn.Should().NotBeNull();
        btn.TextContent.Trim().Should().Be("Select File");
    }

    [Fact]
    public void RendersOrHint()
    {
        var cut = RenderComponent<DxFileUpload>();

        cut.Find(".dx-fileupload-hint").TextContent.Should().Be("or");
    }

    [Fact]
    public void DefaultParameters_HaveCorrectValues()
    {
        var cut = RenderComponent<DxFileUpload>();
        var instance = cut.Instance;

        instance.Multiple.Should().BeTrue();
        instance.MaxFileSize.Should().Be(10 * 1024 * 1024);
        instance.MaxFileCount.Should().Be(10);
        instance.AllowImagePreview.Should().BeTrue();
        instance.MaxPreviewFileSize.Should().Be(5 * 1024 * 1024);
        instance.AutoUpload.Should().BeFalse();
    }
}
