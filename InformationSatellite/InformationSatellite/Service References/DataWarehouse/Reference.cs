﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InformationSatellite.DataWarehouse {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="DataWarehouse.InformationWarehouse")]
    public interface InformationWarehouse {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/InformationWarehouse/DigInformation", ReplyAction="http://tempuri.org/InformationWarehouse/DigInformationResponse")]
        System.Collections.Generic.Dictionary<string, string[]>[] DigInformation(string question);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/InformationWarehouse/DigInformation", ReplyAction="http://tempuri.org/InformationWarehouse/DigInformationResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, string[]>[]> DigInformationAsync(string question);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/InformationWarehouse/StoreInformation", ReplyAction="http://tempuri.org/InformationWarehouse/StoreInformationResponse")]
        void StoreInformation(System.Collections.Generic.Dictionary<string, string[]> information);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/InformationWarehouse/StoreInformation", ReplyAction="http://tempuri.org/InformationWarehouse/StoreInformationResponse")]
        System.Threading.Tasks.Task StoreInformationAsync(System.Collections.Generic.Dictionary<string, string[]> information);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface InformationWarehouseChannel : InformationWarehouse, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class InformationWarehouseClient : System.ServiceModel.ClientBase<InformationWarehouse>, InformationWarehouse {
        
        public InformationWarehouseClient() {
        }
        
        public InformationWarehouseClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public InformationWarehouseClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public InformationWarehouseClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public InformationWarehouseClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Collections.Generic.Dictionary<string, string[]>[] DigInformation(string question) {
            return base.Channel.DigInformation(question);
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, string[]>[]> DigInformationAsync(string question) {
            return base.Channel.DigInformationAsync(question);
        }
        
        public void StoreInformation(System.Collections.Generic.Dictionary<string, string[]> information) {
            base.Channel.StoreInformation(information);
        }
        
        public System.Threading.Tasks.Task StoreInformationAsync(System.Collections.Generic.Dictionary<string, string[]> information) {
            return base.Channel.StoreInformationAsync(information);
        }
    }
}
