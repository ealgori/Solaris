using DbModels.DataContext;
using DbModels.DomainModels.ShClone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Handlers.TaskHandlers.Models.AVR.ConditionClasses
{
    public interface IAVRCondition
    {
        bool IsSatisfy(ShAVRs shAvr, Context context);
    }
}
