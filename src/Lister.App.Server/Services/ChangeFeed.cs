using System.Text.Json;
using System.Threading.Channels;

namespace Lister.App.Server.Services;

public class ChangeFeed
{
    private readonly Channel<string> _channel = Channel.CreateUnbounded<string>();

    public ChannelReader<string> Reader => _channel.Reader;

    public ValueTask PublishAsync(object evt, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(evt);
        return _channel.Writer.WriteAsync(json, cancellationToken);
    }
}