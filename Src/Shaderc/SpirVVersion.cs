// Copyright (c) 2020 - 2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*===================================================================================
	SpirVVersion.cs
====================================================================================*/


using System;


namespace Shaderc
{
    /// <summary>
    /// The known versions of SPIR-V.
    /// </summary>
    public struct SpirVVersion : IEquatable<SpirVVersion>
    {
        internal readonly uint _version;


        public SpirVVersion(uint version)
        {
            _version = version;
        }
        public SpirVVersion(uint major, uint minor)
        {
            _version = (major << 16) + (minor << 8);
        }


        public uint Major => (_version & 0xff0000) >> 16;
        public uint Minor => (_version & 0xff00) >> 8;



        public bool Equals(SpirVVersion other) => _version == other._version;

        public override int GetHashCode() => _version.GetHashCode();

        public override bool Equals(object obj) => obj is SpirVVersion o && Equals(o);

        public override string ToString() => $"SPIR-V {Major}.{Minor}";
        
    }
}
