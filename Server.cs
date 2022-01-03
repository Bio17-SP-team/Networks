using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        private int portNumber;
        private Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            this.portNumber = portNumber;
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEIP = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(serverEIP);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            Console.WriteLine("Listening...", "/n");
            serverSocket.Listen(2000);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {

                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = this.serverSocket.Accept();
                Console.WriteLine("New client accepted: {0}", clientSocket.RemoteEndPoint);
                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newthread.Start(clientSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period

            Socket clientSock = (Socket)obj;
            clientSock.ReceiveTimeout = 0;
            Console.WriteLine("Welcome to the Server", "/n");

            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request

                    byte[] req = new byte[1024 * 1024];
                    int receivedLen = clientSock.Receive(req);

                    // TODO: break the while loop if receivedLen==0
                    if (receivedLen == 0)
                    {
                        Console.WriteLine("Client: {0} ended the connection", clientSock.RemoteEndPoint);
                        break; 
                    }

                    // TODO: Create a Request object using received request string
                    string received_request = Encoding.ASCII.GetString(req);
                    Request request = new Request(received_request);
                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);
                    // TODO: Send Response back to client 
                    clientSock.Send(Encoding.ASCII.GetBytes(response.ResponseString));
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientSock.Close();
        }

        Response HandleRequest(Request request)
        {
            try
            {
                //throw new NotImplementedException();
                //TODO: check for bad request 
                if (!request.ParseRequest())
                    return new Response(StatusCode.BadRequest, "text/html", LoadDefaultPage(Configuration.BadRequestDefaultPageName), "");
                //TODO: map the relativeURI in request to get the physical path of the resource.
                string filepath = Path.Combine(Configuration.RootPath, request.relativeURI);
                //TODO: check for redirect
                string redirectionPath = GetRedirectionPagePathIFExist(request.relativeURI);
                if (redirectionPath != "")
                {
                    return new Response(StatusCode.Redirect, "text/html", LoadDefaultPage(Configuration.RedirectionDefaultPageName), redirectionPath);
                }

                //TODO: check file exists 
                if (!File.Exists(filepath))
                {
                    return new Response(StatusCode.NotFound, "text/html", LoadDefaultPage(Configuration.NotFoundDefaultPageName), "");
                }

                //TODO: read the physical file
                StreamReader reader = new StreamReader(filepath);
                string fileContent = reader.ReadToEnd();
                reader.Close();

                // Create OK response
                Response res = new Response(StatusCode.OK, "text/html", fileContent, "");
                return res;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error.
                return new Response(StatusCode.InternalServerError, "text/html", LoadDefaultPage(Configuration.InternalErrorDefaultPageName), "");
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty


            if (Configuration.RedirectionRules.ContainsKey(relativePath))
            {
                return Configuration.RedirectionRules[relativePath];
            }

            return string.Empty;
        }


        // return of this function is the content in handle request function
        // lw bad request, anady di fo2 b Configuration.BadRequestDefaultPageName
        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string

            if (!File.Exists(filePath))
            {
                Logger.LogException(new FileNotFoundException());
                return string.Empty;
            }

            // else read file and return its content

            //StreamReader reader = new StreamReader(filePath);
            //string file = reader.ReadToEnd();
            //reader.Close();
            //return file;

            string text = System.IO.File.ReadAllText(@filePath);
            return text;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                Configuration.RedirectionRules = new Dictionary<string, string>();
                using (StreamReader file = new StreamReader(filePath))
                {
                    while(!file.EndOfStream)
                    {
                        string filepathread = file.ReadLine();
                        string[]  header_linessplitresp = filepathread.Split(',');
                        // then fill Configuration.RedirectionRules dictionary 
                        Configuration.RedirectionRules.Add(header_linessplitresp[0], header_linessplitresp[1]);
                    }
                }

                //throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
