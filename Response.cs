using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            //throw new NotImplementedException();
            string date = DateTime.Now.ToString("ddd, dd MMM yyy HH’:’mm’:’ss ‘GMT’");
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            headerLines.Add("Content-Type: " + contentType);
            headerLines.Add("Content-Length: " + content.Length.ToString());
            headerLines.Add("Date: " + date);
            if (!String.IsNullOrEmpty(redirectoinPath)) {
                headerLines.Add("Redirection-Path: " + redirectoinPath);
            }
            // TODO: Create the response string

            responseString = GetStatusLine(code) + "\r\n" + GetHeaderLines(headerLines) + "\r\n" + content;

        }


        private string GetHeaderLines(List<string> headerlineslist)
        {
            string headerlines = string.Empty;
            foreach (string value in headerlineslist)
            {   
                    headerlines += value + "\r\n";
            }
            return headerlines;
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;

            if (code == StatusCode.BadRequest) {
                statusLine = Configuration.ServerHTTPVersion + " " + (int)StatusCode.BadRequest + " " + StatusCode.BadRequest;
            }

            else if (code == StatusCode.InternalServerError) {
                statusLine = Configuration.ServerHTTPVersion + " " + (int)StatusCode.InternalServerError + " " + StatusCode.InternalServerError;
            }

            else if (code == StatusCode.NotFound) {
                statusLine = Configuration.ServerHTTPVersion + " " + (int)StatusCode.NotFound + " " + StatusCode.NotFound;
            }

            else if (code == StatusCode.OK) {
                statusLine = Configuration.ServerHTTPVersion + " " + (int)StatusCode.OK + " " + StatusCode.OK;
            }

            else if (code == StatusCode.Redirect) {
                statusLine = Configuration.ServerHTTPVersion + " " + (int)StatusCode.Redirect + " " + StatusCode.Redirect;
            }

            return statusLine;
        }
    }
}
