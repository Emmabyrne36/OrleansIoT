// <auto-generated />
#if !EXCLUDE_GENERATED_CODE
#pragma warning disable 162
#pragma warning disable 219
#pragma warning disable 414
#pragma warning disable 618
#pragma warning disable 649
#pragma warning disable 693
#pragma warning disable 1591
#pragma warning disable 1998
using global::Orleans;

[assembly: global::Orleans.Metadata.FeaturePopulatorAttribute(typeof(OrleansGeneratedCode.OrleansCodeGenOrleansIoT_GrainClassesFeaturePopulator))]
[assembly: global::Orleans.CodeGeneration.OrleansCodeGenerationTargetAttribute("OrleansIoT.GrainClasses, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"), global::Orleans.CodeGeneration.OrleansCodeGenerationTargetAttribute("OrleansIoT.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
namespace OrleansGeneratedCode
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("OrleansCodeGen", "2.0.0.0")]
    internal sealed class OrleansCodeGenOrleansIoT_GrainClassesFeaturePopulator : global::Orleans.Metadata.IFeaturePopulator<global::Orleans.Metadata.GrainInterfaceFeature>, global::Orleans.Metadata.IFeaturePopulator<global::Orleans.Metadata.GrainClassFeature>, global::Orleans.Metadata.IFeaturePopulator<global::Orleans.Serialization.SerializerFeature>
    {
        public void Populate(global::Orleans.Metadata.GrainInterfaceFeature feature)
        {
        }

        public void Populate(global::Orleans.Metadata.GrainClassFeature feature)
        {
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::OrleansIoT.GrainClasses.DeviceGrain)));
        }

        public void Populate(global::Orleans.Serialization.SerializerFeature feature)
        {
            feature.AddKnownType("OrleansIoT.GrainClasses.DeviceGrain,OrleansIoT.GrainClasses", "OrleansIoT.GrainClasses.DeviceGrain");
            feature.AddKnownType("OrleansIoT.GrainClasses.DeviceGrainState,OrleansIoT.GrainClasses", "OrleansIoT.GrainClasses.DeviceGrainState");
            feature.AddKnownType("Microsoft.CodeAnalysis.EmbeddedAttribute,OrleansIoT.Core", "Microsoft.CodeAnalysis.EmbeddedAttribute");
            feature.AddKnownType("System.Runtime.CompilerServices.NullableAttribute", "NullableAttribute");
            feature.AddKnownType("System.Runtime.CompilerServices.NullableContextAttribute", "NullableContextAttribute");
        }
    }
}
#pragma warning restore 162
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 649
#pragma warning restore 693
#pragma warning restore 1591
#pragma warning restore 1998
#endif
