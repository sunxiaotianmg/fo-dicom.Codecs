﻿using System;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;

using Dicom.Imaging.Codec;
using Dicom.Log;

namespace Efferent.Native.Codec
{
    /// <summary>
    /// Implementation of <see cref="TranscoderManager"/> for Windows desktop (.NET) applications.
    /// </summary>
    public sealed class NativeTranscoderManager : TranscoderManager
    {
        #region FIELDS

        /// <summary>
        /// Singleton instance of the <see cref="NativeTranscodeManager"/>.
        /// </summary>
        public static readonly TranscoderManager Instance;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes the static fields of <see cref="NativeTranscodeManager"/>.
        /// </summary>
        static NativeTranscoderManager()
        {
            Instance = new NativeTranscoderManager();
        }

        /// <summary>
        /// Initializes an instance of <see cref="NativeTranscodeManager"/>.
        /// </summary>
        public NativeTranscoderManager()
        {
            this.LoadCodecsImpl(null, null);
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Implementation of method to load codecs from assembly(ies) at the specified <paramref name="path"/> and 
        /// with the specified <paramref name="search"/> pattern.
        /// </summary>
        /// <param name="path">Directory path to codec assemblies.</param>
        /// <param name="search">Search pattern for codec assemblies.</param>
        protected override void LoadCodecsImpl(string path, string search)
        {
            Codecs.Clear();

            var foundAnyCodecs = false;
            var assembly = Assembly.GetExecutingAssembly();
            var codecs = assembly.GetTypes().Where(t => t.IsAssignableFrom(typeof(IDicomCodec)));
            var log = LogManager.GetLogger("Efferent.Native.Codec");

            foreach (var codecType in codecs)
            {
                foundAnyCodecs = true;
                IDicomCodec codec = (IDicomCodec)Activator.CreateInstance(codecType);
                Codecs[codec.TransferSyntax] = codec;
            }

            if (!foundAnyCodecs)
            {
                log.Warn("No Dicom codecs were found after searching {path}\\{wildcard}", path, search);
            }
        }

        #endregion
    }
}
