using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OTILib.Models;
using OTILib.Sockets;
using OTILib.Events;
using OTILib.Handlers;
using System.Net;
using System.Windows.Interop;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using talk2.Models;

namespace talk2.ViewModels
{
    public class RoomViewModel : ObservableObject
    {
        private ChatClient _client;
        private ClientHandler? _clientHandler;
        private int _roomNo;

        public int RoomNo { get => _roomNo;
        }
        public RoomViewModel(int roomNo)
        {
            _msgs = new ObservableCollection<string>();
            _roomNo = roomNo;

            _client = new ChatClient(IPAddress.Parse("127.0.0.1"), 8080);
            _client.Connected += Connected;
            _client.Disconnected += Disconnected;
            _client.Received += Received;
            _client.RunningStateChanged += RunningStateChanged;

            Connect();
        }

        private string _msg;
        public string Msg
        {
            get => _msg;
            set => SetProperty(ref _msg, value);
        }

        private ObservableCollection<string> _msgs;
        public ObservableCollection<string> Msgs
        {
            get => _msgs;
            set => SetProperty(ref _msgs, value);
        }

        public ObservableCollection<Chat> _chats;
        public ObservableCollection<Chat> Chats
        {
            get => _chats;
            set => SetProperty(ref _chats, value);
        }

        #region socket
        private async void Connect()
        {
            await _client.ConnectAsync(new ConnectionDetails
            {
                RoomId = _roomNo,
                UsrNo = 1,// _loginService.UserInfo.UsrNo,
            });
        }

        private void Connected(object? sender, OTILib.Events.ChatEventArgs e)
        {
            _clientHandler = e.ClientHandler;
        }

        public void Disconnected()
        {
            _client.Close();
        }

        private void Disconnected(object? sender, OTILib.Events.ChatEventArgs e)
        {
            _clientHandler = null;
            // _msgs.Add("서버의 연결이 끊겼습니다.");
        }

        private void RunningStateChanged(bool isRunning)
        {
            // btnConnect.Enabled = !isRunning;
            // btnStop.Enabled = isRunning;
        }

        public void SendMsg()
        {
            _clientHandler?.Send(new ChatHub
            {
                RoomId = _roomNo,
                UsrNo = 1, // _loginService.UserInfo.UsrNo,
                Message = Msg,
            });
            // _chatService.InsertChat(_chatId, Msg);
            Msg = "";
        }

        private void Received(object? sender, OTILib.Events.ChatEventArgs e)
        {
            ChatHub hub = e.Hub;
            // string message = hub.State switch
            // {
            //     ChatState.Connect => $"{hub.UsrId}님이 접속하였습니다.",
            //     ChatState.Disconnect => $"{hub.UsrId}님이 종료하였습니다.",
            //     _ => $"{hub.UsrId}: {hub.Message}"
            // };

            switch (hub.State)
            {
                case ChatState.Connect: break;
                case ChatState.Disconnect: break;
                case ChatState.Invite:
                    _chats.Add(new Chat()
                    {
                        UsrNo = 0,
                        chat = $"{hub.inviter}님이 {hub.invitee}를 초대했습니다.",
                        Align = "Center",
                    });
                    break;
                default:
                    // User me = _loginService.UserInfo;
                    _chats.Add(new Chat()
                    {
                        UsrNo = hub.UsrNo,
                        chat = hub.Message,
                        Align = hub.UsrNo == 1 /*me.UsrNo*/ ? "Right" : "Left",
                    });
                    break;
            }

            //ChatsViewModel ContentViewModel = ShellViewModel.ServiceProvider.GetService(typeof(ChatsViewModel)) as ChatsViewModel;
            //ContentViewModel.LoadRoomList();

            //UserRepository ur = new UserRepository();
            //ur.GetUserList();
        }
        #endregion socket
    }
}
