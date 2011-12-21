using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace NetflixWrapper.OAuth
{
    public class OAuthRequest
    {
        public enum HttpMethods
        {
            GET,
            POST
        }

        public enum RequestTypes
        {
            ConsumerRequest,
            SignedRequest,
            ProtectedRequest
        }

        protected OAuthConfig config = null;
        protected Uri requestUri = null;
        protected NameValueCollection requestParams = new NameValueCollection();
        protected HttpMethods httpMethod = HttpMethods.GET;
        protected static Random random = new Random();

        protected RequestTypes requestType;

        private OAuthRequest()
        {
        }

        public static OAuthRequest GenerateConsumerRequest(OAuthConfig config, Uri requestUri, NameValueCollection requestParams, HttpMethods httpMethod)
        {
            if (config == null)
                throw new ArgumentNullException("config");
            if (requestUri == null)
                throw new ArgumentNullException("requestUri");
            if (requestParams == null)
                requestParams = new NameValueCollection();
            
            var request = new OAuthRequest();
            request.requestUri = requestUri;
            request.config = config;
            request.httpMethod = httpMethod;
            request.requestType = RequestTypes.ConsumerRequest;

            // Parse any existing URI Params
            request.ParseUriParams(requestUri, requestParams);

            // Assign request params
            request.requestParams = requestParams;

            // Setup required params
            request.requestParams[config.OAuthConsumerKeyKey] = config.OAuthConsumerKey;

            return request;
        }

        public static OAuthRequest GenerateSignedRequest(OAuthConfig config, Uri requestUri, NameValueCollection requestParams, HttpMethods httpMethod)
        {
            var request = GenerateConsumerRequest(config, requestUri, requestParams, httpMethod);

            request.requestParams[config.OAuthVersionKey] = config.OAuthVersion;
            request.requestParams[config.OAuthNonceKey] = request.GenerateNonce();
            request.requestParams[config.OAuthTimeStampKey] = request.GenerateTimeStamp();
            request.requestParams[config.OAuthSignatureMethodKey] = config.OAuthSignatureMethod;

            request.requestType = RequestTypes.SignedRequest;

            return request;
        }


        public static OAuthRequest GenerateProtectedRequest(OAuthConfig config, Uri requestUri, NameValueCollection requestParams, HttpMethods httpMethod)
        {
            var request = GenerateSignedRequest(config, requestUri, requestParams, httpMethod);

            request.requestParams[config.OAuthTokenKey] = config.OAuthToken;

            request.requestType = RequestTypes.ProtectedRequest;

            return request;
        }



        protected const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
        public string UrlEncode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            var result = new StringBuilder();
            foreach (char c in value)
            {
                if (validChars.IndexOf(c) > -1)
                    result.Append(c);
                else
                    result.AppendFormat("%{0:X2}", (int)c);
            }

            return result.ToString();
        }

        protected string NormalizeUri(Uri uri)
        {
            var result = string.Format("{0}://{1}", uri.Scheme, uri.Host);

            if (!uri.IsDefaultPort || !(uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                result += String.Format(":{0}", uri.Port);

            result += uri.AbsolutePath;

            return result;
        }

        protected string NormalizeRequestParams(NameValueCollection requestParams)
        {
            var values = requestParams
                .Cast<string>()
                .Select(
                    key =>
                        new KeyValuePair<string, string>(key, requestParams[key])
                );

            return string.Join("&",
                        values.OrderBy(value => value.Key)
                            .Select(value => string.Format("{0}={1}", value.Key, (value.Value ?? "")))
                            .ToArray()
                    );
        }

        protected void ParseUriParams(Uri uri, NameValueCollection requestParams)
        {
            var uriParams = HttpUtility.ParseQueryString(uri.Query ?? "");

            if (uriParams != null && uriParams.Count > 0)
            {
                foreach (var key in uriParams.AllKeys)
                {
                    requestParams[key] = uriParams[key];
                }
            }
        }



        protected string GenerateTimeStamp()
        {
            TimeSpan timestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            return Convert.ToInt64(timestamp.TotalSeconds).ToString();
        }

        protected string GenerateNonce()
        {
            return random.Next(1111111, 9999999).ToString();
        }


        protected string GenerateSignature()
        {
            // Generate Signature Base
            var normalizedUri = NormalizeUri(requestUri);

            if (!string.IsNullOrWhiteSpace(config.OAuthToken))
                requestParams[config.OAuthTokenKey] = config.OAuthToken;

            var normalizedRequestParams = NormalizeRequestParams(requestParams);

            // Hash
            var signatureBase = string.Format("{0}&{1}&{2}", httpMethod.ToString().ToUpper(), UrlEncode(normalizedUri), UrlEncode(normalizedRequestParams));
            var dataBuffer = System.Text.Encoding.ASCII.GetBytes(signatureBase);

            var hashAlgorithm = new HMACSHA1();
            hashAlgorithm.Key = Encoding.ASCII.GetBytes(string.Format("{0}&{1}", UrlEncode(config.OAuthConsumerSecret), UrlEncode(config.OAuthTokenSecret ?? "")));

            var hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }

        public string SendRequest()
        {
            string normalizedUri = NormalizeUri(requestUri);
            string normalizedRequestParams = NormalizeRequestParams(requestParams);

            if (requestType == RequestTypes.ProtectedRequest || requestType == RequestTypes.SignedRequest)
            {
                var signature = UrlEncode(GenerateSignature());
                normalizedRequestParams += string.Format("&{0}={1}", config.OAuthSignatureKey, signature);
            }

            string oauthUri = normalizedUri;
            if (httpMethod == HttpMethods.GET)
                oauthUri += "?" + normalizedRequestParams;

            Debug.WriteLine(oauthUri, "Sending Request");

            HttpWebRequest request = HttpWebRequest.Create(oauthUri) as HttpWebRequest;
            request.ProtocolVersion = HttpVersion.Version10;



            if (httpMethod == HttpMethods.POST)
            {
                Debug.WriteLine(normalizedRequestParams, "Sending Request (Post Data)");

                var requestBuffer = Encoding.UTF8.GetBytes(normalizedRequestParams);

                request.Method = httpMethod.ToString();
                request.ContentType = "text/plain";
                request.ContentLength = requestBuffer.Length;

                var requestStream = request.GetRequestStream();
                requestStream.Write(requestBuffer, 0, requestBuffer.Length);
                requestStream.Close();
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            StreamReader responseReader = new StreamReader(response.GetResponseStream());
            var responseContent = responseReader.ReadToEnd();
            responseReader.Close();

            return responseContent;
        }
    }
}
