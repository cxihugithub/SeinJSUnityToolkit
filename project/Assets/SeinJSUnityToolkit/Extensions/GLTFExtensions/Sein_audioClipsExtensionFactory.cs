﻿/**
 * @File   : Sein_audioClipsExtensionFactory.cs
 * @Author : dtysky (dtysky@outlook.com)
 * @Link   : dtysky.moe
 * @Date   : 2019/10/12 0:00:00AM
 */
using System;
using Newtonsoft.Json.Linq;
using GLTF.Math;
using Newtonsoft.Json;
using GLTF.Extensions;
using System.Collections.Generic;
using GLTF.Schema;
using UnityEngine;
using UnityEditor;

namespace SeinJS
{
    public class Sein_audioClipsExtensionFactory : SeinExtensionFactory
    {
        public new static string EXTENSION_NAME = "Sein_audioClips";
        private static Dictionary<ExporterEntry, List<SeinAudioClip>> ENTRY_CLIPS = new Dictionary<ExporterEntry, List<SeinAudioClip>>();
        private static Dictionary<ExporterEntry, Dictionary<AudioClip, string>> ENTRY_URIS = new Dictionary<ExporterEntry, Dictionary<AudioClip, string>>();

        public const string CLIPS = "clips";

        public static int GetClipIndex(ExporterEntry entry, SeinAudioClip clip)
        {
            return ENTRY_CLIPS[entry].IndexOf(clip);
        }

        public override void BeforeExport()
        {
            ENTRY_CLIPS.Clear();
            ENTRY_URIS.Clear();
        }

        public override void Serialize(ExporterEntry entry, Dictionary<string, Extension> extensions, Component component = null)
        {
            if (!ENTRY_CLIPS.ContainsKey(entry))
            {
                ENTRY_CLIPS.Add(entry, new List<SeinAudioClip>());
                ENTRY_URIS.Add(entry, new Dictionary<AudioClip, string>());
            }

            Sein_audioClipsExtension extension = null;
            var source = component as SeinAudioSource;

            if (!extensions.ContainsKey(ExtensionName))
            {
                extension = new Sein_audioClipsExtension();
                AddExtension(extensions, extension);
            }
            else
            {
                extension = (Sein_audioClipsExtension)extensions["Sein_audioClips"];
            }

            var list = ENTRY_CLIPS[entry];
            var uris = ENTRY_URIS[entry];

            foreach(var c in source.clips)
            {
                var clip = c.clip;
                if (list.Contains(clip))
                {
                    continue;
                }

                var newClip = new Sein_audioClipsExtension.AudioClip();
                newClip.name = clip.name;
                newClip.mode = clip.mode;
                newClip.isLazy = clip.isLazy;

                if (uris.ContainsKey(clip.clip))
                {
                    newClip.uri = uris[clip.clip];
                }
                else
                {
                    newClip.uri = SaveAudio(clip.clip);
                }

                extension.clips.Add(newClip);
            }
        }

        private string SaveAudio(AudioClip clip)
        {
            string assetPath = AssetDatabase.GetAssetPath(clip);
            var pathes = ExporterUtils.GetAssetOutPath(clip);
            var exportPath = pathes[0];
            var pathInGlTF = pathes[1];

            FileUtil.CopyFileOrDirectory(assetPath, exportPath);

            return pathInGlTF;
        }

        public override Extension Deserialize(GLTFRoot root, JProperty extensionToken)
        {
            List<Sein_audioClipsExtension.AudioClip> clips = new List<Sein_audioClipsExtension.AudioClip>();

            if (extensionToken != null)
            {
                var clipsToken = extensionToken.Value[CLIPS];

                foreach (var clipToken in clipsToken)
                {
                    var uri = clipToken.Value<string>("uri");
                    var tmp = uri.Split('/');
                    var name = tmp[tmp.Length - 1];

                    clips.Add(new Sein_audioClipsExtension.AudioClip
                    { 
                        mode = clipToken.Value<string>("mode") == "Stream" ? ESeinAudioClipMode.Stream : ESeinAudioClipMode.Buffer,
                        isLazy = clipToken.Value<bool>("isLazy"),
                        uri = uri,
                        name = GlTF_Writer.cleanNonAlphanumeric(name)
                    });
                }
            }

            return new Sein_audioClipsExtension { clips = clips };
        }
    }
}
