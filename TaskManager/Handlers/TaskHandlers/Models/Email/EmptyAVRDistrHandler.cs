using MailProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.Email
{
    public class EmptyAVRDistrHandler : ATaskHandler
    {
        public EmptyAVRDistrHandler(TaskParameters taskParams) : base(taskParams)
        {

        }

        #region MailTemplate
        string mailTemplate = @"<html xmlns:v='urn:schemas-microsoft-com:vml'
xmlns:o='urn:schemas-microsoft-com:office:office'
xmlns:w='urn:schemas-microsoft-com:office:word'
xmlns:x='urn:schemas-microsoft-com:office:excel'
xmlns:m='http://schemas.microsoft.com/office/2004/12/omml'
xmlns='http://www.w3.org/TR/REC-html40'>

<head>
<meta http-equiv=Content-Type content = 'text/html; charset=windows-1251'>
  
 <style>
<!--
 /* Font Definitions */
 @font-face
	{{font-family:Calibri;
	panose-1:2 15 5 2 2 2 4 3 2 4;
	mso-font-alt:'Century Gothic';
	mso-font-charset:204;
	mso-generic-font-family:swiss;
	mso-font-pitch:variable;
	mso-font-signature:-536870145 1073786111 1 0 415 0;}}
    @font-face
	{{font-family:Tahoma;
	panose-1:2 11 6 4 3 5 4 4 2 4;
	mso-font-alt:Helvetica;
	mso-font-charset:204;
	mso-generic-font-family:swiss;
	mso-font-pitch:variable;
	mso-font-signature:-520081665 -1073717157 41 0 66047 0;}}
/* Style Definitions */
p.MsoNormal, li.MsoNormal, div.MsoNormal
	{{mso-style-unhide:no;
	mso-style-qformat:yes;
	mso-style-parent:'';
	margin:0cm;
	margin-bottom:.0001pt;
	mso-pagination:widow-orphan;
	font-size:11.0pt;
	font-family:'Calibri','sans-serif';
	mso-ascii-font-family:Calibri;
	mso-ascii-theme-font:minor-latin;
	mso-fareast-font-family:Calibri;
	mso-fareast-theme-font:minor-latin;
	mso-hansi-font-family:Calibri;
	mso-hansi-theme-font:minor-latin;
	mso-bidi-font-family:'Times New Roman';
	mso-bidi-theme-font:minor-bidi;}}
a:link, span.MsoHyperlink
	{{mso-style-noshow:yes;
	mso-style-priority:99;
	color:blue;
	mso-themecolor:hyperlink;
	text-decoration:underline;
	text-underline:single;}}
a:visited, span.MsoHyperlinkFollowed
	{{mso-style-noshow:yes;
	mso-style-priority:99;
	color:purple;
	mso-themecolor:followedhyperlink;
	text-decoration:underline;
	text-underline:single;}}
p.MsoAcetate, li.MsoAcetate, div.MsoAcetate
	{{mso-style-noshow:yes;
	mso-style-priority:99;
	mso-style-link:'Balloon Text Char';
	margin:0cm;
	margin-bottom:.0001pt;
	mso-pagination:widow-orphan;
	font-size:8.0pt;
	font-family:'Tahoma','sans-serif';
	mso-fareast-font-family:Calibri;
	mso-fareast-theme-font:minor-latin;}}
span.EmailStyle17
	{{mso-style-type:personal-compose;
	mso-style-noshow:yes;
	mso-style-unhide:no;
	mso-ansi-font-size:11.0pt;
	mso-bidi-font-size:11.0pt;
	font-family:'Calibri','sans-serif';
	mso-ascii-font-family:Calibri;
	mso-ascii-theme-font:minor-latin;
	mso-fareast-font-family:Calibri;
	mso-fareast-theme-font:minor-latin;
	mso-hansi-font-family:Calibri;
	mso-hansi-theme-font:minor-latin;
	mso-bidi-font-family:'Times New Roman';
	mso-bidi-theme-font:minor-bidi;
	color:windowtext;}}
span.BalloonTextChar
	{{mso-style-name:'Balloon Text Char';
	mso-style-noshow:yes;
	mso-style-priority:99;
	mso-style-unhide:no;
	mso-style-locked:yes;
	mso-style-link:'Balloon Text';
	mso-ansi-font-size:8.0pt;
	mso-bidi-font-size:8.0pt;
	font-family:'Tahoma','sans-serif';
	mso-ascii-font-family:Tahoma;
	mso-hansi-font-family:Tahoma;
	mso-bidi-font-family:Tahoma;}}
.MsoChpDefault
	{{mso-style-type:export-only;
	mso-default-props:yes;
	font-family:'Calibri','sans-serif';
	mso-ascii-font-family:Calibri;
	mso-ascii-theme-font:minor-latin;
	mso-fareast-font-family:Calibri;
	mso-fareast-theme-font:minor-latin;
	mso-hansi-font-family:Calibri;
	mso-hansi-theme-font:minor-latin;
	mso-bidi-font-family:'Times New Roman';
	mso-bidi-theme-font:minor-bidi;}}
@page WordSection1
{{
    size:612.0pt 792.0pt;
    margin:2.0cm 42.5pt 2.0cm 3.0cm;
    mso-header-margin:36.0pt;
    mso-footer-margin:36.0pt;
    mso-paper-source:0;
}}
div.WordSection1
	{{page:WordSection1;}}
-->
</style>
<!--[if gte mso 10]>
<style>
 /* Style Definitions */
 table.MsoNormalTable
	{{mso-style-name:'Table Normal';
	mso-tstyle-rowband-size:0;
	mso-tstyle-colband-size:0;
	mso-style-noshow:yes;
	mso-style-priority:99;
	mso-style-parent:'';
	mso-padding-alt:0cm 5.4pt 0cm 5.4pt;
	mso-para-margin:0cm;
	mso-para-margin-bottom:.0001pt;
	mso-pagination:widow-orphan;
	font-size:11.0pt;
	font-family:'Calibri','sans-serif';
	mso-ascii-font-family:Calibri;
	mso-ascii-theme-font:minor-latin;
	mso-hansi-font-family:Calibri;
	mso-hansi-theme-font:minor-latin;}}
</style>
<![endif]--><!--[if gte mso 9]><xml>
 <o:shapedefaults v:ext='edit' spidmax='1026'/>
</xml><![endif]--><!--[if gte mso 9]><xml>
 <o:shapelayout v:ext='edit'>
  <o:idmap v:ext='edit' data='1'/>
 </o:shapelayout></xml><![endif]-->
</head>

<body lang = EN - US link=blue vlink = purple style='tab-interval:36.0pt'>

<div class=WordSection1>

<p class=MsoNormal style = 'mso-layout-grid-align:none;text-autospace:none'><span
style='mso-ascii-font-family:Calibri;mso-hansi-font-family:Calibri;mso-bidi-font-family:
Calibri;color:black'><o:p>&nbsp;</o:p></span></p>
На данный момент в SH заведены следующие заявки без состава работ:
<p class=MsoNormal><o:p>&nbsp;</o:p></p>

<table class=MsoNormalTable border = 0 cellspacing=0 cellpadding=0 width=951
 style='width:713.0pt;margin-left:-.75pt;border-collapse:collapse;mso-yfti-tbllook:
 1184;mso-padding-alt:0cm 5.4pt 0cm 5.4pt'>
 <tr style = 'mso-yfti-irow:0;mso-yfti-firstrow:yes;height:15.0pt'>
  <td width=95 nowrap valign = bottom style='width:71.0pt;border-top:none;
  border-left:none;border-bottom:solid white 1.5pt;border-right:solid white 1.0pt;
  mso-border-bottom-alt:solid white 1.5pt;mso-border-right-alt:solid white .5pt;
  background:#4F81BD;padding:0cm 5.4pt 0cm 5.4pt;height:15.0pt'>
  <p class=MsoNormal><b><span style = 'mso-ascii-font-family:Calibri;mso-fareast-font-family:
  'Times New Roman';mso-hansi-font-family:Calibri;mso-bidi-font-family:'Times New Roman';
  color:white'>№ Заявки<o:p></o:p></span></b></p>
  </td>
  <td width = 79 nowrap valign = bottom style='width:59.0pt;border-top:none;
  border-left:none;border-bottom:solid white 1.5pt;border-right:solid white 1.0pt;
  mso-border-left-alt:solid white .5pt;mso-border-left-alt:solid white .5pt;
  mso-border-bottom-alt:solid white 1.5pt;mso-border-right-alt:solid white .5pt;
  background:#4F81BD;padding:0cm 5.4pt 0cm 5.4pt;height:15.0pt'>
  <p class=MsoNormal><b><span style = 'mso-ascii-font-family:Calibri;mso-fareast-font-family:
  'Times New Roman';mso-hansi-font-family:Calibri;mso-bidi-font-family:'Times New Roman';
  color:white'>Филиал<o:p></o:p></span></b></p>
  </td>
  <td width = 95 nowrap valign = bottom style='width:71.0pt;border-top:none;
  border-left:none;border-bottom:solid white 1.5pt;border-right:solid white 1.0pt;
  mso-border-left-alt:solid white .5pt;mso-border-left-alt:solid white .5pt;
  mso-border-bottom-alt:solid white 1.5pt;mso-border-right-alt:solid white .5pt;
  background:#4F81BD;padding:0cm 5.4pt 0cm 5.4pt;height:15.0pt'>
  <p class=MsoNormal><b><span style = 'mso-ascii-font-family:Calibri;mso-fareast-font-family:
  'Times New Roman';mso-hansi-font-family:Calibri;mso-bidi-font-family:'Times New Roman';
  color:white'>Подрядчик<o:p></o:p></span></b></p>
  </td>
  <td width = 83 nowrap valign = bottom style='width:62.0pt;border-top:none;
  border-left:none;border-bottom:solid white 1.5pt;border-right:solid white 1.0pt;
  mso-border-left-alt:solid white .5pt;mso-border-left-alt:solid white .5pt;
  mso-border-bottom-alt:solid white 1.5pt;mso-border-right-alt:solid white .5pt;
  background:#4F81BD;padding:0cm 5.4pt 0cm 5.4pt;height:15.0pt'>
  <p class=MsoNormal><b><span style = 'mso-ascii-font-family:Calibri;mso-fareast-font-family:
  'Times New Roman';mso-hansi-font-family:Calibri;mso-bidi-font-family:'Times New Roman';
  color:white'>Тип АВР<o:p></o:p></span></b></p>
  </td>
  <td width = 131 nowrap valign = bottom style='width:98.0pt;border-top:none;
  border-left:none;border-bottom:solid white 1.5pt;border-right:solid white 1.0pt;
  mso-border-left-alt:solid white .5pt;mso-border-left-alt:solid white .5pt;
  mso-border-bottom-alt:solid white 1.5pt;mso-border-right-alt:solid white .5pt;
  background:#4F81BD;padding:0cm 5.4pt 0cm 5.4pt;height:15.0pt'>
  <p class=MsoNormal><b><span style = 'mso-ascii-font-family:Calibri;mso-fareast-font-family:
  'Times New Roman';mso-hansi-font-family:Calibri;mso-bidi-font-family:'Times New Roman';
  color:white'>Стоимость работ<o:p></o:p></span></b></p>
  </td>
  <td width = 223 nowrap valign = bottom style='width:167.0pt;border-top:none;
  border-left:none;border-bottom:solid white 1.5pt;border-right:solid white 1.0pt;
  mso-border-left-alt:solid white .5pt;mso-border-left-alt:solid white .5pt;
  mso-border-bottom-alt:solid white 1.5pt;mso-border-right-alt:solid white .5pt;
  background:#4F81BD;padding:0cm 5.4pt 0cm 5.4pt;height:15.0pt'>
  <p class=MsoNormal><b><span style = 'mso-ascii-font-family:Calibri;mso-fareast-font-family:
  'Times New Roman';mso-hansi-font-family:Calibri;mso-bidi-font-family:'Times New Roman';
  color:white'>Дата начала выполнения работ<o:p></o:p></span></b></p>
  </td>
  <td width = 247 nowrap valign = bottom style='width:185.0pt;border:none;
  border-bottom:solid white 1.5pt;mso-border-left-alt:solid white .5pt;
  background:#4F81BD;padding:0cm 5.4pt 0cm 5.4pt;height:15.0pt'>
  <p class=MsoNormal><b><span style = 'mso-ascii-font-family:Calibri;mso-fareast-font-family:
  'Times New Roman';mso-hansi-font-family:Calibri;mso-bidi-font-family:'Times New Roman';
  color:white'>Дата окончания выполенния работ<o:p></o:p></span></b></p>
  </td>
 </tr>
{0}
</table>

<p class=MsoNormal><o:p>&nbsp;</o:p></p>

<p class=MsoNormal><o:p>&nbsp;</o:p></p>

<p class=MsoNormal><o:p>&nbsp;</o:p></p>

</div>

</body>

</html>";

    string rowTemplate = @" 
  <td width = 79 nowrap valign = bottom style='vertical-align: top;width:59.0pt;border:none;border-right:
  solid white 1.0pt;mso-border-top-alt:solid white .5pt;mso-border-left-alt:
  solid white .5pt;mso-border-top-alt:solid white .5pt;mso-border-left-alt:
  solid white .5pt;mso-border-right-alt:solid white .5pt;background:#DCE6F1;
  padding:0cm 5.4pt 0cm 5.4pt;height:15.0pt'>{0}</td>
  <td width = 79 nowrap valign = bottom style='vertical-align: top;width:59.0pt;border:none;border-right:
  solid white 1.0pt;mso-border-top-alt:solid white .5pt;mso-border-left-alt:
  solid white .5pt;mso-border-top-alt:solid white .5pt;mso-border-left-alt:
  solid white .5pt;mso-border-right-alt:solid white .5pt;background:#DCE6F1;
  padding:0cm 5.4pt 0cm 5.4pt;height:15.0pt'>{1}</td>
  <td width = 200 nowrap valign = bottom style='vertical-align: top;width:71.0pt;border:none;border-right:
  solid white 1.0pt;mso-border-top-alt:solid white .5pt;mso-border-left-alt:
  solid white .5pt;mso-border-top-alt:solid white .5pt;mso-border-left-alt:
  solid white .5pt;mso-border-right-alt:solid white .5pt;background:#DCE6F1;
  padding:0cm 5.4pt 0cm 5.4pt;height:15.0pt'>{2}</td>
  <td width = 282 nowrap valign = bottom style='vertical-align: top;width:62.0pt;border:none;border-right:
  solid white 1.0pt;mso-border-top-alt:solid white .5pt;mso-border-left-alt:
  solid white .5pt;mso-border-top-alt:solid white .5pt;mso-border-left-alt:
  solid white .5pt;mso-border-right-alt:solid white .5pt;background:#DCE6F1;
  padding:0cm 5.4pt 0cm 5.4pt;height:15.0pt'>{3}</td>
  <td width = 90 nowrap valign = bottom style='vertical-align: top;width:98.0pt;border:none;
  border-right:solid white 1.0pt;mso-border-top-alt:solid white .5pt;
  mso-border-left-alt:solid white .5pt;mso-border-top-alt:solid white .5pt;
  mso-border-left-alt:solid white .5pt;mso-border-right-alt:solid white .5pt;
  background:#DCE6F1;padding:0cm 5.4pt 0cm 5.4pt;height:15.0pt'>{4}</td>
  <td width = 90 nowrap valign = bottom style='vertical-align: top;width:67.0pt;border:none;
  border-right:solid white 1.0pt;mso-border-top-alt:solid white .5pt;
  mso-border-left-alt:solid white .5pt;mso-border-top-alt:solid white .5pt;
  mso-border-left-alt:solid white .5pt;mso-border-right-alt:solid white .5pt;
  background:#DCE6F1;padding:0cm 5.4pt 0cm 5.4pt;height:15.0pt'>{5}</td>
  <td width = 90 nowrap valign = bottom style='vertical-align: top;width:85.0pt;border:none;
  mso-border-top-alt:solid white .5pt;mso-border-left-alt:solid white .5pt;
  background:#DCE6F1;padding:0cm 5.4pt 0cm 5.4pt;height:15.0pt'>{6}</td>
 </tr>";

        #endregion


        /// <summary>
        /// выбираем заявки без позиций, узнаем почту их создателя и запиливаем их ему
        /// по стурктуре филиала узнаем руководителя филиала, узнаем его почту и запиливаем все заявки
        /// без позиций ему
        /// </summary>
        /// <returns></returns>
        public override bool Handle()
        {
            bool addtest = true;
            var testRecipints = new List<string> { DistributionConstants.EalgoriEmail, DistributionConstants.EgorovEmail};

            var startDate = new DateTime(2016, 6, 22);
            var emptyAVRs = TaskParameters.Context.ShAVRs
                .Where(c => c.ObjectCreateDate > startDate)
                .Where(c => c.Items.Count == 0)
                .GroupBy(g => g.Subregion);
            ;
            RedemptionMailProcessor interactor = new RedemptionMailProcessor("SOLARIS");
            var filials = TaskParameters.Context.ShFilialStruct.ToList();
            foreach (var filial in filials)
            {
                var avrs = emptyAVRs.Where(a => a.Key == filial.Name).ToList();
                if (avrs.Count > 0)
                {
                    var avrsGrByCreated = avrs.SelectMany(s => s).GroupBy(g => g.CreatedByEmail);
                    foreach (var avrGr in avrsGrByCreated)
                    {
                        var builder = new StringBuilder();
                        foreach (var avr in avrGr)
                        {
                            builder.AppendLine(string.Format(rowTemplate
                                ,avr.AVRId
                                , avr.Subregion
                                ,avr.Subcontractor
                                ,avr.AVRType
                                ,avr.TotalAmount
                                ,avr.WorkStart.HasValue? avr.WorkStart.Value.ToShortDateString():""
                                , avr.WorkEnd.HasValue ? avr.WorkEnd.Value.ToShortDateString():""));
                        }
                        var message = string.Format(mailTemplate,builder.ToString());

                        //string body = $"На данный момент в SH заведены следующие заявки без состава работ: {string.Join(";", avrGr.Select(s => s.AVRId))}";
                        TaskParameters.EmailHandlerParams.Add(new List<string> { avrGr.Key }, addtest ? testRecipints : null, "Заявки без состава работ", true, message, null);
                    }


                    // сводна рук филиала
                    var rukFils = filial.RukFills.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    // здесь будем осбирать валидный список рукфилов
                    List<string> rukFilsList = new List<string>();
                    if (rukFils.Count > 0)
                    {
                        foreach (var ruk in rukFils)
                        {
                            if (CommonFunctions.StaticHelpers.IsValidEmail(ruk))
                            {
                                rukFilsList.Add(ruk);
                            }
                            else
                            {
                                var email = interactor.GetUserEmail(ruk);
                                if (!string.IsNullOrEmpty(email) && CommonFunctions.StaticHelpers.IsValidEmail(email))
                                {
                                    rukFilsList.Add(email);
                                }
                            }
                        }
                    }
                    if (rukFilsList.Count > 0)
                    {
                        var builder = new StringBuilder();
                        foreach (var avr in avrs.SelectMany(s => s))
                        {
                            builder.AppendLine(string.Format(rowTemplate
                                , avr.AVRId
                                , avr.Subregion
                                , avr.Subcontractor
                                , avr.AVRType
                                , avr.TotalAmount
                                , avr.WorkStart.HasValue ? avr.WorkStart.Value.ToShortDateString() : ""
                                , avr.WorkEnd.HasValue ? avr.WorkEnd.Value.ToShortDateString() : ""));
                        }
                        var message = string.Format(mailTemplate, builder.ToString());

                        //string body = $"На данный момент в SH заведены следующие заявки без состава работ: {string.Join(";", avrs.SelectMany(s => s).Select(s => s.AVRId))}";

                        TaskParameters.EmailHandlerParams.Add(rukFilsList, addtest?testRecipints:null, $"Заявки без состава работ ({filial.Name})", true, message, null);
                    }

                }









            }
            return true;
        }
    }
}
