using EpplusInteract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models;

namespace TaskManager.Handlers.TaskHandlers.Models.GR_TO.SapReader
{
    public class XlsxSapReader : ISapReader
    {
        public string FilePath { get; set; }

        public List<string> Errors { get; set; }
        public bool Succeed { get; set; }
        public List<SAPRow> Rows { get; set; }

     

        public XlsxSapReader(string path)
        {
            this.FilePath = path;
            this.Errors = new List<string>();
            this.Rows = new List<SAPRow>();
            
        }
        public void Read()
        {
            var wsObjs = EpplusSimpleUniReport.ReadFile(FilePath, null, 2);
            if(wsObjs==null||wsObjs.Count==0)
            {
                throw new Exception($"Не удается прочитать файл {FilePath}");
            }
          
            int index = 0;
            Succeed = true;
           
            foreach (var r in wsObjs)
            {
                index++;
                var sapRow = new SAPRow();
                sapRow.PO = r.Column2.Trim();
                sapRow.POItem = r.Column3.Trim();
                if (string.IsNullOrEmpty(r.Column13))
                {
                    //Succeed = false;
                    Log($"row:{index} value '{r.Column13}' is not a correct MaterialCode");
                    continue;
                }
                else
                {
                    sapRow.MaterialCode = r.Column13;
                }

                sapRow.PODeletionIndicator = r.Column11;

                decimal price;
                if(!decimal.TryParse(r.Column4, out price))
                {
                    Succeed = false;
                    Log($"row:{index} value '{r.Column4}' is not a decimal");
                   
                }
                else
                {
                    sapRow.Price = price;
                }


                decimal qtyOrdered;
                if (!decimal.TryParse(r.Column5, out qtyOrdered))
                {
                    Succeed = false;
                    Log($"row:{index} value '{r.Column5}' is not a decimal");

                }
                else
                {
                    sapRow.QtyOrdered = qtyOrdered;
                }


                decimal grQty;
                if (!decimal.TryParse(r.Column9, out grQty))
                {
                    Succeed = false;
                    Log($"row:{index} value '{r.Column9}' is not a decimal");

                }
                else
                {
                    sapRow.GRQty = grQty;
                }

                Rows.Add(sapRow);




            }
           

            
        }

        private void Log(string message)
        {
            Console.WriteLine(message);
            Errors.Add(message);
            System.Diagnostics.Debug.WriteLine(message);
           
        }

      


    }
}
