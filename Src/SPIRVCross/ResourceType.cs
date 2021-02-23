// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace SharpSPIRVCross
{
    public enum ResourceType
    {
        Unknown = 0,

        UniformBuffer = 1,
        StorageBuffer = 2,
        StageInput = 3,
        StageOutput = 4,
        SubpassInput = 5,
        StorageImage = 6,
        SampledImage = 7,
        AtomicCounter = 8,
        PushConstant = 9,
        SeparateImage = 10,
        SeparateSamplers = 11,
        AccelerationStructure = 12,
    }
}
