$(document).ready(function () {

    kendo.culture("ru-RU");
    function startChange() {
        var startDate = start.value(),
                        endDate = end.value();

        if (startDate) {
            startDate = new Date(startDate);
            startDate.setDate(startDate.getDate());
            end.min(startDate);
        } else if (endDate) {
            start.max(new Date(endDate));
        } else {
            endDate = new Date();
            start.max(endDate);
            end.min(endDate);
        }
    }

    function endChange() {
        var endDate = end.value(),
                        startDate = start.value();

        if (endDate) {
            endDate = new Date(endDate);
            endDate.setDate(endDate.getDate());
            start.max(endDate);
        } else if (startDate) {
            end.min(new Date(startDate));
        } else {
            endDate = new Date();
            start.max(endDate);
            end.min(endDate);
        }
    }

    var start = $("#start").kendoDatePicker({
        change: startChange
    }).data("kendoDatePicker");

    var end = $("#end").kendoDatePicker({
        change: endChange
    }).data("kendoDatePicker");

    start.max(end.value());
    end.min(start.value());
    
    
    $("#subcontractor").width(465).kendoComboBox({
        dataTextField: "Name",
        dataValueField: "Id",
        placeholder: "Подрядчик для анализа...",
        filter: "contains",
       // value:subc,
        suggest: true,
        change: subcontractorChange,
        dataSource: {
            transport: {
                read: {
                    url: subcListUrl + '?' + Math.random(),
                    dataType: "json"
                }
            }
        }
    });

//    $("#sourceSubc").width(465).kendoComboBox({
//        dataTextField: "Name",
//        dataValueField: "Id",
//        placeholder: "Опорный прайслист от подрядчика...",
//        filter: "contains",
//      //  value:source,
//        suggest: true,
//        change: sourceSubcChange,
//        dataSource: {
//            transport: {
//                read: {
//                    url: subcListUrl + '?' + Math.random(),
//                    dataType: "json"
//                }
//            }
//        }
//    });

   // alert(subc+" " + source);
})

//$("#subcontractor").change(function () {
//        var n = $(this).val();
//      //  $(secondSubcPLsSelector).empty();
//       
// });


 function DrawPriceListRow(listSelector, id, pl) {
     console.log(pl);
     var list = $(listSelector).append(
     //'<tr bgcolor="' + (pl.Comparable ? "#C1C9F5" : "#FFFFFF") + '">' +
              '<tr  style="background: ' + (pl.Comparable ? "rgb(187, 187, 187)" : "#FFFFFF") + '">' +
             //'<td> <input type="radio" name="' + id + '" value=' + pl.Id + '></td>' +
             '<td>' + pl.Name + '</td>' +
             '<td>' + pl.SignDate + '</td>' +
             (pl.Comparable ?
            '<td style=" padding-top: 1px; ">'+
            (pl.Approved ? 'Утвержден' : 'Не утв')  

     //'<td style=" padding-top: 1px; ">' + (pl.Approved = 'true' ?

     //'<button type="button" class="btn btn-default btn-small" value="' + pl.RevId + '" onclick="cbApprove(' + pl.RevId + ');">Утв</button>' :
     //'<button type="button" class="btn btn-primary btn-small" onclick="cbClick(' + pl.RevId + ');">Отм</button>'
     // ) 
     //      + '<button type="button" class="btn btn-danger btn-small" onclick="deletePL(' + +pl.RevId + ')">X</button>'
     //'<td><input type="checkbox" onclick="cbClick(' + pl.RevId + ');" value=' + '"' + +pl.RevId + '"' + (pl.Approved = 'true' ? 'checked' : '') + '>'
             : '<td style=" padding-top: 5px; ">Не требует утв.')
     // 

             + '<a href="' + downloadUrl + pl.FileId + '" class="icon-download-alt pull-right "></a></td>' +
              +'</tr>');
     // '<tr></tr>');

 }

 function setOpacity(selector, opacity) {
     $(selector).css('opacity',opacity);
 }

function subcontractorChange(e)
{
    var selector = "#prices";
    var fadeSelector = "table";
    $(selector).empty();
    var n = this.value();
    setOpacity(fadeSelector, 0.2);
    $.getJSON(getPriceListsUrl + '?' + Math.random(), { subcId: n })
       .done(function (json) {

           for (var i = 0; i < json.length; i++) {
               DrawPriceListRow(selector, "spl", json[i]);
           };


       })
 .fail(function (jqxhr, textStatus, error) {
     var err = textStatus + ", " + error;
     console.log("Request Failed: " + err);
 })
 .always(function() {
     setOpacity(fadeSelector, 1);
  });
}
//function sourceSubcChange(e)
//{
//    var selector = "#sourcePrice";
//    var fadeSelector = "table";
//    $(selector).empty();
//    var value = this.value();
//    setOpacity(fadeSelector, 0.2);

//    var n = this.value();
//    $.getJSON(getPriceListsUrl + '?' + Math.random(), { subcId: n })
//       .done(function (json) {

//           for (var i = 0; i < json.length; i++) {
//               DrawPriceListRow(selector, "spl", json[i]);
//           };
//       })
// .fail(function (jqxhr, textStatus, error) {
//     var err = textStatus + ", " + error;
//     console.log("Request Failed: " + err);
// })
//  .always(function () {
//      setOpacity(fadeSelector, 1);
//  });
//}