using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;

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
            try
            {
                await writer.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                writer.Complete(ex);
            }
        }


        public static async ValueTask SendAsync(this PipeWriter writer, byte[] buffer, CancellationToken cancellationToken = default)
        {
            try
            {
                await writer.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
            }catch(Exception ex)
            {
                writer.Complete(ex);
            }
        }

    }
}
