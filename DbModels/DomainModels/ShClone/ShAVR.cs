using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DbModels.DomainModels.ShClone
{
    public class ShAVR
    {
        [Key]
        [Display(Name = "Заявка на оплату АВР")]
        public string AVRId { get; set; }

        public string Subregion { get; set; }
        [Display(Name = "Стоимость работ")]
        public decimal? TotalAmount { get; set; }
        public DateTime? WorkStart { get; set; }
        public DateTime? WorkEnd { get; set; }
        [Display(Name = "Подрядчик")]
        public string Subcontractor { get; set; }
        public string Project { get; set; }
        public string Region { get; set; }
        [Display(Name = "Номер Purchase Order")]
        public string PurchaseOrderNumber { get; set; }

        public DateTime? CreationPORdate { get; set; }
        [Display(Name = "Состав работ и смета утверждены рук. отдела")]
        public string RukOtdela { get; set; }
        [Display(Name = "Состав работ и смета утверждены рук. филиала")]
        public string RukFiliala { get; set; }
        public string SubcontractorRef { get; set; }
    }

    //public class ShAVRf : ShAVR
    //{

    //}

    public class ShAVRs : ShAVR
    {

        public DateTime? ObjectCreationDateTime { get; set; }
        [Display(Name = "Номер заявки подрядчика")]
        public string TaskSubcontractorNumber { get; set; }
        [Display(Name = "Состав работ и смета утверждены рук. отдела by")]
        public string DepartmentManager { get; set; }
        [Display(Name = "Состав работ и смета утверждены рук. филиала by")]
        public string BranchManagar { get; set; }
        [Display(Name = "Факт выполнения работ подтверждаю CB")]
        public bool FactVypolneniiaRabotPodtverzhdaiuCB { get; set; }
        [Display(Name = "Факт выполнения работ утвержден рук. отдела by")]
        public string FactVypolneniiaRabotPodtverzhdaiuRukOtd { get; set; }
        [Display(Name = "Заявка ECR адм. получена в обработку")]
        public DateTime? ZayavkaECRAdmPoluchenaVobrabotku { get; set; }
        [Display(Name = "Дата выпуска PO")]
        public DateTime? DataVipuskaPO { get; set; }
        [Display(Name = "POR отправлен в OD")]
        public DateTime? PORotpravlenVOD { get; set; }
        [Display(Name = "Коментарии ECR Adm к заявке")]
        public string KomentariiECRAdmKzayavke { get; set; }
        [Display(Name = "Отправлено в соурсинг")]
        public DateTime? SentToSourcing { get; set; }
        [Display(Name = "Утверждение соурсингом")]
        public DateTime? ApproovedBySoursing { get; set; }
        [Display(Name = "Подписание")]
        public DateTime? Signed { get; set; }
        [Display(Name = "Отправлено подрядчику")]
        public DateTime? SentToSubcontractor { get; set; }
        [Display(Name = "Счет №")]
        public string BillNumber { get; set; }
        [Display(Name = "Счет-фактура №")]
        public string FacruteNumber { get; set; }
        [Display(Name = "КЗД получен")]
        public DateTime? KZDPoluchen { get; set; }
        [Display(Name = "Статус рассмотрения")]
        public string StatusRassmotreniya { get; set; }
        [Display(Name = "Комментарий рассмотрения")]
        public string CommentarijRassmotreniya { get; set; }
        [Display(Name = "Передано в оплату")]
        public DateTime? PeredanoVOplatu { get; set; }
        [Display(Name = "Отправлено подрядчику для корректировки")]
        public DateTime? OtpravlenoPodryadchikuDlyaRassmotreniya { get; set; }
        [Display(Name = "Номер почтового отправления")]
        public string DeliveryNumber { get; set; }
        public string RukRegionApproval { get; set; }
        /// <summary>
        /// 01.12.2015 Поле больше не используется
        /// </summary>
        public bool MSIPApprove { get; set; }
        public string AVRType { get; set; }

        public string Network { get; set; }

        public string ESNetwork { get; set; }
        public string MUSNetwork { get; set; }

        public string ActivityCode { get; set; }

        public DateTime? PaymentDate { get; set; }
        public DateTime? ClearingDate { get; set; }

        public string Year { get; set; }

        public string CreatedByEmail { get; set; }
        public string RukOtdelaEmail { get; set; }
        public int? Priority { get; set; }
        [ExcludeAttribute]
        public virtual ICollection<ShVCRequest> ShVCRequests { get; set; }
        [ExcludeAttribute]
        public virtual ICollection<ShAVRItem> Items { get; set; }

        ///// <summary>
        ///// Готов к опрайсовке в вк.(Либо подрядчик эрикссон, либо уже опрайсован Ксюшей)
        ///// </summary>
        //[ExcludeAttribute]
        //public bool? NeedVCPrice { get; set; }
        ///// <summary>
        ///// Готов к опрайсовке. Не эрикссон, и заморожен
        ///// </summary>
        //[ExcludeAttribute]
        //public bool? NeedPrice { get; set; }
        ///// <summary>
        ///// Либо получили нетворк, либо это ЕС, либо не требуется эта хуета.
        ///// </summary>
        //[ExcludeAttribute]
        //public bool? PorReady { get; set; }
        ///// <summary>
        ///// Катя Опрайсовала и проставила, что готовы оправлять реквест
        ///// </summary>
        //[ExcludeAttribute]
        //public bool? ReadyForRequest { get; set; }

        ///// <summary>
        ///// Флаг того, что требуется отправить МУС
        ///// </summary>
        //[ExcludeAttribute]
        //public bool NeedMus { get; set; }

        /// <summary>
        /// Исключенный атрибут. Каждый раз просчитываетяс заново.
        /// </summary>
        [ExcludeAttribute]
        public Statuses Status { get; set; }
        public bool PorAccesible { get; set; }

        /// <summary>
        /// Флаг участия в расчетах лимитов. проставляется автоматом при заморозке и в дальнейшем не снимается
        /// </summary>
        public bool InCalculations { get; set; }




        public DateTime? PriceNotifySend { get; set; }
        public DateTime? VCPriceNotifySend { get; set; }
        public DateTime? MUSNetworkNotifySend { get; set; }

        /// <summary>
        /// используется на форме Preprice (AVRController), просчитывается в отдельном хендлере после обновления, только для требующих перевыставления.
        /// </summary>
        [Exclude]
        public decimal? TotalVCReexpose { get; set; }

    }

    public enum Statuses
    {
        None,
        NeedPrice,
        NeedVCPrice,
        NeedMus,
        MusSend,
        PorSend,
        ReadyForRequest,
        RequestSend
    }

}
