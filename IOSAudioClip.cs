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
using System.Runtime.InteropServices;

namespace NoZ.Platform.IOS
{
    class IOSAudioClip : AudioClip
    {
        public AVAudioFormat Format { get; private set; }

        public AVAudioPcmBuffer Buffer { get; private set; }

        public IOSAudioClip()
        {
        }

        public IOSAudioClip(int samples, AudioChannelFormat channelFormat, int frequency) : base(samples, channelFormat, frequency)
        {
            Format = new AVAudioFormat((double)frequency, (uint)(channelFormat == AudioChannelFormat.Mono ? 1 : 2));
            Buffer = new AVAudioPcmBuffer(Format, (uint)samples);
        }

        public override void GetData(short[] data, int offset)
        {
            throw new NotImplementedException();
        }

        public override void SetData(short[] data, int offset)
        {
            if (null == Buffer)
            {
                Format = new AVAudioFormat(AVAudioCommonFormat.PCMFloat32, (double)Frequency, (uint)(ChannelFormat == AudioChannelFormat.Mono ? 1 : 2), true);
                Buffer = new AVAudioPcmBuffer(Format, (uint)SampleCount);
                Buffer.FrameLength = (uint)(SampleCount / sizeof(float));
            }

            unsafe
            {
                try
                {
                    float* f = (float*)Marshal.ReadIntPtr(Buffer.FloatChannelData, 0 * IntPtr.Size);
                    for (int i = 0; i < SampleCount; i++)
                        f[i] = data[i] / 32767.0f;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}