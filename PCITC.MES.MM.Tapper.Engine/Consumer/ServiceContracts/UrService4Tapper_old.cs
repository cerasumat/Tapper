﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.0
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------



[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName="IUrService4Tapper")]
public interface IUrService4Tapper
{
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUrService4Tapper/InitUnitShift", ReplyAction="http://tempuri.org/IUrService4Tapper/InitUnitShiftResponse")]
    bool InitUnitShift(string userId, int unitId, System.DateTime begTime, System.DateTime endTime);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUrService4Tapper/InitUnitShift", ReplyAction="http://tempuri.org/IUrService4Tapper/InitUnitShiftResponse")]
    System.Threading.Tasks.Task<bool> InitUnitShiftAsync(string userId, int unitId, System.DateTime begTime, System.DateTime endTime);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUrService4Tapper/InstrumentSample", ReplyAction="http://tempuri.org/IUrService4Tapper/InstrumentSampleResponse")]
    bool InstrumentSample(int unitId, System.DateTime begTime, System.DateTime endTime);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUrService4Tapper/InstrumentSample", ReplyAction="http://tempuri.org/IUrService4Tapper/InstrumentSampleResponse")]
    System.Threading.Tasks.Task<bool> InstrumentSampleAsync(int unitId, System.DateTime begTime, System.DateTime endTime);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUrService4Tapper/SidelinesCaculate", ReplyAction="http://tempuri.org/IUrService4Tapper/SidelinesCaculateResponse")]
    bool SidelinesCaculate(int unitId, System.DateTime begTime, System.DateTime endTime);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUrService4Tapper/SidelinesCaculate", ReplyAction="http://tempuri.org/IUrService4Tapper/SidelinesCaculateResponse")]
    System.Threading.Tasks.Task<bool> SidelinesCaculateAsync(int unitId, System.DateTime begTime, System.DateTime endTime);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUrService4Tapper/ReviseByPreShiftRatio", ReplyAction="http://tempuri.org/IUrService4Tapper/ReviseByPreShiftRatioResponse")]
    bool ReviseByPreShiftRatio(int unitId, System.DateTime begTime, System.DateTime endTime);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUrService4Tapper/ReviseByPreShiftRatio", ReplyAction="http://tempuri.org/IUrService4Tapper/ReviseByPreShiftRatioResponse")]
    System.Threading.Tasks.Task<bool> ReviseByPreShiftRatioAsync(int unitId, System.DateTime begTime, System.DateTime endTime);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUrService4Tapper/ReviseBySolutionRatio", ReplyAction="http://tempuri.org/IUrService4Tapper/ReviseBySolutionRatioResponse")]
    bool ReviseBySolutionRatio(int unitId, System.DateTime begTime, System.DateTime endTime);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUrService4Tapper/ReviseBySolutionRatio", ReplyAction="http://tempuri.org/IUrService4Tapper/ReviseBySolutionRatioResponse")]
    System.Threading.Tasks.Task<bool> ReviseBySolutionRatioAsync(int unitId, System.DateTime begTime, System.DateTime endTime);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUrService4Tapper/CalculateIndex", ReplyAction="http://tempuri.org/IUrService4Tapper/CalculateIndexResponse")]
    bool CalculateIndex(int unitId, System.DateTime begTime, System.DateTime endTime);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUrService4Tapper/CalculateIndex", ReplyAction="http://tempuri.org/IUrService4Tapper/CalculateIndexResponse")]
    System.Threading.Tasks.Task<bool> CalculateIndexAsync(int unitId, System.DateTime begTime, System.DateTime endTime);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUrService4Tapper/SubmitUnit", ReplyAction="http://tempuri.org/IUrService4Tapper/SubmitUnitResponse")]
    bool SubmitUnit(string userId, int unitId, System.DateTime begTime, System.DateTime endTime);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUrService4Tapper/SubmitUnit", ReplyAction="http://tempuri.org/IUrService4Tapper/SubmitUnitResponse")]
    System.Threading.Tasks.Task<bool> SubmitUnitAsync(string userId, int unitId, System.DateTime begTime, System.DateTime endTime);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface IUrService4TapperChannel : IUrService4Tapper, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class UrService4TapperClient : System.ServiceModel.ClientBase<IUrService4Tapper>, IUrService4Tapper
{
    
    public UrService4TapperClient()
    {
    }
    
    public UrService4TapperClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public UrService4TapperClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public UrService4TapperClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public UrService4TapperClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public bool InitUnitShift(string userId, int unitId, System.DateTime begTime, System.DateTime endTime)
    {
        return base.Channel.InitUnitShift(userId, unitId, begTime, endTime);
    }
    
    public System.Threading.Tasks.Task<bool> InitUnitShiftAsync(string userId, int unitId, System.DateTime begTime, System.DateTime endTime)
    {
        return base.Channel.InitUnitShiftAsync(userId, unitId, begTime, endTime);
    }
    
    public bool InstrumentSample(int unitId, System.DateTime begTime, System.DateTime endTime)
    {
        return base.Channel.InstrumentSample(unitId, begTime, endTime);
    }
    
    public System.Threading.Tasks.Task<bool> InstrumentSampleAsync(int unitId, System.DateTime begTime, System.DateTime endTime)
    {
        return base.Channel.InstrumentSampleAsync(unitId, begTime, endTime);
    }
    
    public bool SidelinesCaculate(int unitId, System.DateTime begTime, System.DateTime endTime)
    {
        return base.Channel.SidelinesCaculate(unitId, begTime, endTime);
    }
    
    public System.Threading.Tasks.Task<bool> SidelinesCaculateAsync(int unitId, System.DateTime begTime, System.DateTime endTime)
    {
        return base.Channel.SidelinesCaculateAsync(unitId, begTime, endTime);
    }
    
    public bool ReviseByPreShiftRatio(int unitId, System.DateTime begTime, System.DateTime endTime)
    {
        return base.Channel.ReviseByPreShiftRatio(unitId, begTime, endTime);
    }
    
    public System.Threading.Tasks.Task<bool> ReviseByPreShiftRatioAsync(int unitId, System.DateTime begTime, System.DateTime endTime)
    {
        return base.Channel.ReviseByPreShiftRatioAsync(unitId, begTime, endTime);
    }
    
    public bool ReviseBySolutionRatio(int unitId, System.DateTime begTime, System.DateTime endTime)
    {
        return base.Channel.ReviseBySolutionRatio(unitId, begTime, endTime);
    }
    
    public System.Threading.Tasks.Task<bool> ReviseBySolutionRatioAsync(int unitId, System.DateTime begTime, System.DateTime endTime)
    {
        return base.Channel.ReviseBySolutionRatioAsync(unitId, begTime, endTime);
    }
    
    public bool CalculateIndex(int unitId, System.DateTime begTime, System.DateTime endTime)
    {
        return base.Channel.CalculateIndex(unitId, begTime, endTime);
    }
    
    public System.Threading.Tasks.Task<bool> CalculateIndexAsync(int unitId, System.DateTime begTime, System.DateTime endTime)
    {
        return base.Channel.CalculateIndexAsync(unitId, begTime, endTime);
    }
    
    public bool SubmitUnit(string userId, int unitId, System.DateTime begTime, System.DateTime endTime)
    {
        return base.Channel.SubmitUnit(userId, unitId, begTime, endTime);
    }
    
    public System.Threading.Tasks.Task<bool> SubmitUnitAsync(string userId, int unitId, System.DateTime begTime, System.DateTime endTime)
    {
        return base.Channel.SubmitUnitAsync(userId, unitId, begTime, endTime);
    }
}