using System;

using AVFoundation;
using System.Runtime.InteropServices;

namespace NoZ.Platform.IOS {
    class IOSAudioClip : AudioClip {
        public AVAudioFormat Format { get; private set; }

        public AVAudioPcmBuffer Buffer { get; private set; }

        public IOSAudioClip() {
        }

        public IOSAudioClip (int samples, AudioChannelFormat channelFormat, int frequency) : base (samples, channelFormat, frequency) {
            Format = new AVAudioFormat((double)frequency, (uint)(channelFormat == AudioChannelFormat.Mono ? 1 : 2));
            Buffer = new AVAudioPcmBuffer(Format, (uint)samples);
        }

        public override void GetData(short[] data, int offset) {
            throw new NotImplementedException();
        }

        public override void SetData(short[] data, int offset) {
            if(null == Buffer) {
                Format = new AVAudioFormat(AVAudioCommonFormat.PCMFloat32, (double)Frequency, (uint)(ChannelFormat == AudioChannelFormat.Mono ? 1 : 2), true);
                Buffer = new AVAudioPcmBuffer(Format, (uint)SampleCount);
                Buffer.FrameLength = (uint)(SampleCount / sizeof(float));
            }

            unsafe {
                try {             
                    float* f = (float*)Marshal.ReadIntPtr(Buffer.FloatChannelData, 0 * IntPtr.Size);
                    for (int i = 0; i < SampleCount; i++)
                        f[i] = data[i] / 32767.0f;
                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                }                    
            }
        }
    }
}