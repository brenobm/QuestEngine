﻿//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v12.0.10.0 (NJsonSchema v9.13.11.0 (Newtonsoft.Json v11.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

namespace QuestEngine.Client
{
    #pragma warning disable

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "12.0.10.0 (NJsonSchema v9.13.11.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial interface IProgressClient
    {
        /// <summary>Update the Player progress on the active quest</summary>
        /// <param name="request">Progress Input</param>
        /// <returns>Progress Output</returns>
        /// <exception cref="QuestEngineException">A server side error occurred.</exception>
        ProgressOutput PostProgress(ProgressInput request);
    
        /// <summary>Update the Player progress on the active quest</summary>
        /// <param name="request">Progress Input</param>
        /// <returns>Progress Output</returns>
        /// <exception cref="QuestEngineException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<ProgressOutput> PostProgressAsync(ProgressInput request, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NSwag", "12.0.10.0 (NJsonSchema v9.13.11.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial interface IStateClient
    {
        /// <summary>Show the state of the player on the active quest</summary>
        /// <param name="playerId">Player Id</param>
        /// <returns>State Output</returns>
        /// <exception cref="QuestEngineException">A server side error occurred.</exception>
        StateOutput GetStatePlayer(string playerId);
    
        /// <summary>Show the state of the player on the active quest</summary>
        /// <param name="playerId">Player Id</param>
        /// <returns>State Output</returns>
        /// <exception cref="QuestEngineException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<StateOutput> GetStatePlayerAsync(string playerId, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
    
    }
    
    

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.13.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ProgressInput 
    {
        [Newtonsoft.Json.JsonProperty("PlayerId", Required = Newtonsoft.Json.Required.Always)]
        public string PlayerId { get; set; }
    
        [Newtonsoft.Json.JsonProperty("PlayerLevel", Required = Newtonsoft.Json.Required.Always)]
        public long PlayerLevel { get; set; }
    
        [Newtonsoft.Json.JsonProperty("ChipAmountBet", Required = Newtonsoft.Json.Required.Always)]
        public long ChipAmountBet { get; set; }
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.13.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class Milestone 
    {
        [Newtonsoft.Json.JsonProperty("MilestoneIndex", Required = Newtonsoft.Json.Required.Always)]
        public int MilestoneIndex { get; set; }
    
        [Newtonsoft.Json.JsonProperty("ChipsAwarded", Required = Newtonsoft.Json.Required.Always)]
        public long ChipsAwarded { get; set; }
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.13.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ProgressOutput 
    {
        [Newtonsoft.Json.JsonProperty("QuestPointsEarned", Required = Newtonsoft.Json.Required.Always)]
        public long QuestPointsEarned { get; set; }
    
        [Newtonsoft.Json.JsonProperty("TotalQuestPercentCompleted", Required = Newtonsoft.Json.Required.Always)]
        public decimal TotalQuestPercentCompleted { get; set; }
    
        [Newtonsoft.Json.JsonProperty("MilestonesCompleted", Required = Newtonsoft.Json.Required.Always)]
        public System.Collections.ObjectModel.ReadOnlyCollection<Milestone> MilestonesCompleted { get; set; } = new System.Collections.ObjectModel.ReadOnlyCollection<Milestone>(new System.Collections.Generic.List<Milestone>());
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.13.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class StateOutput 
    {
        [Newtonsoft.Json.JsonProperty("TotalQuestPercentCompleted", Required = Newtonsoft.Json.Required.Always)]
        public decimal TotalQuestPercentCompleted { get; set; }
    
        [Newtonsoft.Json.JsonProperty("LastMilestoneIndexCompleted", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public long? LastMilestoneIndexCompleted { get; set; }
    
    
    }

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "12.0.10.0 (NJsonSchema v9.13.11.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial class QuestEngineException : System.Exception
    {
        public int StatusCode { get; private set; }

        public string Response { get; private set; }

        public System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> Headers { get; private set; }

        public QuestEngineException(string message, int statusCode, string response, System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Exception innerException) 
            : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + response.Substring(0, response.Length >= 512 ? 512 : response.Length), innerException)
        {
            StatusCode = statusCode;
            Response = response; 
            Headers = headers;
        }

        public override string ToString()
        {
            return string.Format("HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "12.0.10.0 (NJsonSchema v9.13.11.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial class QuestEngineException<TResult> : QuestEngineException
    {
        public TResult Result { get; private set; }

        public QuestEngineException(string message, int statusCode, string response, System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> headers, TResult result, System.Exception innerException) 
            : base(message, statusCode, response, headers, innerException)
        {
            Result = result;
        }
    }

    #pragma warning restore
}