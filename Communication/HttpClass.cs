using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VTP_Induction
{
    public sealed class WcsHttpServer : IDisposable
    {
        private readonly HttpListener _listener;
        private CancellationTokenSource _cts;

        public event Action<OrderTaskRequest> OrderReceived;
        public event Action<DonePalletRequest> PoReceived;
        public event Action<string> Log;

        public WcsHttpServer(int port)
        {
            _listener = new HttpListener();

            _listener.Prefixes.Add("http://+:" + port + "/ids/orderTask/");
            _listener.Prefixes.Add("http://+:" + port + "/ids/confirm_product/");
        }

        public void Start()
        {
            if (_cts != null) return;

            _cts = new CancellationTokenSource();
            try
            {
                _cts = new CancellationTokenSource();
                _listener.Start();
                if (Log != null) Log("HTTP server started: " + string.Join(", ", _listener.Prefixes));
                Task.Run(() => ListenLoopAsync(_cts.Token));
            }
            catch (HttpListenerException ex)
            {
                if (Log != null) Log("HttpListener start failed: " + ex.Message + " (ErrorCode=" + ex.ErrorCode + ")");
                Stop();
                throw;
            }

            if (Log != null) Log("HTTP server started. Prefix: " + string.Join(", ", _listener.Prefixes));

            Task.Run(() => ListenLoopAsync(_cts.Token));
        }

        public void Stop()
        {
            if (_cts == null) return;

            try { _cts.Cancel(); } catch { }
            try { _listener.Stop(); } catch { }

            try { _cts.Dispose(); } catch { }
            _cts = null;

            if (Log != null) Log("HTTP server stopped.");
        }

        private async Task ListenLoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                HttpListenerContext ctx = null;

                try
                {
                    ctx = await _listener.GetContextAsync();
                }
                catch (HttpListenerException)
                {
                    // Thường xảy ra khi Stop()
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    if (Log != null) Log("ListenLoop error: " + ex.Message);
                    continue;
                }

                // xử lý từng request trên task riêng
                var unused = Task.Run(() => HandleRequestAsync(ctx));
            }
        }

        private async Task HandleRequestAsync(HttpListenerContext ctx)
        {
            HttpListenerRequest req = ctx.Request;
            HttpListenerResponse res = ctx.Response;

            string path = req.Url.AbsolutePath.TrimEnd('/').ToLower();

            try
            {
                if (!string.Equals(req.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
                {
                    await WriteJsonAsync(res, 405, new { code = 1001, desc = "Only POST is allowed" });
                    return;
                }

                string body;
                using (var reader = new StreamReader(req.InputStream, req.ContentEncoding ?? Encoding.UTF8))
                    body = await reader.ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(body))
                {
                    await WriteJsonAsync(res, 400, new { code = 1001, desc = "Empty body" });
                    return;
                }

                if (path == "/ids/ordertask")
                {
                    await HandleOrderTask(body, res);
                    return;
                }
                else if (path == "/ids/confirm_product")
                {
                    await HandlePoOnly(body, res);
                    return;
                }
                else
                {
                    await WriteJsonAsync(res, 404, new { code = 1001, desc = "Not found" });
                    return;
                }
            }
            catch
            {
                WriteJsonSync(res, 500, new { code = 1001, desc = "Server error" });
            }
            finally
            {
                try { res.Close(); } catch { }
            }
        }

        private async Task HandlePoOnly(string body, HttpListenerResponse res)
        {
            DonePalletRequest data;

            try
            {
                data = JsonConvert.DeserializeObject<DonePalletRequest>(body);
            }
            catch (Exception exJson)
            {
                WriteJsonSync(res, 400, new { code = 1001, desc = "Invalid JSON" });
                Log("Invalid JSON: " + exJson.Message);
                return; 
            }

            if (data == null || data.PO_ID <= 0)
            {
                await WriteJsonAsync(res, 400, new { code = 1001, desc = "PO_ID invalid" });
                return;
            }

            if (string.IsNullOrWhiteSpace(data.Line_ID) ||
                string.IsNullOrWhiteSpace(data.Pallet_ID))
            {
                await WriteJsonAsync(res, 400, new { code = 1001, desc = "Line_ID or Pallet_ID invalid" });
                return;
            }

            if (Log != null)
                Log("PO=" + data.PO_ID
                    + ", Line=" + data.Line_ID
                    + ", Pallet=" + data.Pallet_ID
                    + ", Location=" + data.Location
                    + ", From=" + data.From_System);

            if (PoReceived != null)
                PoReceived(data);

            await WriteJsonAsync(res, 200, new { code = 1000, desc = "SUCCESS" });
        }

        private async Task HandleOrderTask(string body, HttpListenerResponse res)
        {
            OrderTaskRequest data;

            try
            {
                data = JsonConvert.DeserializeObject<OrderTaskRequest>(body);
            }
            catch (Exception exJson)
            {
                WriteJsonSync(res, 400, new { code = 1001, desc = "Invalid JSON" });
                Log("Invalid JSON: " + exJson.Message);
                return;
            }

            if (data == null || data.PO_ID <= 0)
            {
                await WriteJsonAsync(res, 400, new { code = 1001, desc = "order_id invalid" });
                return;
            }

            int lineCount = (data.InforDetail == null) ? 0 : data.InforDetail.Count;

            if (Log != null)
                Log("Received order_id=" + data.PO_ID + ", lines=" + lineCount);

            if (OrderReceived != null)
                OrderReceived(data);

            await WriteJsonAsync(res, 200, new { code = 1000, desc = "SUCCESS" });
        }


        private static async Task WriteJsonAsync(HttpListenerResponse res, int statusCode, object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var buffer = Encoding.UTF8.GetBytes(json);

            res.StatusCode = statusCode;
            res.ContentType = "application/json; charset=utf-8";
            res.ContentEncoding = Encoding.UTF8;
            res.ContentLength64 = buffer.Length;

            await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        private static void WriteJsonSync(HttpListenerResponse res, int statusCode, object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var buffer = Encoding.UTF8.GetBytes(json);

            res.StatusCode = statusCode;
            res.ContentType = "application/json; charset=utf-8";
            res.ContentEncoding = Encoding.UTF8;
            res.ContentLength64 = buffer.Length;

            res.OutputStream.Write(buffer, 0, buffer.Length);
        }

        public void Dispose()
        {
            Stop();
            try { _listener.Close(); } catch { }
        }
    }
}
