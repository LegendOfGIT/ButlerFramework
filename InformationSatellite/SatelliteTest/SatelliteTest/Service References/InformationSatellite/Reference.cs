﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SatelliteTest.InformationSatellite {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="InformationSatellite.IInformationSatellite")]
    public interface IInformationSatellite {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInformationSatellite/Process", ReplyAction="http://tempuri.org/IInformationSatellite/ProcessResponse")]
        void Process(string template, System.Collections.Generic.Dictionary<string, string> parameters);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInformationSatellite/Process", ReplyAction="http://tempuri.org/IInformationSatellite/ProcessResponse")]
        System.Threading.Tasks.Task ProcessAsync(string template, System.Collections.Generic.Dictionary<string, string> parameters);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IInformationSatelliteChannel : SatelliteTest.InformationSatellite.IInformationSatellite, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class InformationSatelliteClient : System.ServiceModel.ClientBase<SatelliteTest.InformationSatellite.IInformationSatellite>, SatelliteTest.InformationSatellite.IInformationSatellite {
        
        public InformationSatelliteClient() {
        }
        
        public InformationSatelliteClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public InformationSatelliteClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public InformationSatelliteClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public InformationSatelliteClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void Process(string template, System.Collections.Generic.Dictionary<string, string> parameters) {
            base.Channel.Process(template, parameters);
        }
        
        public System.Threading.Tasks.Task ProcessAsync(string template, System.Collections.Generic.Dictionary<string, string> parameters) {
            return base.Channel.ProcessAsync(template, parameters);
        }
    }
}
