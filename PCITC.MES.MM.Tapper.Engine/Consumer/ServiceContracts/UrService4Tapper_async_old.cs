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
    
    [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IUrService4Tapper/InitUnitShift", ReplyAction="http://tempuri.org/IUrService4Tapper/InitUnitShiftResponse")]
    System.IAsyncResult BeginInitUnitShift(string userId, int unitId, System.DateTime begTime, System.DateTime endTime, System.AsyncCallback callback, object asyncState);
    
    bool EndInitUnitShift(System.IAsyncResult result);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUrService4Tapper/InstrumentSample", ReplyAction="http://tempuri.org/IUrService4Tapper/InstrumentSampleResponse")]
    bool InstrumentSample(int unitId, System.DateTime begTime, System.DateTime endTime);
    
    [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IUrService4Tapper/InstrumentSample", ReplyAction="http://tempuri.org/IUrService4Tapper/InstrumentSampleResponse")]
    System.IAsyncResult BeginInstrumentSample(int unitId, System.DateTime begTime, System.DateTime endTime, System.AsyncCallback callback, object asyncState);
    
    bool EndInstrumentSample(System.IAsyncResult result);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUrService4Tapper/SidelinesCaculate", ReplyAction="http://tempuri.org/IUrService4Tapper/SidelinesCaculateResponse")]
    bool SidelinesCaculate(int unitId, System.DateTime begTime, System.DateTime endTime);
    
    [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IUrService4Tapper/SidelinesCaculate", ReplyAction="http://tempuri.org/IUrService4Tapper/SidelinesCaculateResponse")]
    System.IAsyncResult BeginSidelinesCaculate(int unitId, System.DateTime begTime, System.DateTime endTime, System.AsyncCallback callback, object asyncState);
    
    bool EndSidelinesCaculate(System.IAsyncResult result);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUrService4Tapper/ReviseByPreShiftRatio", ReplyAction="http://tempuri.org/IUrService4Tapper/ReviseByPreShiftRatioResponse")]
    bool ReviseByPreShiftRatio(int unitId, System.DateTime begTime, System.DateTime endTime);
    
    [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IUrService4Tapper/ReviseByPreShiftRatio", ReplyAction="http://tempuri.org/IUrService4Tapper/ReviseByPreShiftRatioResponse")]
    System.IAsyncResult BeginReviseByPreShiftRatio(int unitId, System.DateTime begTime, System.DateTime endTime, System.AsyncCallback callback, object asyncState);
    
    bool EndReviseByPreShiftRatio(System.IAsyncResult result);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUrService4Tapper/ReviseBySolutionRatio", ReplyAction="http://tempuri.org/IUrService4Tapper/ReviseBySolutionRatioResponse")]
    bool ReviseBySolutionRatio(int unitId, System.DateTime begTime, System.DateTime endTime);
    
    [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IUrService4Tapper/ReviseBySolutionRatio", ReplyAction="http://tempuri.org/IUrService4Tapper/ReviseBySolutionRatioResponse")]
    System.IAsyncResult BeginReviseBySolutionRatio(int unitId, System.DateTime begTime, System.DateTime endTime, System.AsyncCallback callback, object asyncState);
    
    bool EndReviseBySolutionRatio(System.IAsyncResult result);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUrService4Tapper/CalculateIndex", ReplyAction="http://tempuri.org/IUrService4Tapper/CalculateIndexResponse")]
    bool CalculateIndex(int unitId, System.DateTime begTime, System.DateTime endTime);
    
    [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IUrService4Tapper/CalculateIndex", ReplyAction="http://tempuri.org/IUrService4Tapper/CalculateIndexResponse")]
    System.IAsyncResult BeginCalculateIndex(int unitId, System.DateTime begTime, System.DateTime endTime, System.AsyncCallback callback, object asyncState);
    
    bool EndCalculateIndex(System.IAsyncResult result);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUrService4Tapper/SubmitUnit", ReplyAction="http://tempuri.org/IUrService4Tapper/SubmitUnitResponse")]
    bool SubmitUnit(string userId, int unitId, System.DateTime begTime, System.DateTime endTime);
    
    [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IUrService4Tapper/SubmitUnit", ReplyAction="http://tempuri.org/IUrService4Tapper/SubmitUnitResponse")]
    System.IAsyncResult BeginSubmitUnit(string userId, int unitId, System.DateTime begTime, System.DateTime endTime, System.AsyncCallback callback, object asyncState);
    
    bool EndSubmitUnit(System.IAsyncResult result);
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
    
    public System.IAsyncResult BeginInitUnitShift(string userId, int unitId, System.DateTime begTime, System.DateTime endTime, System.AsyncCallback callback, object asyncState)
    {
        return base.Channel.BeginInitUnitShift(userId, unitId, begTime, endTime, callback, asyncState);
    }
    
    public bool EndInitUnitShift(System.IAsyncResult result)
    {
        return base.Channel.EndInitUnitShift(result);
    }
    
    public bool InstrumentSample(int unitId, System.DateTime begTime, System.DateTime endTime)
    {
        return base.Channel.InstrumentSample(unitId, begTime, endTime);
    }
    
    public System.IAsyncResult BeginInstrumentSample(int unitId, System.DateTime begTime, System.DateTime endTime, System.AsyncCallback callback, object asyncState)
    {
        return base.Channel.BeginInstrumentSample(unitId, begTime, endTime, callback, asyncState);
    }
    
    public bool EndInstrumentSample(System.IAsyncResult result)
    {
        return base.Channel.EndInstrumentSample(result);
    }
    
    public bool SidelinesCaculate(int unitId, System.DateTime begTime, System.DateTime endTime)
    {
        return base.Channel.SidelinesCaculate(unitId, begTime, endTime);
    }
    
    public System.IAsyncResult BeginSidelinesCaculate(int unitId, System.DateTime begTime, System.DateTime endTime, System.AsyncCallback callback, object asyncState)
    {
        return base.Channel.BeginSidelinesCaculate(unitId, begTime, endTime, callback, asyncState);
    }
    
    public bool EndSidelinesCaculate(System.IAsyncResult result)
    {
        return base.Channel.EndSidelinesCaculate(result);
    }
    
    public bool ReviseByPreShiftRatio(int unitId, System.DateTime begTime, System.DateTime endTime)
    {
        return base.Channel.ReviseByPreShiftRatio(unitId, begTime, endTime);
    }
    
    public System.IAsyncResult BeginReviseByPreShiftRatio(int unitId, System.DateTime begTime, System.DateTime endTime, System.AsyncCallback callback, object asyncState)
    {
        return base.Channel.BeginReviseByPreShiftRatio(unitId, begTime, endTime, callback, asyncState);
    }
    
    public bool EndReviseByPreShiftRatio(System.IAsyncResult result)
    {
        return base.Channel.EndReviseByPreShiftRatio(result);
    }
    
    public bool ReviseBySolutionRatio(int unitId, System.DateTime begTime, System.DateTime endTime)
    {
        return base.Channel.ReviseBySolutionRatio(unitId, begTime, endTime);
    }
    
    public System.IAsyncResult BeginReviseBySolutionRatio(int unitId, System.DateTime begTime, System.DateTime endTime, System.AsyncCallback callback, object asyncState)
    {
        return base.Channel.BeginReviseBySolutionRatio(unitId, begTime, endTime, callback, asyncState);
    }
    
    public bool EndReviseBySolutionRatio(System.IAsyncResult result)
    {
        return base.Channel.EndReviseBySolutionRatio(result);
    }
    
    public bool CalculateIndex(int unitId, System.DateTime begTime, System.DateTime endTime)
    {
        return base.Channel.CalculateIndex(unitId, begTime, endTime);
    }
    
    public System.IAsyncResult BeginCalculateIndex(int unitId, System.DateTime begTime, System.DateTime endTime, System.AsyncCallback callback, object asyncState)
    {
        return base.Channel.BeginCalculateIndex(unitId, begTime, endTime, callback, asyncState);
    }
    
    public bool EndCalculateIndex(System.IAsyncResult result)
    {
        return base.Channel.EndCalculateIndex(result);
    }
    
    public bool SubmitUnit(string userId, int unitId, System.DateTime begTime, System.DateTime endTime)
    {
        return base.Channel.SubmitUnit(userId, unitId, begTime, endTime);
    }
    
    public System.IAsyncResult BeginSubmitUnit(string userId, int unitId, System.DateTime begTime, System.DateTime endTime, System.AsyncCallback callback, object asyncState)
    {
        return base.Channel.BeginSubmitUnit(userId, unitId, begTime, endTime, callback, asyncState);
    }
    
    public bool EndSubmitUnit(System.IAsyncResult result)
    {
        return base.Channel.EndSubmitUnit(result);
    }
}
