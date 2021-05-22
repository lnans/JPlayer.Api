using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JPlayer.Business;
using JPlayer.Business.SystemInfo;
using JPlayer.Data.Dto.SystemInfo;
using JPlayer.Lib.Object;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace JPlayer.Api.Controllers
{
    /// <summary>
    ///     Web sockets available for this API
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class WebSocketsController : ControllerBase
    {
        private readonly byte[] _defaultBuffer = new byte[1024 * 4];
        private readonly ILogger<WebSocketsController> _logger;

        public WebSocketsController(ILogger<WebSocketsController> logger)
        {
            this._logger = logger;
        }

        /// <summary>
        ///     This socket send system information periodically
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("system")]
        public async Task SystemInformationSocket()
        {
            if (this.HttpContext.WebSockets.IsWebSocketRequest)
            {
                using WebSocket webSocket = await this.HttpContext.WebSockets.AcceptWebSocketAsync();
                this._logger.LogInformation($"System info ${GlobalLabelCodes.WebSocketOpen}");

                // Send current system info before starting periodically send
                byte[] initDataState = Encoding.UTF8.GetBytes(SystemInfoWorker.Instance.GetQueue().ToJson());
                await webSocket.SendAsync(new ArraySegment<byte>(initDataState, 0, initDataState.Length), WebSocketMessageType.Text, true, CancellationToken.None);

                // local function added to system worker event
                async void OnSystemEvent(object sender, SystemInfoEventArgs args)
                {
                    byte[] data = Encoding.UTF8.GetBytes(args.Data.ToJson());
                    await webSocket.SendAsync(new ArraySegment<byte>(data, 0, data.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                }

                SystemInfoWorker.Instance.SystemInfoEvent += OnSystemEvent;
                while (webSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(this._defaultBuffer), CancellationToken.None);
                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        SystemInfoWorker.Instance.SystemInfoEvent -= OnSystemEvent;
                        await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, GlobalLabelCodes.WebSocketClose, CancellationToken.None);
                        this._logger.LogInformation($"System info {GlobalLabelCodes.WebSocketClose} by client");
                        return;
                    }
                }

                SystemInfoWorker.Instance.SystemInfoEvent -= OnSystemEvent;
                await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, GlobalLabelCodes.WebSocketClose, CancellationToken.None);
                this._logger.LogInformation($"System info {GlobalLabelCodes.WebSocketClose}");
            }
            else
            {
                this.HttpContext.Response.StatusCode = 400;
            }
        }
    }
}