using System.IO;
using System.Text;

namespace MyWorksheet.ReportService.Services.Templating.Text.FormatterFramework.javascript;

[JsCallingConvention()]
public class JsDocumentStreamWrapper
{
    private readonly Stream _resultStream;
    private readonly StreamWriter _writer;

    public JsDocumentStreamWrapper(Stream resultStream)
    {
        _resultStream = resultStream;
        _writer = new StreamWriter(resultStream, Encoding.Default, 2024, true);
    }

    [JsCallable]
    public void Write(string text)
    {
        _writer.Write(text);
    }

    [JsCallable]
    public void WriteLine(string text)
    {
        _writer.WriteLine(text);
    }

    internal void Dispose()
    {
        _writer.Dispose();
    }

    internal Stream GetStream()
    {
        return _resultStream;
    }
}