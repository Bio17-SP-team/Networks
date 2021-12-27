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
            serverSocket.Listen(200);
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

                    byte[] req = new byte[1024];
                    int receivedLen = clientSock.Receive(req);

                    // TODO: break the while loop if receivedLen==0
                    if (receivedLen == 0)
                    {
                        Console.WriteLine("Client: {0} ended the connection", clientSock.RemoteEndPoint);
                        break;                    }

                    // TODO: Create a Request object using received request string

                    string received_request = Encoding.ASCII.GetString(req, 0, receivedLen);
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
            //throw new NotImplementedException();
            string content;
            try
            {
                //TODO: check for bad request 
                if (request.ParseRequest())
                {

                    //TODO: map the relativeURI in request to get the physical path of the resource.
                    //"://docs.microsoft.com/en-us/dotnet/api/system.uri.localpath?view=net-6.0"
                    Uri uri = new Uri(request.relativeURI);
                    string filepath = uri.LocalPath;

                    //TODO: check for redirect
                    GetRedirectionPagePathIFExist(filepath);
                    //TODO: check file exists 
                    //TODO: read the physical file
                    content = LoadDefaultPage(filepath);
                    // Create OK response
                    Response res = new Response(StatusCode.OK, "text/html", content, "");
                    return res;
                }
                else
                {
                    Response res = new Response(StatusCode.BadRequest, "text/html", LoadDefaultPage(Configuration.BadRequestDefaultPageName), "");
                    return res;

                } 
                    
              
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class

                Logger.LogException(ex);

                // TODO: in case of exception, return Internal Server Error. 
                Response res2 = new Response(StatusCode.InternalServerError, "text/html", LoadDefaultPage(Configuration.InternalErrorDefaultPageName), "");
                return res2;
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty

            foreach (KeyValuePair<string, string> entry in Configuration.RedirectionRules)
            {
                // do something with entry.Value or entry.Key
                if (entry.Key == relativePath)
                {
                    return entry.Value;
                }
            }

            return string.Empty;
        }


        // return of this function is the content in handle request function
        // lw bad request, anady di fo2 b Configuration.BadRequestDefaultPageName
        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string

            if (!Directory.Exists(filePath))
            {
                Logger.LogException(new DirectoryNotFoundException());
                return string.Empty;
            }

            // else read file and return its content
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
                    while(file.Peek() >= 0)
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
