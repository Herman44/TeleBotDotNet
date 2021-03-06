﻿using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using TeleBotDotNet.Json;
using TeleBotDotNet.Log;
using TeleBotDotNet.Requests.Methods;
using TeleBotDotNet.Requests.Methods.Bases;
using TeleBotDotNet.Responses.Methods;
using TeleBotDotNet.Responses.Types;

namespace TeleBotDotNet
{
    /// <summary>
    /// API implementation of September 18, 2015.
    /// For documentation, please see https://core.telegram.org/bots/api.
    /// </summary>
    public class TeleBot
    {
        private LogEngine _log;

        public TeleBot(string apiToken, bool enableLog = false)
        {
            ApiToken = apiToken;
            Log.Enabled = enableLog;
        }

        public LogEngine Log => _log ?? (_log = new LogEngine());

        private static string ApiUrl => "https://api.telegram.org";

        private string ApiToken { get; }

        private dynamic ExecuteAction(BaseMethodRequest request)
        {
            var webRequest = WebRequest.Create($"{ApiUrl}/bot{ApiToken}/{request.MethodName}");
            webRequest.Method = "POST";
            var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            boundary = "--" + boundary;

            using (var requestStream = webRequest.GetRequestStream())
            {
                var options = request.Parse();

                // Write the values
                foreach (var parameter in options.Parameters)
                {
                    var buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes($"Content-Disposition: form-data; name=\"{parameter.Key}\"{Environment.NewLine}{Environment.NewLine}");
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(parameter.Value + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                // Write the files
                foreach (var file in options.Files)
                {
                    var buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes($"Content-Disposition: form-data; name=\"{file.Key}\"; filename=\"{file.FileName}\"{Environment.NewLine}");
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes($"Content-Type: {file.ContentType}{Environment.NewLine}{Environment.NewLine}");
                    requestStream.Write(buffer, 0, buffer.Length);
                    new MemoryStream(file.File).CopyTo(requestStream);
                    buffer = Encoding.ASCII.GetBytes(Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                var boundaryBuffer = Encoding.ASCII.GetBytes(boundary + "--");
                requestStream.Write(boundaryBuffer, 0, boundaryBuffer.Length);
            }

            try
            {
                return DecodeWebResponse(webRequest.GetResponse());
            }
            catch (WebException e)
            {
                return DecodeWebResponse(e.Response);
            }
        }

        private static JsonData DecodeWebResponse(WebResponse webResponse)
        {
            using (webResponse)
            using (var responseStream = webResponse.GetResponseStream())
            using (var stream = new MemoryStream())
            {
                responseStream?.CopyTo(stream);
                return JsonData.Deserialize(Encoding.UTF8.GetString(stream.ToArray()));
            }
        }

        public GetMeResponse GetMe(GetMeRequest getMeRequest)
        {
            Log.Info(nameof(GetMe));
            return GetMeResponse.Parse(ExecuteAction(getMeRequest));
        }

        public SendMessageResponse SendMessage(SendMessageRequest sendMessageRequest)
        {
            Log.Info(nameof(SendMessage));
            return SendMessageResponse.Parse(ExecuteAction(sendMessageRequest));
        }

        public ForwardMessageResponse ForwardMessage(ForwardMessageRequest forwardMessageRequest)
        {
            Log.Info(nameof(ForwardMessage));
            return ForwardMessageResponse.Parse(ExecuteAction(forwardMessageRequest));
        }

        public SendPhotoResponse SendPhoto(SendPhotoRequest sendPhotoRequest)
        {
            Log.Info(nameof(SendPhoto));
            return SendPhotoResponse.Parse(ExecuteAction(sendPhotoRequest));
        }

        public SendAudioResponse SendAudio(SendAudioRequest sendAudioRequest)
        {
            Log.Info(nameof(SendAudio));
            return SendAudioResponse.Parse(ExecuteAction(sendAudioRequest));
        }

        public SendDocumentResponse SendDocument(SendDocumentRequest sendDocumentRequest)
        {
            Log.Info(nameof(SendDocument));
            return SendDocumentResponse.Parse(ExecuteAction(sendDocumentRequest));
        }

        public SendStickerResponse SendSticker(SendStickerRequest sendStickerRequest)
        {
            Log.Info(nameof(SendSticker));
            return SendStickerResponse.Parse(ExecuteAction(sendStickerRequest));
        }

        public SendVideoResponse SendVideo(SendVideoRequest sendVideoRequest)
        {
            Log.Info(nameof(SendVideo));
            return SendVideoResponse.Parse(ExecuteAction(sendVideoRequest));
        }

        public SendLocationResponse SendLocation(SendLocationRequest sendLocationRequest)
        {
            Log.Info(nameof(SendLocation));
            return SendLocationResponse.Parse(ExecuteAction(sendLocationRequest));
        }

        public SendChatActionResponse SendChatAction(SendChatActionRequest sendChatActionRequest)
        {
            Log.Info(nameof(SendChatAction));
            return SendChatActionResponse.Parse(ExecuteAction(sendChatActionRequest));
        }

        public GetUserProfilePhotosResponse GetUserProfilePhotos(GetUserProfilePhotosRequest getUserProfilePhotosRequest)
        {
            Log.Info(nameof(GetUserProfilePhotos));
            return GetUserProfilePhotosResponse.Parse(ExecuteAction(getUserProfilePhotosRequest));
        }

        public GetUpdatesResponse GetUpdates(GetUpdatesRequest getUpdatesRequest)
        {
            Log.Info(nameof(GetUpdates));
            return GetUpdatesResponse.Parse(ExecuteAction(getUpdatesRequest));
        }

        public SetWebhookResponse SetWebhook(SetWebhookRequest setWebhookRequest)
        {
            Log.Info(nameof(SetWebhook));
            return SetWebhookResponse.Parse(ExecuteAction(setWebhookRequest));
        }

        public UpdateResponse ConvertWebhookResponse(string json)
        {
            Log.Info(nameof(ConvertWebhookResponse));
            return UpdateResponse.Parse(JsonData.Deserialize(json));
        }

        public GetFileResponse GetFile(GetFileRequest getFileRequest)
        {
            Log.Info(nameof(GetFile));
            return GetFileResponse.Parse(ExecuteAction(getFileRequest));
        }

        /// <summary>
        /// Download a file using a <see cref="GetFileResponse"/> object.
        /// </summary>
        public byte[] DownloadFile(GetFileResponse getFileResponse)
        {
            Log.Info(nameof(DownloadFile));

            if (string.IsNullOrEmpty(getFileResponse?.Result?.FilePath))
            {
                return null;
            }

            using (var client = new WebClient())
            {
                try
                {
                    return client.DownloadData($"{ApiUrl}/file/bot{ApiToken}/{getFileResponse.Result.FilePath}");
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}