using DbModels.DataContext;
using DbModels.DataContext.Repositories;
using DbModels.DomainModels.ShClone;
using MailProcessing;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.AVR.ImportClasses;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.AVR
{
	public static class CreateVCRequest
	{

		#region templates
		private static string NotifyMainTemplate = @"
<html>

<head>
<meta http-equiv=Content-Type content=""text/html; charset=windows-1251"">
<meta name=Generator content=""Microsoft Word 14 (filtered)"">
<style>
<!--
 /* Font Definitions */
 @font-face
	{{font-family:Wingdings;
	panose-1:5 0 0 0 0 0 0 0 0 0;}}
@font-face
	{{font-family:Wingdings;
	panose-1:5 0 0 0 0 0 0 0 0 0;}}
@font-face
	{{font-family:Calibri;
	panose-1:2 15 5 2 2 2 4 3 2 4;}}
 /* Style Definitions */
 p.MsoNormal, li.MsoNormal, div.MsoNormal
	{{margin-top:0cm;
	margin-right:0cm;
	margin-bottom:10.0pt;
	margin-left:0cm;
	line-height:115%;
	font-size:11.0pt;
	font-family:""Calibri"",""sans-serif"";}}
p.MsoListParagraph, li.MsoListParagraph, div.MsoListParagraph
	{{margin-top:0cm;
	margin-right:0cm;
	margin-bottom:10.0pt;
	margin-left:36.0pt;
	line-height:115%;
	font-size:11.0pt;
	font-family:""Calibri"",""sans-serif"";}}
p.MsoListParagraphCxSpFirst, li.MsoListParagraphCxSpFirst, div.MsoListParagraphCxSpFirst
	{{margin-top:0cm;
	margin-right:0cm;
	margin-bottom:0cm;
	margin-left:36.0pt;
	margin-bottom:.0001pt;
	line-height:115%;
	font-size:11.0pt;
	font-family:""Calibri"",""sans-serif"";}}
p.MsoListParagraphCxSpMiddle, li.MsoListParagraphCxSpMiddle, div.MsoListParagraphCxSpMiddle
	{{margin-top:0cm;
	margin-right:0cm;
	margin-bottom:0cm;
	margin-left:36.0pt;
	margin-bottom:.0001pt;
	line-height:115%;
	font-size:11.0pt;
	font-family:""Calibri"",""sans-serif"";}}
p.MsoListParagraphCxSpLast, li.MsoListParagraphCxSpLast, div.MsoListParagraphCxSpLast
	{{margin-top:0cm;
	margin-right:0cm;
	margin-bottom:10.0pt;
	margin-left:36.0pt;
	line-height:115%;
	font-size:11.0pt;
	font-family:""Calibri"",""sans-serif"";}}
.MsoPapDefault
	{{margin-bottom:10.0pt;
	line-height:115%;}}
@page WordSection1
	{{size:612.0pt 792.0pt;
	margin:72.0pt 72.0pt 72.0pt 72.0pt;}}
div.WordSection1
	{{page:WordSection1;}}
 /* List Definitions */
 ol
	{{margin-bottom:0cm;}}
ul
	{{margin-bottom:0cm;}}
-->
</style>

</head>

<body lang=EN-US>

<div class=WordSection1>

<p class=MsoNormal><a name=""OLE_LINK2""></a><a name=""OLE_LINK1""><span lang=RU
style='font-family:""Times New Roman"",""serif""'>Уважаемый  Заказчик,</span></a></p>

<p class=MsoNormal style='text-indent:36.0pt'><span lang=RU style='font-family:
""Times New Roman"",""serif""'>Информируем Вас о выполнении лимитированых работах и
не контрактных работах по инцеденту/заявке № {1}</p>

<p class=MsoNormal><span lang=RU style='font-family:""Times New Roman"",""serif""'>Просьба
ознакомиться с приведенной таблицей:</span></p>

<table class=MsoTableGrid border=1 cellspacing=0 cellpadding=0
 style='border-collapse:collapse;border:none'>
 <tr>
  <td width=47 valign=top style='width:35.05pt;border:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt'>
  <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
  normal'><span lang=RU style='font-family:""Times New Roman"",""serif""'>№</span></p>
  </td>
  <td width=209 valign=top style='width:156.45pt;border:solid windowtext 1.0pt;
  border-left:none;padding:0cm 5.4pt 0cm 5.4pt'>
  <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
  normal'><span lang=RU style='font-family:""Times New Roman"",""serif""'>Описание
  позиции лимита</span></p>
  </td>
  <td width=96 valign=top style='width:71.65pt;border:solid windowtext 1.0pt;
  border-left:none;padding:0cm 5.4pt 0cm 5.4pt'>
  <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
  normal'><span lang=RU style='font-family:""Times New Roman"",""serif""'>Кол-во в
  заявке</span></p>
  </td>
  <td width=112 valign=top style='width:83.75pt;border:solid windowtext 1.0pt;
  border-left:none;padding:0cm 5.4pt 0cm 5.4pt'>
  <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
  normal'><span lang=RU style='font-family:""Times New Roman"",""serif""'>Фактически
  выполнено</span></p>
  </td>
  <td width=176 valign=top style='width:131.9pt;border:solid windowtext 1.0pt;
  border-left:none;padding:0cm 5.4pt 0cm 5.4pt'>
  <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
  normal'><span lang=RU style='font-family:""Times New Roman"",""serif""'>Установленый
  лимит</span></p>
  </td>
 </tr>
 {0}
</table>

<p class=MsoNormal><span lang=RU style='font-family:""Times New Roman"",""serif""'>&nbsp;</span></p>

<p class=MsoNormal><span lang=RU style='font-family:""Times New Roman"",""serif""'>В
соответствии с проведенными работами просьба согласовать объем работ:</span></p>

<p class=MsoListParagraphCxSpFirst style='text-indent:-18.0pt'><span lang=RU
style='font-family:Symbol'>·<span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span lang=RU style='font-family:""Times New Roman"",""serif""'>В
случае проведения работ в рамках лимита требуется Ваше подтверждение.</span></p>

<p class=MsoListParagraphCxSpMiddle style='text-indent:-18.0pt'><span lang=RU
style='font-family:Symbol'>·<span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span lang=RU style='font-family:""Times New Roman"",""serif""'>В
случае привышения лимита или не контратных в составе работ по заявке просьба
подписать и отправить скан документа прикрепленному к данному уведомлению.</span></p>

<p class=MsoListParagraphCxSpLast style='text-indent:-18.0pt'><span lang=RU
style='font-family:Symbol'>·<span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span lang=RU style='font-family:""Times New Roman"",""serif""'>В
случае проведения части работ в рамках лимита, а части за рамками/не котрактые
работы, просьба отправить подтверждение и подписаное приложение.</span></p>

<p class=MsoNormal><span lang=RU style='font-family:""Times New Roman"",""serif""'>По
факту выполнения количество услуг/работ может быть скорректировано. </span></p>

<p class=MsoNormal><span lang=RU style='font-family:""Times New Roman"",""serif""'>В
случае каких либо вопросов обращайтесь по следующему адресу:</span></p>

<p class=MsoNormal><span style='font-family:""Times New Roman"",""serif""'>Ekaterina
Kosinskaya &lt;ekaterina.kosinskaya@ericsson.com&gt;</span></p>

</div>

</body>

</html>

";

		private static string NoteRowTemplate = @"
<tr>
  <td width=47 valign=top style='width:35.05pt;border:solid windowtext 1.0pt;
  border-top:none;padding:0cm 5.4pt 0cm 5.4pt'>
  <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
  normal'><span lang=RU style='font-family:""Times New Roman"",""serif""'>{0}</span></p>
  </td>
  <td width=209 valign=top style='width:156.45pt;border-top:none;border-left:
  none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt'>
  <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
  normal'><span lang=RU style='font-family:""Times New Roman"",""serif""'>{1}</span></p>
  </td>
  <td width=96 valign=top style='width:71.65pt;border-top:none;border-left:
  none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt'>
  <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
  normal'><span lang=RU style='font-family:""Times New Roman"",""serif""'>{2}</span></p>
  </td>
  <td width=112 valign=top style='width:83.75pt;border-top:none;border-left:
  none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt'>
  <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
  normal'><span lang=RU style='font-family:""Times New Roman"",""serif""'>{3}</span></p>
  </td>
  <td width=176 valign=top style='width:131.9pt;border-top:none;border-left:
  none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt'>
  <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
  normal'><span lang=RU style='font-family:""Times New Roman"",""serif""'>{4}</span></p>
  </td>
 </tr>


";
		private static string RequestMainTemplate =
		   @"
<html>

<head>
<meta http-equiv=Content-Type content=""text/html; charset=windows-1251"">
<meta name=Generator content=""Microsoft Word 14 (filtered)"">
<style>
<!--
 /* Font Definitions */
 @font-face
	{{font-family:Calibri;
	panose-1:2 15 5 2 2 2 4 3 2 4;}}
 /* Style Definitions */
 p.MsoNormal, li.MsoNormal, div.MsoNormal
	{{margin-top:0cm;
	margin-right:0cm;
	margin-bottom:10.0pt;
	margin-left:0cm;
	line-height:115%;
	font-size:11.0pt;
	font-family:""Calibri"",""sans-serif"";}}
a:link, span.MsoHyperlink
	{{color:blue;
	text-decoration:underline;}}
a:visited, span.MsoHyperlinkFollowed
	{{color:purple;
	text-decoration:underline;}}
p.MsoListParagraph, li.MsoListParagraph, div.MsoListParagraph
	{{margin-top:0cm;
	margin-right:0cm;
	margin-bottom:10.0pt;
	margin-left:36.0pt;
	line-height:115%;
	font-size:11.0pt;
	font-family:""Calibri"",""sans-serif"";}}
p.MsoListParagraphCxSpFirst, li.MsoListParagraphCxSpFirst, div.MsoListParagraphCxSpFirst
	{{margin-top:0cm;
	margin-right:0cm;
	margin-bottom:0cm;
	margin-left:36.0pt;
	margin-bottom:.0001pt;
	line-height:115%;
	font-size:11.0pt;
	font-family:""Calibri"",""sans-serif"";}}
p.MsoListParagraphCxSpMiddle, li.MsoListParagraphCxSpMiddle, div.MsoListParagraphCxSpMiddle
	{{margin-top:0cm;
	margin-right:0cm;
	margin-bottom:0cm;
	margin-left:36.0pt;
	margin-bottom:.0001pt;
	line-height:115%;
	font-size:11.0pt;
	font-family:""Calibri"",""sans-serif"";}}
p.MsoListParagraphCxSpLast, li.MsoListParagraphCxSpLast, div.MsoListParagraphCxSpLast
	{{margin-top:0cm;
	margin-right:0cm;
	margin-bottom:10.0pt;
	margin-left:36.0pt;
	line-height:115%;
	font-size:11.0pt;
	font-family:""Calibri"",""sans-serif"";}}
.MsoPapDefault
	{{margin-bottom:10.0pt;
	line-height:115%;}}
@page WordSection1
	{{size:612.0pt 792.0pt;
	margin:72.0pt 72.0pt 72.0pt 72.0pt;}}
div.WordSection1
	{{page:WordSection1;}}
 /* List Definitions */
 ol
	{{margin-bottom:0cm;}}
ul
	{{margin-bottom:0cm;}}
-->
</style>

</head>

<body lang=EN-US link=blue vlink=purple>

<div class=WordSection1>

<p class=MsoNormal><span lang=RU>Уважаемый Заказчик,</span></p>

<p class=MsoNormal><span lang=RU>&nbsp;</span></p>

<p class=MsoNormal><span lang=RU>Информируем Вас о лимитированых работах и не
контрактных работах по инцеденту/заявке № {1}.</span></p>

<p class=MsoNormal><span lang=RU>Просьба ознакомиться с приведенной таблицей:</span></p>

<table class=MsoTableGrid border=1 cellspacing=0 cellpadding=0
 style='border-collapse:collapse;border:none'>
 <tr>
  <td width=128 valign=top style='width:95.75pt;border:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt'>
  <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
  normal'><span lang=RU style='font-family:""Times New Roman"",""serif""'>№</span></p>
  </td>
  <td width=128 valign=top style='border:solid windowtext 1.0pt;
  border-left:none;padding:0cm 5.4pt 0cm 5.4pt'>
  <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
  normal'><span lang=RU style='font-family:""Times New Roman"",""serif""'>Описание
  позиции лимита</span></p>
  </td>
  <td width=128 valign=top style='width:95.75pt;border:solid windowtext 1.0pt;
  border-left:none;padding:0cm 5.4pt 0cm 5.4pt'>
  <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
  normal'><span lang=RU style='font-family:""Times New Roman"",""serif""'>Кол-во в
  заявке</span></p>
  </td>
  <td width=128 valign=top style='width:95.75pt;border:solid windowtext 1.0pt;
  border-left:none;padding:0cm 5.4pt 0cm 5.4pt'>
  <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
  normal'><span lang=RU style='font-family:""Times New Roman"",""serif""'>Фактически
  выполнено</span></p>
  </td>
  <td width=128 valign=top style='width:95.8pt;border:solid windowtext 1.0pt;
  border-left:none;padding:0cm 5.4pt 0cm 5.4pt'>
  <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
  normal'><span lang=RU style='font-family:""Times New Roman"",""serif""'>Установленый
  лимит</span></p>
  </td>
 </tr>
 {0}
</table>

<p class=MsoNormal><span lang=RU>&nbsp;</span></p>

<p class=MsoNormal><span lang=RU>Для начала проведения работ требуется одно из
перечисланных действий:</span></p>

<p class=MsoListParagraphCxSpFirst style='text-indent:-18.0pt'><span lang=RU
style='font-family:Symbol'>·<span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span lang=RU>В случае проведения работ в рамках лимита требуется
Ваше подтверждение.</span></p>

<p class=MsoListParagraphCxSpMiddle style='text-indent:-18.0pt'><span lang=RU
style='font-family:Symbol'>·<span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span lang=RU>В случае привышения лимита или не контратных в
составе работ по заявке просьба подписать и отправить скан документа
прикрепленному к данному уведомлению.</span></p>

<p class=MsoListParagraphCxSpLast style='text-indent:-18.0pt'><span lang=RU
style='font-family:Symbol'>·<span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span lang=RU>В случае проведения части работ в рамках лимита, а
части за рамками/не котрактые работы, просьба отправить подтверждение и
подписаное приложение.</span></p>

<p class=MsoNormal><span lang=RU>По факту выполнения количество услуг/работ
может быть скорректировано. </span></p>

<p class=MsoNormal><span lang=RU>В случае каких либо вопросов обращайтесь по
следующему адресу:</span></p>

<p class=MsoNormal><span lang=RU>Ekaterina Kosinskaya</span><span
style='font-family:""Times New Roman"",""serif""'> &lt;</span><span lang=RU
style='font-family:""Times New Roman"",""serif""'><a
href=""mailto:ekaterina.kosinskaya@ericsson.com""><span lang=EN-US>ekaterina.kosinskaya@ericsson.com</span></a></span><span
style='font-family:""Times New Roman"",""serif""'>&gt;</span></p>

<p class=MsoNormal>&nbsp;</p>

</div>

</body>

</html>

";
		private static string RequestRowTemplate = @"
<tr>
  <td width=128 valign=top style='width:95.75pt;border:solid windowtext 1.0pt;
  border-top:none;padding:0cm 5.4pt 0cm 5.4pt'>
  <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
  normal'><span lang=RU>{0}</span></p>
  </td>
  <td width=128 valign=top style='border-top:none;border-left:
  none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt'>
  <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
  normal'><span lang=RU>{1}</span></p>
  </td>
  <td width=128 valign=top style='width:95.75pt;border-top:none;border-left:
  none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt'>
  <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
  normal'><span lang=RU>{2}</span></p>
  </td>
  <td width=128 valign=top style='width:95.75pt;border-top:none;border-left:
  none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt'>
  <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
  normal'><span lang=RU>{3}</span></p>
  </td>
  <td width=128 valign=top style='width:95.8pt;border-top:none;border-left:
  none;border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt'>
  <p class=MsoNormal style='margin-bottom:0cm;margin-bottom:.0001pt;line-height:
  normal'><span lang=RU>{4}</span></p>
  </td>
 </tr>
";

		#endregion


		public static ShVCRequestImport Handle(string vcRequestName, Context context)
		{
			string votingOptions = string.Format("{0};{1}", Common.AVRCommon.AcceptMask, Common.AVRCommon.RejectMask);
			string notifyRecipients =
				//    "aleksey.chekalin@ericsson.com";
				//"aleksey.gorin@ericsson.com";
			"ekaterina.kosinskaya@ericsson.com";



            var vcRequestToUpload = context.VCRequestsToCreate.FirstOrDefault(r => r.VCRequestNumber == vcRequestName);
            if (vcRequestToUpload == null)
                return null;

			RedemptionMailProcessor interactor = new RedemptionMailProcessor("SOLARIS");

			var shAvr = context.ShAVRs.Find(vcRequestToUpload.AVRId);

			System.Diagnostics.Debug.WriteLine(shAvr.AVRId);

			var shAvrItems = shAvr.Items.ToList(); // TaskParameters.Context.ShAVRItems.Where(a => a.AVRSId == shAvr.AVRId).ToList();
			var inLimitItems = shAvrItems.Where(AVRItemRepository.InLimitComp).ToList();
            var orderItems = context.SatMusItems.Where(m => m.VCRequestNumber == vcRequestName).ToList();
            var mappedOrderItems = orderItems.Where(i => i.AvrItemId.HasValue);
            var orderItemsIds = mappedOrderItems.Select(s => s.AvrItemId).ToList();
            /// эти позиции присутствуют и в заказе и в письме. Катя их переопрайсовала и вставила свои количества.
            /// а могла и удалить. поэтому надо сравнить их с позициями мус. и пересекающиеся добавлять
			var outOfLimitItems = shAvrItems.Where(AVRItemRepository.OutOfLimitComp).Where(i=> orderItemsIds.Contains(i.AVRItemId)).ToList();



			//var addOnSalesItems = shAvrItems.Where(AVRItemRepository.IsVCAddonSalesComp).ToList();
			//var orderItems = outOfLimitItems.Union(addOnSalesItems).Distinct().ToList();

			if (inLimitItems.Any(i => !i.InLimit.HasValue))
			{
				Console.WriteLine(string.Format("Еще не все лимиты позиции на АВР {0} учтены при расчете лимитов.", shAvr.AVRId));
				// TaskParameters.TaskLogger.LogError(string.Format("Еще не все лимиты позиции на АВР {0} учтены при расчете лимитов.", shAvr.AVRId));
				// continue;
				return null;
			}

			var shVCRequest = new ShVCRequestImport() { Id = vcRequestName, ShAVRs = shAvr.AVRId };

			byte[] orderBytes = null;
			if (orderItems.Any())
			{
				//if (!shAvr.PrePriced.HasValue)
				//{
				//    Console.WriteLine(string.Format("Предопрайсовка АВР {0} еще не проведена.", shAvr.AVRId));
				//    // TaskParameters.TaskLogger.LogError(string.Format("Предопрайсовка АВР {0} еще не проведена.", shAvr.AVRId));
				//    // continue;
				//    return null;
				//}
				var count = 0;
				orderBytes = ExcelParser.EpplusInteract.CreateAVROrder.CreateOrderFile(orderItems, vcRequestName);
				if (orderBytes == null)
				{
					Console.WriteLine(string.Format("Ошибка формирования заказа для авр: {0}", shAvr.AVRId));
					// TaskParameters.TaskLogger.LogError(string.Format("Ошибка формирования заказа для авр: {0}", shAvr.AVRId));
					//  continue;
					return null;
				}

			}

			string rowtemplate = shAvr.Priority > 2 ? RequestRowTemplate : NoteRowTemplate;
			string mailtemplate = shAvr.Priority > 2 ? RequestMainTemplate : NotifyMainTemplate;

			var mailText = string.Empty;
			// заявленные на добавление в импорт позиций
			StringBuilder textBuilder = new StringBuilder();
			int lCount = 0;
			if (inLimitItems.Any())
			{

				foreach (var ilitem in inLimitItems.GroupBy(i=>new { Description =  i.Description, Limit = i.Limit  }).ToList())
				{
					lCount++;

					textBuilder.AppendLine(string.Format(rowtemplate
							, lCount
							, ilitem.Key.Limit != null ? ilitem.Key.Limit.Description : (string.IsNullOrEmpty(ilitem.Key.Description) ? "" : ilitem.Key.Description)
							, ilitem.Sum(i=>i.Quantity)
						//, ilitem.Price.HasValue ? ilitem.Price.Value : 0
							, ilitem.Key.Limit.Executed.HasValue ? ilitem.Key.Limit.Executed.Value : 0
							, ilitem.Key.Limit.SettedLimit.HasValue ? ilitem.Key.Limit.SettedLimit.Value : 0
							));
				}

			}
			if (outOfLimitItems.Any())
			{

                var olItems = outOfLimitItems.Join(
                    mappedOrderItems,
                    o => o.AVRItemId,
                    m => m.AvrItemId,
                    (o, m) => new { o, m }
                    );


                foreach (var olitem in olItems.GroupBy(i => new { Description = i.o.Description, Limit = i.o.Limit }).ToList())
				{

                    lCount++;
					if (olitem.Key.Limit != null)
					{
						textBuilder.AppendLine(string.Format(rowtemplate
						   , lCount
						   , olitem.Key.Limit != null ? olitem.Key.Limit.Description : (string.IsNullOrEmpty(olitem.Key.Description) ? "" : olitem.Key.Description)
						   , olitem.Sum(i=>i.m.Quantity)
							//, olitem.VCPrice.HasValue ? olitem.VCPrice.Value : 0
						   , olitem.Key.Limit.Executed.HasValue ? olitem.Key.Limit.Executed.Value : 0
						   , olitem.Key.Limit.SettedLimit.HasValue ? olitem.Key.Limit.SettedLimit.Value : 0
						   ));
					}
				}

			}


			mailText = string.Format(mailtemplate, textBuilder.ToString(), shAvr.AVRId);

			var autoMail = new AutoMail();
			autoMail.Email = notifyRecipients;
			autoMail.Body = mailText;
			autoMail.Subject = string.Format("{0}", vcRequestName);
			if (inLimitItems.Any())
			{
				autoMail.VotingOptions = votingOptions;
			}


			if (orderBytes != null)
			{
				string orderFilePath = Common.AVRCommon.SaveOrderFile(vcRequestName, orderBytes);
				if (File.Exists(orderFilePath))
				{
					var attachment = new Attachment() { FilePath = orderFilePath };
					autoMail.Attachments.Add(attachment);
				}
				else
				{
					Console.WriteLine(string.Format("Ошибка сохранения заказа для авр: {0} - {1}", shAvr.AVRId, orderFilePath));
					// TaskParameters.TaskLogger.LogError(string.Format("Ошибка сохранения заказа для авр: {0} - {1}", shAvr.AVRId, orderFilePath));
					// continue;
					return null;
				}
			}

			if (orderBytes != null)
				shVCRequest.HasOrder = true;



			if (inLimitItems.Any()&& shAvr.Priority>2)
				shVCRequest.HasRequest = true;


			// сохраняем письмо
			var mailPath = interactor.SaveMailToFile(autoMail, Common.AVRCommon.GetAVRArhivePath(vcRequestName));
			shVCRequest.Attachment = Path.GetDirectoryName(mailPath);



			return shVCRequest;
		}
	}
}
