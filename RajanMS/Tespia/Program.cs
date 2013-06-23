using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.IO;
using Common.Network;

namespace Tespia
{
    class Program
    {
        static Acceptor sAcceptor;
        static MapleSession sSession;

        static void Main(string[] args)
        {
            Logger.InitConsole("Tespia v111");

            sAcceptor = new Acceptor(IPAddress.Loopback, 8484);
            sAcceptor.OnClientAccepted = OnClientAccepted;
            sAcceptor.Start();
            
            Logger.Write(LogLevel.Info, "Listening on port 8484");

            Console.ReadLine();
        }

        static void OnClientAccepted(Socket socket)
        {
            sSession = new MapleSession(socket);
            sSession.OnPacket = ClientPacket;
            sSession.OnDisconnected = ClientDisconnected;
            sSession.StartClient();
            Logger.Write(LogLevel.Connection, "Accepted client");
        }

        static void ClientPacket(byte[] byffer)
        {
            Logger.Write(LogLevel.DataLoad, BitConverter.ToString(byffer));
            InPacket ip = new InPacket(byffer);
            short opcode = ip.ReadShort();

            switch (opcode)
            {
                case 0x0038: //start hackshield
                    using (OutPacket op = new OutPacket(0)) //0 is opcode lolol
                    {
                        op.WriteInt();
                        op.WriteShort();
                        op.WriteInt(123); //account id
                        op.WriteByte(); //if 0x0A gender request
                        op.WriteBool(true); //is admin
                        op.WriteZero(3);
                        op.WriteMapleString("Rajan");
                        op.WriteByte();
                        op.WriteBool(false); //quiete ban
                        op.WriteLong();
                        op.WriteByte(1);
                        op.WriteLong(); //creation
                        op.WriteInt();
                        op.WriteBool(true); //disable pin
                        op.WriteByte(1); // pic 0 = Register, 1 = Request, 2 = Disable
                        op.WriteLong();

                        sSession.WritePacket(op.ToArray());
                    }
                    break;
                case 0x001F: //request server list!
                    break;
            }

            ip.Dispose();
        }
        static void ClientDisconnected()
        {
            Logger.Write(LogLevel.Connection, "Client disconnected");
        }
    }
}
