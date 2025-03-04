﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg.Test.Fixtures;
using Xunit;

namespace Xabe.FFmpeg.Test
{
    public class HardwareAcceleration : IClassFixture<StorageFixture>
    {
        private readonly StorageFixture _storageFixture;

        public HardwareAcceleration(StorageFixture storageFixture)
        {
            _storageFixture = storageFixture;
        }

        [RunnableInDebugOnly]
        public async Task ConversionWithHardware()
        {
            string output = _storageFixture.GetTempFileName(FileExtensions.Mp4);

            IConversionResult result = await (await FFmpeg.Conversions.FromSnippet.ConvertWithHardware(Resources.MkvWithAudio, output, HardwareAccelerator.cuvid, VideoCodec.h264_cuvid, VideoCodec.h264_nvenc)).Start();

            var mediaInfo = await FFmpeg.GetMediaInfo(output);
            Assert.Equal(TimeSpan.FromSeconds(10), mediaInfo.Duration);
            Assert.Single(mediaInfo.VideoStreams);
            Assert.Single(mediaInfo.AudioStreams);
            IAudioStream audioStream = mediaInfo.AudioStreams.First();
            IVideoStream videoStream = mediaInfo.VideoStreams.First();
            Assert.NotNull(videoStream);
            Assert.NotNull(audioStream);
            Assert.Equal("h264", videoStream.Codec);
            Assert.Equal("aac", audioStream.Codec);
        }
    }
}
