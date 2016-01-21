using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DbModels.DomainModels.SAT
{
public class SATSubregion
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string RukOtdelaEmail { get; set; }
    public string RukFillialaEmail { get; set; }
    public string POROREmail { get; set; }
    public DateTime? Deleted { get; set; }
    public bool Enabled { get; set; }
}
}
