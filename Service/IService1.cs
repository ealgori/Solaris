using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DbModels.DomainModels.DbTasks;

namespace Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        string GetStatus();

        [OperationContract]
        List<int> GetActiveTasks();

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        [OperationContract]
        int AddTask(int dbTaskId, string fileName, byte[] file);

        //[OperationContract]
        //int AddTask(int dbTaskId, string UserName, List<CommonFunctions.CommonClasses.TaskTranceParam> parameters);
        
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";
        int intValue = 0;

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public int IntValue
        {
            get { return intValue; }
            set { intValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
