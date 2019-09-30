/*
  NoZ Game Engine

  Copyright(c) 2019 NoZ Games, LLC

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files(the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions :

  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
  SOFTWARE.
*/

using System;
using AVFoundation;

namespace NoZ.Platform.IOS
{
    class IOSAudioDriver : IAudioDriver
    {
        private const int MaxPlayers = 2;

        private class Player
        {
            public AVAudioPlayerNode Node;
            public Action<AVAudioPlayerNodeCompletionCallbackType> Callback;
            public bool Done;
            public uint Id;
            public uint PlayId;

            public void OnPlayedBack(AVAudioPlayerNodeCompletionCallbackType type)
            {
                Done = true;
            }
        }

        private AVAudioEngine _engine;
        private Player[] _players;
        private AVAudioMixerNode _mixer;

        public float Volume {
            get => _mixer.Volume;
            set => _mixer.Volume = value;
        }

        public IOSAudioDriver()
        {
            _engine = new AVAudioEngine();
            _mixer = new AVAudioMixerNode();

            _engine.AttachNode(_mixer);
            _engine.Connect(_mixer, _engine.MainMixerNode, _engine.MainMixerNode.GetBusOutputFormat(0));

            _players = new Player[MaxPlayers];
            for (int i = 0; i < MaxPlayers; i++)
            {
                var player = new Player();
                player.Callback = player.OnPlayedBack;
                player.Done = true;
                player.Node = new AVAudioPlayerNode();
                player.PlayId = 1;
                player.Id = (uint)i;
                _players[i] = player;

                _engine.AttachNode(player.Node);
                //_engine.Connect(player.Node, _engine.MainMixerNode, _format);                
            }

            _engine.Prepare();
            _engine.StartAndReturnError(out var error);
        }

        public AudioClip CreateClip()
        {
            return new IOSAudioClip();
        }

        public AudioClip CreateClip(int samples, AudioChannelFormat channelFormat, int frequency)
        {
            return new IOSAudioClip(samples, channelFormat, frequency);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool IsPlaying(Voice voice)
        {
            throw new NotImplementedException();
        }

        public Voice Play(AudioClip clip)
        {
            var playerIndex = -1;
            for (int i = 0; i < MaxPlayers && -1 == playerIndex; i++)
            {
                if (!_players[i].Done)
                    continue;
                playerIndex = i;
            }

            if (-1 == playerIndex)
                return Voice.Error;

            var player = _players[playerIndex];

            player.Node.Stop();
            _engine.DisconnectNodeOutput(player.Node);
            _engine.Connect(player.Node, _mixer, ((IOSAudioClip)clip).Format);

            player.PlayId++;
            player.Done = false;
            player.Node.Volume = 1.0f;
            player.Node.ScheduleBuffer(((IOSAudioClip)clip).Buffer, AVAudioPlayerNodeCompletionCallbackType.PlayedBack, player.Callback);
            player.Node.Play();

            return Voice.Create(player.Id, player.PlayId);
        }

        public void DoFrame()
        {
            // Stop any players that are marked done and are still playing
            for (int i = 0; i < MaxPlayers; i++)
            {
                var player = _players[i];
                if (player.Done && player.Node.Playing)
                    player.Node.Stop();
            }
        }
    }
}