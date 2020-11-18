using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Reactive.Linq;

namespace TinyServer.ReactiveSocket
{
    public static class SocketExtensions
    {

          public static byte[] ToMessageBuffer(this string source) 
          {
              return Encoding.UTF8.GetBytes($"{source}\n");
          }

          public static string ToMessage(this ReadOnlySequence<byte> source)
          {
               return Encoding.UTF8.GetString(source.ToArray());
          }


        public static async ValueTask SendAsync(this PipeWriter writer, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
              await writer.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
        }


        public static async ValueTask SendAsync(this PipeWriter writer, byte[] buffer, CancellationToken cancellationToken = default)
        {
              await writer.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
        }

        public static ValueTask<FlushResult> SendMessageAsync(this PipeWriter writer, byte[] buffer, CancellationToken cancellationToken = default)
        {
            return writer.WriteAsync(buffer, cancellationToken);
        }


       

    }
}
