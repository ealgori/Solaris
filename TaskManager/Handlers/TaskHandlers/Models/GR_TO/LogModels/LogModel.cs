using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models;

namespace TaskManager.Handlers.TaskHandlers.Models.GR_TO.LogModels
{
    public class LogModel
    {
        public string Id { get; set; }
        public List<ShItemModel> ShModels { get; set; }
        public List<SAPItemModel> SAPRows { get; set; }
        public string Message { get; set; }
        public LogStatus Status { get; set; }

        public List<LogModelRow>ToLogModelRows ()
        {
            var models = new List<LogModelRow>();

            var shCount = ShModels == null ? 0 : ShModels.Count;
            var sapCount = SAPRows == null ? 0 : SAPRows.Count;

            var shCounter = 0;
            var sapCounter = 0;

            while (shCount>shCounter||sapCount>sapCounter)
            {
                var model = new LogModelRow();
                if(shCount>0)
                {
                    if(shCount > shCounter)
                    {
                        var shModel = ShModels[shCounter];
                        model.Id = shModel.PO;
                        model.ShItemId = shModel.Id;
                        model.ShItemMaterialCode = shModel.MaterialCode;
                        model.ShItemPrice = shModel.Price;
                        model.ShItemQty = shModel.Qty;
                        model.ShTOFactDate = shModel.TOFactDate;

                        shCounter++;
                    }

                }

                if (sapCount > 0)
                {
                    if (sapCount > sapCounter)
                    {
                        var sapModel = SAPRows[sapCounter];
                        model.SapPOItem = sapModel.POItemId;
                        model.SapOrdered = sapModel.QtyOrdered;
                        model.SapPrice = sapModel.Price;
                        model.SAPMaterialCode = sapModel.MaterialCode;
                        model.SAPGRQty = sapModel.GRQty;

                        sapCounter++;
                    }
                }

                model.Id = this.Id;
                model.Status = this.Status.ToString();
                model.Message = this.Message;

                models.Add(model);


            }




          
            return models;
        }
       
    }

    public enum LogStatus
    {
        Debug, Error, Info
    }


    public class LogModelRow
    {
        public string Id { get; set; }
        public string ShItemId { get; set; }
        public decimal? ShItemQty { get; set; }
        public decimal? ShItemPrice { get; set; }
        public string ShItemMaterialCode { get; set; }
        public string ShItemGR { get; set; }
        public DateTime? ShTOFactDate { get; set; }

        public string SapPOItem { get; set; }
        public decimal SapPrice { get; set; }
        public decimal SapOrdered { get; set; }
        public decimal SAPGRQty { get; set; }
        public string SAPMaterialCode { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }
    }
 
}
