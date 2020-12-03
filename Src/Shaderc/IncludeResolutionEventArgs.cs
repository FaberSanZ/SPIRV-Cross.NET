// Copyright (c) 2020 - 2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*===================================================================================
	IncludeResolutionEventArgs.cs
====================================================================================*/


using System;

namespace Shaderc
{
    public class IncludeResolutionEventArgs : EventArgs
    {
        public readonly string Source;
        public readonly string Include;
        public readonly IncludeType Type;
        public string ResolvedName;
        public string ResolvedContent;

        internal IncludeResolutionEventArgs(string source, string include, IncludeType type)
        {
            Source = source;
            Include = include;
            Type = type;
        }
    }
}
